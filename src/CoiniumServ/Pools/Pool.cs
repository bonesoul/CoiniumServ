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
using CoiniumServ.Configuration;
using CoiniumServ.Cryptology.Algorithms;
using CoiniumServ.Daemon;
using CoiniumServ.Factories;
using CoiniumServ.Jobs.Manager;
using CoiniumServ.Miners;
using CoiniumServ.Persistance.Layers;
using CoiniumServ.Persistance.Layers.Empty;
using CoiniumServ.Persistance.Layers.Hybrid;
using CoiniumServ.Persistance.Layers.Mpos;
using CoiniumServ.Persistance.Providers;
using CoiniumServ.Persistance.Providers.MySql;
using CoiniumServ.Server.Mining;
using CoiniumServ.Server.Mining.Service;
using CoiniumServ.Shares;
using CoiniumServ.Utils.Helpers.Time;
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
        public bool Enabled { get; private set; }

        public ulong Hashrate { get; private set; }

        public Dictionary<string, double> RoundShares { get; private set; }

        public IPoolConfig Config { get; private set; }

        public IHashAlgorithm HashAlgorithm { get; private set; }

        public IMinerManager MinerManager { get; private set; }

        public INetworkInfo NetworkInfo { get; private set; }

        public IBlocksCache BlocksCache { get; private set; }
        
        public IDaemonClient Daemon { get; private set; }

        // object factory.
        private readonly IObjectFactory _objectFactory;

        // dependent objects.
        
        private IJobManager _jobManager;
        
        private IShareManager _shareManager;       
        
        private IBanManager _banningManager;
        
        private IStorageLayer _storageLayer;

        private readonly IConfigManager _configManager;

        private Dictionary<IMiningServer, IRpcService> _servers;

        private double _shareMultiplier; // share multiplier to be used in hashrate calculation.

        private readonly ILogger _logger;

        /// <summary>
        /// Instance id of the pool.
        /// </summary>
        public UInt32 InstanceId { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Pool" /> class.
        /// </summary>
        /// <param name="poolConfig"></param>
        /// <param name="configManager"></param>
        /// <param name="objectFactory"></param>
        public Pool(IPoolConfig poolConfig, IConfigManager configManager, IObjectFactory objectFactory)
        {
            Enforce.ArgumentNotNull(() => poolConfig); // make sure we have a pool-config instance supplied.
            Enforce.ArgumentNotNull(() => configManager); // make sure we have a config-manager instance supplied.
            Enforce.ArgumentNotNull(() => objectFactory); // make sure we have a objectFactory instance supplied.

            // TODO: validate pool central wallet & rewards within the startup.

            _configManager = configManager;
            _objectFactory = objectFactory;
            Config = poolConfig;
            _logger = Log.ForContext<Pool>().ForContext("Component", Config.Coin.Name);                                   

            GenerateInstanceId(); // generate unique instance id for the pool.

            try
            {
                InitDaemon(); // init coin daemon.
                InitStorage(); // init storage support.
                InitManagers(); // init managers.
                InitServers(); // init servers.
                Enabled = true;
            }
            catch (Exception e)
            {
                _logger.Error("Error initializing pool; {0:l}", e);
                Enabled = false;
            }
        }

        private void InitDaemon()
        {
            if (Config.Daemon == null || Config.Daemon.Valid == false)
            {
                _logger.Error("Coin daemon configuration is not valid!");
                return;
            }

            Daemon = _objectFactory.GetDaemonClient(Config.Daemon, Config.Coin);
            HashAlgorithm = _objectFactory.GetHashAlgorithm(Config.Coin.Algorithm);
            NetworkInfo = _objectFactory.GetNetworkInfo(Daemon, HashAlgorithm, Config);
            
            _shareMultiplier = Math.Pow(2, 32) / HashAlgorithm.Multiplier; // will be used in hashrate calculation.
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

            // start the migration manager if needed
            if (Config.Storage.Layer is HybridStorageConfig)
                _objectFactory.GetMigrationManager((IMySqlProvider)providers.First(p => p is MySqlProvider), Config); // run migration manager.

            // load the storage layer.
            if (Config.Storage.Layer is HybridStorageConfig)
                _storageLayer = _objectFactory.GetStorageLayer(StorageLayers.Hybrid, providers, Daemon, Config);
            else if (Config.Storage.Layer is MposStorageLayerConfig)
                _storageLayer = _objectFactory.GetStorageLayer(StorageLayers.Mpos, providers, Daemon, Config);
            else if (Config.Storage.Layer is EmptyStorageLayerConfig)
                _storageLayer = _objectFactory.GetStorageLayer(StorageLayers.Empty, providers, Daemon, Config);
        }

        private void InitManagers()
        {
            try
            {
                BlocksCache = _objectFactory.GetBlocksCache(_storageLayer);
                MinerManager = _objectFactory.GetMinerManager(Config, _storageLayer);

                var jobTracker = _objectFactory.GetJobTracker(Config);
                var blockProcessor = _objectFactory.GetBlockProcessor(Config, Daemon, _storageLayer);
                _shareManager = _objectFactory.GetShareManager(Config, Daemon, jobTracker, _storageLayer, blockProcessor);
                _objectFactory.GetVardiffManager(Config, _shareManager);
                _banningManager = _objectFactory.GetBanManager(Config, _shareManager);
                _jobManager = _objectFactory.GetJobManager(Config, Daemon, jobTracker, _shareManager, MinerManager, HashAlgorithm);
                _jobManager.Initialize(InstanceId);

                _objectFactory.GetBlockAccounter(Config, _storageLayer);
                _objectFactory.GetPaymentProcessor(Config, _storageLayer, Daemon);
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
                var stratumService = _objectFactory.GetMiningService("Stratum", Config, _shareManager, Daemon);
                stratumServer.Initialize(Config.Stratum);

                _servers.Add(stratumServer, stratumService);
            }

            if (Config.Getwork != null && Config.Getwork.Enabled)
            {
                var getworkServer = _objectFactory.GetMiningServer("Getwork", Config, this, MinerManager, _jobManager, _banningManager);
                var getworkService = _objectFactory.GetMiningService("Getwork", Config, _shareManager, Daemon);

                getworkServer.Initialize(Config.Getwork);

                _servers.Add(getworkServer, getworkService);
            }
        }

        public void Start()
        {
            if (Enabled == false)
                return;

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
            if (Enabled == false)
                return;

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
            BlocksCache.Recache(); // recache the blocks.
            NetworkInfo.Recache(); // let network statistics recache.
            CalculateHashrate(); // calculate the pool hashrate.

            RoundShares = _storageLayer.GetCurrentShares(); // recache current round.

            // cache the json-service response
            ServiceResponse = JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        }

        private void CalculateHashrate()
        {
            // read hashrate stats.
            var windowTime = TimeHelpers.NowInUnixTime() - _configManager.StatisticsConfig.HashrateWindow;
            _storageLayer.DeleteExpiredHashrateData(windowTime);
            var hashrates = _storageLayer.GetHashrateData(windowTime);

            double total = hashrates.Sum(pair => pair.Value);
            Hashrate = Convert.ToUInt64(_shareMultiplier * total / _configManager.StatisticsConfig.HashrateWindow);
        }
    }
}
