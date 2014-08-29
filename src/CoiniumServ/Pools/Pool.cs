#region License
// 
//     CoiniumServ - Crypto Currency Mining Pool Server Software
//     Copyright (C) 2013 - 2014, CoiniumServ Project - http://www.coinium.org
//     http://www.coiniumserv.com - https://github.com/CoiniumServ/CoiniumServ
// 
//     This software is dual-licensed: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
//    
//     For the terms of this license, see licenses/gpl_v3.txt.
// 
//     Alternatively, you can license this software under a commercial
//     license or white-label it as set out in licenses/commercial.txt.
// 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using CoiniumServ.Banning;
using CoiniumServ.Coin.Helpers;
using CoiniumServ.Cryptology.Algorithms;
using CoiniumServ.Daemon;
using CoiniumServ.Daemon.Exceptions;
using CoiniumServ.Factories;
using CoiniumServ.Jobs.Manager;
using CoiniumServ.Miners;
using CoiniumServ.Persistance;
using CoiniumServ.Persistance.Layers;
using CoiniumServ.Persistance.Layers.Empty;
using CoiniumServ.Persistance.Layers.Hybrid;
using CoiniumServ.Persistance.Layers.Mpos;
using CoiniumServ.Persistance.Providers;
using CoiniumServ.Persistance.Providers.MySql;
using CoiniumServ.Server.Mining;
using CoiniumServ.Server.Mining.Service;
using CoiniumServ.Shares;
using CoiniumServ.Statistics;
using CoiniumServ.Statistics.New;
using CoiniumServ.Utils.Helpers.Validation;
using Newtonsoft.Json;
using Serilog;

namespace CoiniumServ.Pools
{
    /// <summary>
    /// Contains pool services and server.
    /// </summary>
    public class Pool : IPool
    {
        public IPoolConfig Config { get; private set; }

        public IPerPool Statistics { get; private set; }
        public ulong Hashrate { get; private set; }
        public Dictionary<string, double> RoundShares { get; private set; }
        public IHashAlgorithm HashAlgorithm { get; private set; }
        public IMinerManager MinerManager { get; private set; }

        // object factory.
        private readonly IObjectFactory _objectFactory;

        // dependent objects.
        private IDaemonClient _daemonClient;       

        [JsonProperty("network")]
        private INetworkStats _networkStats;
        
        private IJobManager _jobManager;
        
        private IShareManager _shareManager;       
        
        private IBanManager _banningManager;
        
        private IStorageLayer _storageLayer;

        private IStorageOld _storageOld;

        private Dictionary<IMiningServer, IRpcService> _servers;

        private readonly ILogger _logger;

