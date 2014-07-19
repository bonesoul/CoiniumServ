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
using CoiniumServ.Coin.Helpers;
using CoiniumServ.Cryptology.Algorithms;
using CoiniumServ.Daemon;
using CoiniumServ.Factories;
using CoiniumServ.Mining.Banning;
using CoiniumServ.Mining.Jobs.Manager;
using CoiniumServ.Mining.Jobs.Tracker;
using CoiniumServ.Mining.Miners;
using CoiniumServ.Mining.Pools.Config;
using CoiniumServ.Mining.Pools.Statistics;
using CoiniumServ.Mining.Shares;
using CoiniumServ.Mining.Vardiff;
using CoiniumServ.Payments;
using CoiniumServ.Persistance;
using CoiniumServ.Server;
using CoiniumServ.Server.Mining;
using CoiniumServ.Server.Mining.Service;
using CoiniumServ.Utils.Helpers.Validation;
using Serilog;

namespace CoiniumServ.Mining.Pools
{
    /// <summary>
    /// Contains pool services and server.
    /// </summary>
    public class Pool : IPool
    {
        public IPoolConfig Config { get; private set; }

        public IPerPool Statistics { get; private set; }

        private readonly IObjectFactory _objectFactory;

        private readonly IServiceFactory _serviceFactory;
        private readonly IJobTrackerFactory _jobTrackerFactory;
        private readonly IJobManagerFactory _jobManagerFactory;
        private readonly IShareManagerFactory _shareManagerFactory;
        private readonly IMinerManagerFactory _minerManagerFactory;
        private readonly IStorageFactory _storageFactory;
        private readonly IPaymentProcessorFactory _paymentProcessorFactory;
        private readonly IStatisticsObjectFactory _statisticsObjectFactory;
        private readonly IVardiffManagerFactory _vardiffManagerFactory;
        private readonly IBanManagerFactory _banningManagerFactory;

        private IDaemonClient _daemonClient;
        private readonly IServerFactory _serverFactory;
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

        private ILogger _logger;

