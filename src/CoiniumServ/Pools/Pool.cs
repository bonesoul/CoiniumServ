#region License
// 
//     MIT License
//
//     CoiniumServ - Crypto Currency Mining Pool Server Software
//     Copyright (C) 2013 - 2017, CoiniumServ Project
//     Hüseyin Uslu, shalafiraistlin at gmail dot com
//     https://github.com/bonesoul/CoiniumServ
// 
//     Permission is hereby granted, free of charge, to any person obtaining a copy
//     of this software and associated documentation files (the "Software"), to deal
//     in the Software without restriction, including without limitation the rights
//     to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//     copies of the Software, and to permit persons to whom the Software is
//     furnished to do so, subject to the following conditions:
//     
//     The above copyright notice and this permission notice shall be included in all
//     copies or substantial portions of the Software.
//     
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//     FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//     LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//     OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//     SOFTWARE.
// 
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using CoiniumServ.Accounts;
using CoiniumServ.Algorithms;
using CoiniumServ.Banning;
using CoiniumServ.Configuration;
using CoiniumServ.Container;
using CoiniumServ.Daemon;
using CoiniumServ.Jobs.Manager;
using CoiniumServ.Mining;
using CoiniumServ.Payments;
using CoiniumServ.Persistance.Layers;
using CoiniumServ.Persistance.Layers.Hybrid;
using CoiniumServ.Persistance.Layers.Mpos;
using CoiniumServ.Persistance.Layers.Null;
using CoiniumServ.Persistance.Providers;
using CoiniumServ.Persistance.Providers.MySql;
using CoiniumServ.Server.Mining;
using CoiniumServ.Server.Mining.Service;
using CoiniumServ.Shares;
using CoiniumServ.Utils.Helpers;
using Nancy.TinyIoc;
using Newtonsoft.Json;
using Serilog;

namespace CoiniumServ.Pools
{
    /// <summary>
    /// Contains pool services and server.
    /// </summary>
    public class Pool : IPool
    {
        public bool Initialized { get; private set; }

        public double Hashrate { get; private set; }

        public Dictionary<string, double> RoundShares { get; private set; }

        public IPoolConfig Config { get; private set; }

        public IHashAlgorithm HashAlgorithm { get; private set; }

        public IMinerManager MinerManager { get; private set; }

        public INetworkInfo NetworkInfo { get; private set; }

        public IBlockRepository BlockRepository { get; private set; }

        public IPaymentRepository PaymentRepository { get; private set; }

        public IDaemonClient Daemon { get; private set; }

        public IAccountManager AccountManager { get; private set; }

        public string ServiceResponse { get; private set; }

        private readonly IObjectFactory _objectFactory;

        private IJobManager _jobManager;

        private IShareManager _shareManager;

        private IBanManager _banningManager;

        private IStorageLayer _storage;

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
            Initialized = false; // mark the pool as un-initiliazed until all services are up and running.

            // ensure dependencies are supplied.
            Enforce.ArgumentNotNull(() => poolConfig);
            Enforce.ArgumentNotNull(() => configManager);
            Enforce.ArgumentNotNull(() => objectFactory);

            _configManager = configManager;
            _objectFactory = objectFactory;
            Config = poolConfig;

            _logger = Log.ForContext<Pool>().ForContext("Component", poolConfig.Coin.Name);
        }

        public void Initialize()
        {
            if (Initialized)
                return;

            try
            {
                if (!Config.Valid) // make sure we have valid configuration.
                {
                    _logger.Error("Can't start pool as configuration is not valid.");
                    return;
                }

                GenerateInstanceId(); // generate unique instance id for the pool.

                if (!InitHashAlgorithm()) // init the hash algorithm required by the coin.
                    return;

                if (!InitDaemonClient()) // init the coin daemon client.
                    return;

                if (!InitStorage()) // init storage support.
                    return;

                if (!InitCoreServices()) // init core services.
                    return;

                if (!InitStatisticsServices()) // init statistics services.
                    return;

                if (!InitNetworkServers()) // init network servers.
                    return;

                Initialized = true;
            }
            catch (Exception e)
            {
                _logger.Error("Pool initilization failed; {0:l}", e);
                Initialized = false;
            }
        }

