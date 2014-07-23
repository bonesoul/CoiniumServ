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
using System.Security.Cryptography;
using CoiniumServ.Banning;
using CoiniumServ.Coin.Helpers;
using CoiniumServ.Cryptology.Algorithms;
using CoiniumServ.Daemon;
using CoiniumServ.Factories;
using CoiniumServ.Jobs.Manager;
using CoiniumServ.Jobs.Tracker;
using CoiniumServ.Miners;
using CoiniumServ.Payments;
using CoiniumServ.Persistance;
using CoiniumServ.Pools.Config;
using CoiniumServ.Server.Mining;
using CoiniumServ.Server.Mining.Service;
using CoiniumServ.Shares;
using CoiniumServ.Statistics;
using CoiniumServ.Utils.Helpers.Validation;
using CoiniumServ.Vardiff;
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

        private readonly IObjectFactory _objectFactory;

        private IDaemonClient _daemonClient;
        private IMinerManager _minerManager;
        private IJobTracker _jobTracker;
        private IJobManager _jobManager;
        private IShareManager _shareManager;
        private IStorage _storage;
        private IHashAlgorithm _hashAlgorithm;
        private IPaymentProcessor _paymentProcessor;
        private IVardiffManager _vardiffManager;
        private IBanManager _banningManager;

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
            InitManagers();
            InitServers();
            PrintPoolInfo();
        }

        private void InitDaemon()
        {
            if (Config.Daemon == null || Config.Daemon.Valid == false)
                _logger.Error("Coin daemon configuration is not valid!");

            _daemonClient = _objectFactory.GetDaemonClient(Config.Coin.Name, Config.Daemon);
        }

        private void InitManagers()
        {
            // init the algorithm
            _hashAlgorithm = _objectFactory.GetHashAlgorithm(Config.Coin.Algorithm);

            _storage = _objectFactory.GetStorage(Storages.Redis, Config);

            _paymentProcessor = _objectFactory.GetPaymentProcessor(Config.Coin.Name, _daemonClient, _storage, Config.Wallet);
            _paymentProcessor.Initialize(Config.Payments);

            _minerManager = _objectFactory.GetMiningManager(Config.Coin.Name, _daemonClient);

            _jobTracker = _objectFactory.GetJobTracker();

            _shareManager = _objectFactory.GetShareManager(Config.Coin.Name, _daemonClient, _jobTracker, _storage);

            _vardiffManager = _objectFactory.GetVardiffManager(Config.Coin.Name, _shareManager, Config.Stratum.Vardiff);

            _banningManager = _objectFactory.GetBanManager(Config.Coin.Name, _shareManager, Config.Banning);

            _jobManager = _objectFactory.GetJobManager(Config.Coin.Name, _daemonClient, _jobTracker, _shareManager, _minerManager,
                _hashAlgorithm, Config.Wallet, Config.Rewards);

            _jobManager.Initialize(InstanceId);

            var latestBlocks = _objectFactory.GetLatestBlocks(_storage);
            var blockStats = _objectFactory.GetBlockStats(latestBlocks, _storage);
            Statistics = _objectFactory.GetPerPoolStats(Config, _daemonClient, _minerManager, _hashAlgorithm, blockStats, _storage);
        }

        private void InitServers()
        {
            _servers = new Dictionary<IMiningServer, IRpcService>();

            // TODO: we don't need here a server config list as a pool can host only one instance of stratum and one vanilla server.
            // we must be dictative here, using a server list may cause situations we don't want (multiple stratum configs etc..)

            if (Config.Stratum != null && Config.Stratum.Enabled)
            {
                var stratumServer = _objectFactory.GetMiningServer("Stratum", this, _minerManager, _jobManager, _banningManager, Config.Coin);
                var stratumService = _objectFactory.GetMiningService("Stratum", Config.Coin, _shareManager, _daemonClient);
                stratumServer.Initialize(Config.Stratum);

                _servers.Add(stratumServer, stratumService);
            }

            if (Config.Vanilla != null && Config.Vanilla.Enabled)
            {
                var vanillaServer = _objectFactory.GetMiningServer("Vanilla", this, _minerManager, _jobManager, _banningManager, Config.Coin);
                var vanillaService = _objectFactory.GetMiningService("Vanilla", Config.Coin, _shareManager, _daemonClient);

                vanillaServer.Initialize(Config.Vanilla);

                _servers.Add(vanillaServer, vanillaService);
            }
        }

        private void PrintPoolInfo()
        {
            var info = _daemonClient.GetInfo();
            var miningInfo = _daemonClient.GetMiningInfo();

            // TODO: add downloading blocks information from getblocktemplate().
            // TODO: somecoins like namecoin do not have the method getmininginfo(), so divide this information and handle exceptions.
            // TODO: read services from config so that we can print pool info even before starting the servers.
            _logger.Information("Coin symbol: {0:l} algorithm: {1:l} " +
                                               "Coin version: {2} protocol: {3} wallet: {4} " +
                                               "Daemon network: {5:l} peers: {6} blocks: {7} errors: {8:l} " +
                                               "Network difficulty: {9:0.00000000} block difficulty: {10:0.00} " +
                                               "Network hashrate: {11:l} ",
                Config.Coin.Symbol,
                Config.Coin.Algorithm,
                info.Version,
                info.ProtocolVersion,
                info.WalletVersion,
                info.Testnet ? "testnet" : "mainnet",
                info.Connections, info.Blocks,
                string.IsNullOrEmpty(info.Errors) ? "none" : info.Errors,
                miningInfo.Difficulty,
                miningInfo.Difficulty * _hashAlgorithm.Multiplier,
                miningInfo.NetworkHashps.GetReadableHashrate()
                );                
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
    }
}