        /// <summary>
        /// Instance id of the pool.
        /// </summary>
        public UInt32 InstanceId { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Pool" /> class.
        /// </summary>
        /// <param name="poolConfig"></param>
        /// <param name="objectFactory"></param>
        public Pool(
            IPoolConfig poolConfig,
            IObjectFactory objectFactory)
        {
            Enforce.ArgumentNotNull(() => poolConfig); // make sure we have a config instance supplied.
            Enforce.ArgumentNotNull(() => objectFactory); // make sure we have a objectFactory instance supplied.

            _objectFactory = objectFactory;

            // TODO: validate pool central wallet & rewards within the startup.

            Config = poolConfig;
            
            _logger = Log.ForContext<Pool>().ForContext("Component", Config.Coin.Name);

            GenerateInstanceId();

            InitDaemon();
            InitStorage();
            InitManagers();
            InitServers();
        }

        private void InitDaemon()
        {
            if (Config.Daemon == null || Config.Daemon.Valid == false)
            {
                _logger.Error("Coin daemon configuration is not valid!");
                return;
            }

            _daemonClient = _objectFactory.GetDaemonClient(Config);
            HashAlgorithm = _objectFactory.GetHashAlgorithm(Config.Coin.Algorithm);

            // read getinfo().
            try
            {
                var info = _daemonClient.GetInfo();

                _logger.Information("Coin symbol: {0:l} algorithm: {1:l} " +
                                    "Coin version: {2:l} protocol: {3} wallet: {4} " +
                                    "Daemon network: {5:l} peers: {6} blocks: {7} errors: {8:l} ",
                    Config.Coin.Symbol,
                    Config.Coin.Algorithm,
                    info.Version,
                    info.ProtocolVersion,
                    info.WalletVersion,
                    info.Testnet ? "testnet" : "mainnet",
                    info.Connections, info.Blocks,
                    string.IsNullOrEmpty(info.Errors) ? "none" : info.Errors);
            }
            catch (RpcException e)
            {
                _logger.Error("Can not read getinfo(): {0:l}", e.Message);
                return;
            }

            // read getmininginfo().
            try
            {
                // try reading mininginfo(), some coins may not support it.
                var miningInfo = _daemonClient.GetMiningInfo();

                _logger.Information("Network difficulty: {0:0.00000000} block difficulty: {1:0.00} Network hashrate: {2:l} ",
                    miningInfo.Difficulty,
                    miningInfo.Difficulty * HashAlgorithm.Multiplier,
                    miningInfo.NetworkHashps.GetReadableHashrate());
            }
            catch (RpcException e)
            {
                _logger.Error("Can not read mininginfo() - the coin may not support the request: {0:l}", e.Message);
            }

            // read getdifficulty() to determine if it's POS coin.
            try
            {
                /*  By default proof-of-work coins return a floating point as difficulty (https://en.bitcoin.it/wiki/Original_Bitcoin_client/API_calls_lis).
                 *  Though proof-of-stake coins returns a json-object;
                 *  { "proof-of-work" : 41867.16992903, "proof-of-stake" : 0.00390625, "search-interval" : 0 }
                 *  So basically we can use this info to determine if assigned coin is a proof-of-stake one.
                 */

                var response = _daemonClient.MakeRawRequest("getdifficulty");
                if (response.Contains("proof-of-stake")) // if response contains proof-of-stake field
                    Config.Coin.IsPOS = true; // then automatically set coin-config.IsPOS to true.
            }
            catch (RpcException e)
            {
                _logger.Error("Can not read getdifficulty() - the coin may not support the request: {0:l}", e.Message);
            }
        }

        private void InitStorage()
        {
            // load the providers for the current storage layer.
            var providers =
                Config.Storage.Layer.Providers.Select(
                    providerConfig =>
                        _objectFactory.GetStorageProvider(
                            providerConfig is IMySqlProviderConfig ? StorageProviders.MySql : StorageProviders.Redis,
                            Config, providerConfig)).ToList();

            // load the storage layer.
            if (Config.Storage.Layer is HybridStorageLayerConfig)
                _storageLayer = _objectFactory.GetStorageLayer(StorageLayers.Hybrid, providers, _daemonClient, Config);
            else if (Config.Storage.Layer is MposStorageLayerConfig)
                _storageLayer = _objectFactory.GetStorageLayer(StorageLayers.Mpos, providers, _daemonClient, Config);
            else if (Config.Storage.Layer is EmptyStorageLayerConfig)
                _storageLayer = _objectFactory.GetStorageLayer(StorageLayers.Empty, providers, _daemonClient, Config);
        }

        private void InitManagers()
        {
            try
            {
                _storageOld = _objectFactory.GetOldStorage(StorageProviders.Redis, Config);

                MinerManager = _objectFactory.GetMinerManager(Config, _storageLayer);

                _networkStats = _objectFactory.GetNetworkStats(_daemonClient);

                var jobTracker = _objectFactory.GetJobTracker();

                var blockProcessor = _objectFactory.GetBlockProcessor(Config, _daemonClient);

                _shareManager = _objectFactory.GetShareManager(Config, _daemonClient, jobTracker, _storageLayer, _storageOld, blockProcessor);

                var vardiffManager = _objectFactory.GetVardiffManager(Config, _shareManager);

                _banningManager = _objectFactory.GetBanManager(Config, _shareManager);

                _jobManager = _objectFactory.GetJobManager(Config, _daemonClient, jobTracker, _shareManager, MinerManager, HashAlgorithm);
                _jobManager.Initialize(InstanceId);

                var paymentProcessor = _objectFactory.GetPaymentProcessor(Config, _daemonClient, _storageOld, blockProcessor);
                paymentProcessor.Initialize(Config.Payments);

                var latestBlocks = _objectFactory.GetLatestBlocks(_storageOld);
                var blockStats = _objectFactory.GetBlockStats(latestBlocks, _storageOld);
                Statistics = _objectFactory.GetPerPoolStats(Config, _daemonClient, MinerManager, HashAlgorithm, blockStats, _storageOld);
            }
            catch (Exception e)
            {
                _logger.Error("Pool initialization error: {0:l}", e.Message);
            }
        }

        private void InitServers()
        {
            // todo: merge this with InitManagers so we don't have use private declaration of class instances

            _servers = new Dictionary<IMiningServer, IRpcService>();

            if (Config.Stratum != null && Config.Stratum.Enabled)
            {
                var stratumServer = _objectFactory.GetMiningServer("Stratum", Config, this, MinerManager, _jobManager, _banningManager);
                var stratumService = _objectFactory.GetMiningService("Stratum", Config, _shareManager, _daemonClient);
                stratumServer.Initialize(Config.Stratum);

                _servers.Add(stratumServer, stratumService);
            }

            if (Config.Vanilla != null && Config.Vanilla.Enabled)
            {
                var vanillaServer = _objectFactory.GetMiningServer("Vanilla", Config, this, MinerManager, _jobManager, _banningManager);
                var vanillaService = _objectFactory.GetMiningService("Vanilla", Config, _shareManager, _daemonClient);

                vanillaServer.Initialize(Config.Vanilla);

                _servers.Add(vanillaServer, vanillaService);
            }
        }

        public void Start()
        {
            if (!Config.Valid)
            {
                _logger.Error("Can't start pool as configuration is not valid.");
                return;
            }

            foreach (var server in _servers)
            {
                server.Key.Start();
            }
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generates an instance Id for the pool that is cryptographically random. 
        /// </summary>
        private void GenerateInstanceId()
        {
            var rndGenerator = RandomNumberGenerator.Create(); // cryptographically random generator.
            var randomBytes = new byte[4];
            rndGenerator.GetNonZeroBytes(randomBytes); // create cryptographically random array of bytes.
            InstanceId = BitConverter.ToUInt32(randomBytes, 0); // convert them to instance Id.
            _logger.Debug("Generated cryptographically random instance Id: {0}", InstanceId);
        }

        public string ServiceResponse { get; private set; }
        public void Recache()
        {
            _networkStats.Recache(); // let network statistics recache.
            CalculateHashrate(); // calculate the pool hashrate.
            RecacheRound(); // recache current round.

            // cache the json-service response
            ServiceResponse = JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        }

        private void RecacheRound()
        {
            RoundShares = _storageOld.GetSharesForCurrentRound();
        }

        private void CalculateHashrate()
        {
            //// read hashrate stats.
            //var windowTime = TimeHelpers.NowInUnixTime() - _statisticsConfig.HashrateWindow;
            //_storage.DeleteExpiredHashrateData(windowTime);
            //var hashrates = _storage.GetHashrateData(windowTime);

            //double total = hashrates.Sum(pair => pair.Value);
            //Hashrate = Convert.ToUInt64(_shareMultiplier * total / _statisticsConfig.HashrateWindow);
        }
    }
}