        /// <summary>
        /// Instance id of the pool.
        /// </summary>
        public UInt32 InstanceId { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Pool" /> class.
        /// </summary>
        /// <param name="objectFactory"></param>
        /// <param name="serverFactory">The server factory.</param>
        /// <param name="serviceFactory">The service factory.</param>
        /// <param name="client">The client.</param>
        /// <param name="minerManagerFactory">The miner manager factory.</param>
        /// <param name="jobTrackerFactory"></param>
        /// <param name="jobManagerFactory">The job manager factory.</param>
        /// <param name="shareManagerFactory">The share manager factory.</param>
        /// <param name="storageFactory"></param>
        /// <param name="paymentProcessorFactory"></param>
        /// <param name="statisticsObjectFactory"></param>
        /// <param name="vardiffManagerFactory"></param>
        /// <param name="banningManagerFactory"></param>
        public Pool(
            IObjectFactory objectFactory,
            IServerFactory serverFactory, 
            IServiceFactory serviceFactory,
            IMinerManagerFactory minerManagerFactory, 
            IJobTrackerFactory jobTrackerFactory,
            IJobManagerFactory jobManagerFactory, 
            IShareManagerFactory shareManagerFactory,
            IStorageFactory storageFactory,
            IPaymentProcessorFactory paymentProcessorFactory,
            IStatisticsObjectFactory statisticsObjectFactory, 
            IVardiffManagerFactory vardiffManagerFactory,
            IBanManagerFactory banningManagerFactory)
        {

            Enforce.ArgumentNotNull(objectFactory, "IObjectFactory");

            Enforce.ArgumentNotNull(serverFactory, "IServerFactory");
            Enforce.ArgumentNotNull(serviceFactory, "IServiceFactory");
            Enforce.ArgumentNotNull(minerManagerFactory, "IMinerManagerFactory");
            Enforce.ArgumentNotNull(jobTrackerFactory, "IJobTrackerFactory");
            Enforce.ArgumentNotNull(jobManagerFactory, "IJobManagerFactory");
            Enforce.ArgumentNotNull(shareManagerFactory, "IShareManagerFactory");
            Enforce.ArgumentNotNull(storageFactory, "IStorageFactory");
            Enforce.ArgumentNotNull(paymentProcessorFactory, "IPaymentProcessorFactory");
            Enforce.ArgumentNotNull(vardiffManagerFactory, "IVardiffManagerFactory");
            Enforce.ArgumentNotNull(banningManagerFactory, "IBanningManagerFactory");

            _objectFactory = objectFactory;

            _minerManagerFactory = minerManagerFactory;
            _jobManagerFactory = jobManagerFactory;
            _jobTrackerFactory = jobTrackerFactory;
            _shareManagerFactory = shareManagerFactory;
            _serverFactory = serverFactory;
            _serviceFactory = serviceFactory;
            _storageFactory = storageFactory;
            _paymentProcessorFactory = paymentProcessorFactory;
            _statisticsObjectFactory = statisticsObjectFactory;
            _vardiffManagerFactory = vardiffManagerFactory;
            _banningManagerFactory = banningManagerFactory;
        }

        /// <summary>
        /// Initializes the specified bind ip.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <exception cref="System.ArgumentNullException">config;config.Daemon can not be null!</exception>
        public void Initialize(IPoolConfig config)
        {
            Config = config;
            _logger = Log.ForContext<Pool>().ForContext("Component", Config.Coin.Name);
            
            // TODO: validate pool central wallet & rewards within the startup.

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

            _daemonClient = _objectFactory.GetDaemonClient(Config.Daemon, Config.Coin);
        }

        private void InitManagers()
        {
            // init the algorithm
            _hashAlgorithm = _objectFactory.GetHashAlgorithm(Config.Coin.Algorithm);

            _storage = _storageFactory.Get(Storages.Redis, Config);

            _paymentProcessor = _paymentProcessorFactory.Get(_daemonClient, _storage, Config.Wallet, Config.Coin);
            _paymentProcessor.Initialize(Config.Payments);

            _minerManager = _minerManagerFactory.Get(_daemonClient, Config.Coin);

            _jobTracker = _jobTrackerFactory.Get();

            _shareManager = _shareManagerFactory.Get(_daemonClient, _jobTracker, _storage, Config.Coin);

            _vardiffManager = _vardiffManagerFactory.Get(_shareManager, Config.Stratum.Vardiff, Config.Coin);

            _banningManager = _banningManagerFactory.Get( _shareManager,Config.Banning,Config.Coin);

            _jobManager = _jobManagerFactory.Get(_daemonClient, _jobTracker, _shareManager, _minerManager,
                _hashAlgorithm, Config.Wallet, Config.Rewards, Config.Coin);

            _jobManager.Initialize(InstanceId);

            var latestBlocks = _statisticsObjectFactory.GetLatestBlocks(_storage);
            var blockStats = _statisticsObjectFactory.GetBlockStats(latestBlocks, _storage);
            Statistics = _statisticsObjectFactory.GetPerPoolStats(Config, _daemonClient, _minerManager, _hashAlgorithm, blockStats, _storage);
        }

        private void InitServers()
        {
            _servers = new Dictionary<IMiningServer, IRpcService>();

            // TODO: we don't need here a server config list as a pool can host only one instance of stratum and one vanilla server.
            // we must be dictative here, using a server list may cause situations we don't want (multiple stratum configs etc..)

            if (Config.Stratum != null && Config.Stratum.Enabled)
            {
                var stratumServer = _serverFactory.Get("Stratum", this, _minerManager, _jobManager, _banningManager, Config.Coin);
                var stratumService = _serviceFactory.Get("Stratum", Config.Coin, _shareManager, _daemonClient);
                stratumServer.Initialize(Config.Stratum);

                _servers.Add(stratumServer, stratumService);
            }

            if (Config.Vanilla != null && Config.Vanilla.Enabled)
            {
                var vanillaServer = _serverFactory.Get("Vanilla", this, _minerManager, _jobManager, _banningManager, Config.Coin);
                var vanillaService = _serviceFactory.Get("Vanilla", Config.Coin, _shareManager, _daemonClient);

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
                                               "Network hashrate: {11:l} " +
                                               "{12:l}",
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
                miningInfo.NetworkHashps.GetReadableHashrate(),
                "Services: " + _servers.Select(pair => pair.Key)
                    .Aggregate(string.Empty,
                        (current, server) =>
                            current +
                            string.Format("{0} @ {1}:{2}, ", server.Config.Name.ToLower(), server.BindIP, server.Port))
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