        private bool InitHashAlgorithm()
        {
            try
            {
                HashAlgorithm = _objectFactory.GetHashAlgorithm(Config.Coin);
                _shareMultiplier = Math.Pow(2, 32) / HashAlgorithm.Multiplier; // will be used in hashrate calculation.
                return true;
            }
            catch (TinyIoCResolutionException)
            {
                _logger.Error("Unknown hash algorithm: {0:l}, pool initilization failed", Config.Coin.Algorithm);
                return false;
            }            
        }

        private bool InitDaemonClient()
        {
            if (Config.Daemon == null || Config.Daemon.Valid == false)
            {
                _logger.Error("Coin daemon configuration is not valid!");
                return false;
            }

            Daemon = _objectFactory.GetDaemonClient(Config.Daemon, Config.Coin);
            return true;
        }

        private bool InitStorage()
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
                _storage = _objectFactory.GetStorageLayer(StorageLayers.Hybrid, providers, Daemon, Config);

            else if (Config.Storage.Layer is MposStorageConfig)
                _storage = _objectFactory.GetStorageLayer(StorageLayers.Mpos, providers, Daemon, Config);

            else if (Config.Storage.Layer is NullStorageConfig)
                _storage = _objectFactory.GetStorageLayer(StorageLayers.Empty, providers, Daemon, Config);

            return true;
        }

        private bool InitCoreServices()
        {
            AccountManager = _objectFactory.GetAccountManager(_storage, Config);
            MinerManager = _objectFactory.GetMinerManager(Config, _storage, AccountManager);

            var jobTracker = _objectFactory.GetJobTracker(Config);
            _shareManager = _objectFactory.GetShareManager(Config, Daemon, jobTracker, _storage);
            _objectFactory.GetVardiffManager(Config, _shareManager);
            _banningManager = _objectFactory.GetBanManager(Config, _shareManager);
            _jobManager = _objectFactory.GetJobManager(Config, Daemon, jobTracker, _shareManager, MinerManager, HashAlgorithm);
            _jobManager.Initialize(InstanceId);

            var blockProcessor = _objectFactory.GetBlockProcessor(Config, Daemon, _storage);
            var blockAccounter = _objectFactory.GetBlockAccounter(Config, _storage, AccountManager);
            var paymentProcessor = _objectFactory.GetPaymentProcessor(Config, _storage, Daemon, AccountManager);
            _objectFactory.GetPaymentManager(Config, blockProcessor, blockAccounter, paymentProcessor);

            return true;
        }

        private bool InitStatisticsServices()
        {
            NetworkInfo = _objectFactory.GetNetworkInfo(Daemon, HashAlgorithm, Config);
            BlockRepository = _objectFactory.GetBlockRepository(_storage);
            PaymentRepository = _objectFactory.GetPaymentRepository(_storage);

            return true;
        }

        private bool InitNetworkServers()
        {
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

            foreach (var server in _servers)
            {
                server.Key.Start();
            }

            return true;
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

        public void Recache()
        {
            if (!Initialized)
                return;

            BlockRepository.Recache(); // recache the blocks.
            NetworkInfo.Recache(); // let network statistics recache.
            CalculateHashrate(); // calculate the pool hashrate.

            RoundShares = _storage.GetCurrentShares(); // recache current round.

            // cache the json-service response
            ServiceResponse = JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        }

        private void CalculateHashrate()
        {
            // read hashrate stats.
            var windowTime = TimeHelpers.NowInUnixTimestamp() - _configManager.StatisticsConfig.HashrateWindow;
            _storage.DeleteExpiredHashrateData(windowTime);
            var hashrates = _storage.GetHashrateData(windowTime);

            double total = hashrates.Sum(pair => pair.Value);
            Hashrate = Convert.ToUInt64(_shareMultiplier * total / _configManager.StatisticsConfig.HashrateWindow);
        }
    }
}
