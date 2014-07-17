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
using CoiniumServ.Crypto.Algorithms;
using CoiniumServ.Daemon;
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

        private readonly IDaemonClient _daemonClient;
        private readonly IServerFactory _serverFactory;
        private readonly IServiceFactory _serviceFactory;
        private readonly IJobTrackerFactory _jobTrackerFactory;
        private readonly IJobManagerFactory _jobManagerFactory;
        private readonly IShareManagerFactory _shareManagerFactory;
        private readonly IMinerManagerFactory _minerManagerFactory;
        private readonly IHashAlgorithmFactory _hashAlgorithmFactory;
        private readonly IStorageFactory _storageFactory;
        private readonly IPaymentProcessorFactory _paymentProcessorFactory;
        private readonly IStatisticsObjectFactory _statisticsObjectFactory;
        private readonly IVardiffManagerFactory _varddManagerFactory;

        private IMinerManager _minerManager;
        private IJobTracker _jobTracker;
        private IJobManager _jobManager;
        private IShareManager _shareManager;
        private IStorage _storage;
        private IHashAlgorithm _hashAlgorithm;
        private IPaymentProcessor _paymentProcessor;
        private IVardiffManager _vardiffManager;

        private Dictionary<IMiningServer, IRpcService> _servers;

        /// <summary>
        /// Instance id of the pool.
        /// </summary>
        public UInt32 InstanceId { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Pool" /> class.
        /// </summary>
        /// <param name="hashAlgorithmFactory">The hash algorithm factory.</param>
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
        public Pool(
            IHashAlgorithmFactory hashAlgorithmFactory, 
            IServerFactory serverFactory, 
            IServiceFactory serviceFactory,
            IDaemonClient client, 
            IMinerManagerFactory minerManagerFactory, 
            IJobTrackerFactory jobTrackerFactory,
            IJobManagerFactory jobManagerFactory, 
            IShareManagerFactory shareManagerFactory,
            IStorageFactory storageFactory,
            IPaymentProcessorFactory paymentProcessorFactory,
            IStatisticsObjectFactory statisticsObjectFactory, 
            IVardiffManagerFactory vardiffManagerFactory)
        {
            Enforce.ArgumentNotNull(hashAlgorithmFactory, "IHashAlgorithmFactory");
            Enforce.ArgumentNotNull(serverFactory, "IServerFactory");
            Enforce.ArgumentNotNull(serviceFactory, "IServiceFactory");
            Enforce.ArgumentNotNull(client, "IDaemonClient");
            Enforce.ArgumentNotNull(minerManagerFactory, "IMinerManagerFactory");
            Enforce.ArgumentNotNull(jobTrackerFactory, "IJobTrackerFactory");
            Enforce.ArgumentNotNull(jobManagerFactory, "IJobManagerFactory");
            Enforce.ArgumentNotNull(shareManagerFactory, "IShareManagerFactory");
            Enforce.ArgumentNotNull(storageFactory, "IStorageFactory");
            Enforce.ArgumentNotNull(paymentProcessorFactory, "IPaymentProcessorFactory");
            Enforce.ArgumentNotNull(vardiffManagerFactory, "IVardiffManagerFactory");

            _daemonClient = client;
            _minerManagerFactory = minerManagerFactory;
            _jobManagerFactory = jobManagerFactory;
            _jobTrackerFactory = jobTrackerFactory;
            _shareManagerFactory = shareManagerFactory;
            _serverFactory = serverFactory;
            _serviceFactory = serviceFactory;
            _hashAlgorithmFactory = hashAlgorithmFactory;
            _storageFactory = storageFactory;
            _paymentProcessorFactory = paymentProcessorFactory;
            _statisticsObjectFactory = statisticsObjectFactory;
            _varddManagerFactory = vardiffManagerFactory;

            GenerateInstanceId();
        }

        /// <summary>
        /// Initializes the specified bind ip.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <exception cref="System.ArgumentNullException">config;config.Daemon can not be null!</exception>
        public void Initialize(IPoolConfig config)
        {
            Config = config;
            
            // TODO: validate pool central wallet & rewards within the startup.

            InitDaemon();            
            InitManagers();
            InitServers();
            PrintPoolInfo();
        }

        private void InitDaemon()
        {
            if (Config.Daemon == null || Config.Daemon.Valid == false)
                Log.ForContext<Pool>().Error("Coin daemon configuration is not valid!");

            _daemonClient.Initialize(Config.Daemon);
        }

        private void InitManagers()
        {
            // init the algorithm
            _hashAlgorithm = _hashAlgorithmFactory.Get(Config.Coin.Algorithm);

            _storage = _storageFactory.Get(Storages.Redis, Config);

            _paymentProcessor = _paymentProcessorFactory.Get(_daemonClient, _storage, Config.Wallet);
            _paymentProcessor.Initialize(Config.Payments);

            _minerManager = _minerManagerFactory.Get(_daemonClient);

            _jobTracker = _jobTrackerFactory.Get();

            _shareManager = _shareManagerFactory.Get(_daemonClient, _jobTracker, _storage);

            _vardiffManager = _varddManagerFactory.Get(Config.Stratum.Vardiff, _shareManager);

            _jobManager = _jobManagerFactory.Get(_daemonClient, _jobTracker, _shareManager, _minerManager, _hashAlgorithm, Config.Wallet, Config.Rewards);
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
                var stratumServer = _serverFactory.Get("Stratum", this, _minerManager, _jobManager);
                var stratumService = _serviceFactory.Get("Stratum", Config.Coin, _shareManager, _daemonClient);
                stratumServer.Initialize(Config.Stratum);

                _servers.Add(stratumServer, stratumService);
            }

            if (Config.Vanilla != null && Config.Vanilla.Enabled)
            {
                var vanillaServer = _serverFactory.Get("Vanilla", this, _minerManager, _jobManager);
                var vanillaService = _serviceFactory.Get("Vanilla", Config.Coin, _shareManager, _daemonClient);

                vanillaServer.Initialize(Config.Vanilla);

                _servers.Add(vanillaServer, vanillaService);
            }
        }

        public void Start()
        {
            if (!Config.Valid)
            {
                Log.ForContext<Pool>().Error("Can't start pool as configuration is not valid.");
                return;
            }                

            foreach (var server in _servers)
            {
                server.Key.Start();
            }
        }

        private void PrintPoolInfo()
        {
            var info = _daemonClient.GetInfo();
            var miningInfo = _daemonClient.GetMiningInfo();

            // TODO: add downloading blocks information from getblocktemplate().
            // TODO: make this multi-line & readable.
            // TODO: read services from config so that we can print pool info even before starting the servers.
            Log.ForContext<Pool>().Information("Pool started for {0:l}\r\n" +
                                               "Coin symbol: {1:l} algorithm: {2:l}\r\n" +
                                               "Coin version: {3} protocol: {4} wallet: {5}\r\n" +
                                               "Daemon network: {6:l} peers: {7} blocks: {8} errors: {9:l}\r\n" +
                                               "Network difficulty: {10:0.00000000} block difficulty: {11:0.00}\r\n" +
                                               "Network hashrate: {12:l}\r\n" +
                                               "{13:l}\r\n",
                Config.Coin.Name,
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
            Log.ForContext<Pool>().Debug("Generated cryptographically random instance Id: {0}", InstanceId);
        }
    }
}
