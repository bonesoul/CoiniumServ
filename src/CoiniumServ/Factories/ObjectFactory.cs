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
using System.Collections.Generic;
using CoiniumServ.Banning;
using CoiniumServ.Blocks;
using CoiniumServ.Container.Context;
using CoiniumServ.Cryptology.Algorithms;
using CoiniumServ.Daemon;
using CoiniumServ.Jobs.Manager;
using CoiniumServ.Jobs.Tracker;
using CoiniumServ.Logging;
using CoiniumServ.Metrics;
using CoiniumServ.Miners;
using CoiniumServ.Payments;
using CoiniumServ.Persistance.Layers;
using CoiniumServ.Persistance.Layers.Hybrid;
using CoiniumServ.Persistance.Providers;
using CoiniumServ.Persistance.Providers.MySql;
using CoiniumServ.Pools;
using CoiniumServ.Server.Mining;
using CoiniumServ.Server.Mining.Service;
using CoiniumServ.Server.Web;
using CoiniumServ.Shares;
using CoiniumServ.Statistics;
using CoiniumServ.Vardiff;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;

namespace CoiniumServ.Factories
{
    /// <summary>
    /// Object factory that creates instances of objects
    /// </summary>
    public class ObjectFactory:IObjectFactory
    {
        #region context

        /// <summary>
        /// The application context for internal use.
        /// </summary>
        private readonly IApplicationContext _applicationContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectFactory" /> class.
        /// </summary>
        /// <param name="applicationContext">The application context.</param>
        public ObjectFactory(IApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        #endregion

        #region global objects

        public IPoolManager GetPoolManager()
        {
            return _applicationContext.Container.Resolve<IPoolManager>();
        }

        public IStatisticsManager GetStatisticsManager()
        {
            return _applicationContext.Container.Resolve<IStatisticsManager>();
        }

        public ILogManager GetLogManager()
        {
            return _applicationContext.Container.Resolve<ILogManager>();
        }

        #endregion

        #region pool objects

        public IPool GetPool(IPoolConfig poolConfig)
        {
            var @params = new NamedParameterOverloads
            {
                {"poolConfig", poolConfig},                
            };

            return _applicationContext.Container.Resolve<IPool>(@params);
        }

        /// <summary>
        /// Returns a new instance of daemon client.
        /// </summary>
        /// <returns></returns>
        public IDaemonClient GetDaemonClient(IPoolConfig poolConfig)
        {
            var @params = new NamedParameterOverloads
            {
                {"poolConfig", poolConfig}              
            };

            return _applicationContext.Container.Resolve<IDaemonClient>(@params);
        }

        public IMinerManager GetMinerManager(IPoolConfig poolConfig, IStorageLayer storageLayer)
        {
            var @params = new NamedParameterOverloads
            {
                {"poolConfig", poolConfig},
                {"storageLayer", storageLayer},
            };

            return _applicationContext.Container.Resolve<IMinerManager>(@params);
        }

        public IJobManager GetJobManager(IPoolConfig poolConfig, IDaemonClient daemonClient, IJobTracker jobTracker, IShareManager shareManager,
            IMinerManager minerManager, IHashAlgorithm hashAlgorithm)
        {
            var @params = new NamedParameterOverloads
            {
                {"poolConfig", poolConfig},
                {"daemonClient", daemonClient},
                {"jobTracker", jobTracker},
                {"shareManager", shareManager},
                {"minerManager", minerManager},
                {"hashAlgorithm", hashAlgorithm},
            };

            return _applicationContext.Container.Resolve<IJobManager>(@params);
        }

        public IJobTracker GetJobTracker(IPoolConfig poolConfig)
        {
            var @params = new NamedParameterOverloads
            {
                {"poolConfig", poolConfig},
            };

            return _applicationContext.Container.Resolve<IJobTracker>(@params);
        }

        public IShareManager GetShareManager(IPoolConfig poolConfig, IDaemonClient daemonClient, IJobTracker jobTracker, IStorageLayer storageLayer, IBlockProcessor blockProcessor)
        {
            var @params = new NamedParameterOverloads
            {
                {"poolConfig", poolConfig},
                {"daemonClient", daemonClient},
                {"jobTracker", jobTracker},
                {"storageLayer", storageLayer},
                {"blockProcessor", blockProcessor}
            };

            return _applicationContext.Container.Resolve<IShareManager>(@params);
        }

        public IPaymentProcessor GetPaymentProcessor(IPoolConfig poolConfig, IDaemonClient daemonClient, IStorageLayer storageLayer, IBlockProcessor blockProcessor)
        {
            var @params = new NamedParameterOverloads
            {
                {"poolConfig", poolConfig},
                {"daemonClient", daemonClient},
                {"storageLayer", storageLayer},
                {"blockProcessor", blockProcessor},
            };

            return _applicationContext.Container.Resolve<IPaymentProcessor>(@params);
        }

        public IBlockProcessor GetBlockProcessor(IPoolConfig poolConfig, IDaemonClient daemonClient)
        {
            var @params = new NamedParameterOverloads
            {
                {"poolConfig", poolConfig},
                {"daemonClient", daemonClient},           
            };

            return _applicationContext.Container.Resolve<IBlockProcessor>(@params);
        }

        public IBanManager GetBanManager(IPoolConfig poolConfig, IShareManager shareManager)
        {
            var @params = new NamedParameterOverloads
            {
                {"poolConfig", poolConfig},
                {"shareManager", shareManager},                
            };

            return _applicationContext.Container.Resolve<IBanManager>(@params);
        }

        public IVardiffManager GetVardiffManager(IPoolConfig poolConfig, IShareManager shareManager)
        {
            var @params = new NamedParameterOverloads
            {
                {"poolConfig", poolConfig},
                {"shareManager", shareManager},
            };

            return _applicationContext.Container.Resolve<IVardiffManager>(@params);
        }

        public INetworkInfo GetNetworkInfo(IDaemonClient daemonClient, IHashAlgorithm hashAlgorithm, IPoolConfig poolConfig)
        {
            var @params = new NamedParameterOverloads
            {
                {"daemonClient", daemonClient},
                {"hashAlgorithm", hashAlgorithm},
                {"poolConfig", poolConfig},
            };

            return _applicationContext.Container.Resolve<INetworkInfo>(@params);
        }

        public IBlocksCache GetBlocksCache(IStorageLayer storageLayer)
        {
            var @params = new NamedParameterOverloads
            {
                {"storageLayer", storageLayer},
            };

            return _applicationContext.Container.Resolve<IBlocksCache>(@params);
        }

        public IMiningServer GetMiningServer(string type, IPoolConfig poolConfig, IPool pool, IMinerManager minerManager, IJobManager jobManager, IBanManager banManager)
        {
            var @params = new NamedParameterOverloads
            {
                {"poolConfig", poolConfig},
                {"pool", pool},
                {"minerManager", minerManager},
                {"jobManager", jobManager},
                {"banManager", banManager},
            };

            return _applicationContext.Container.Resolve<IMiningServer>(type, @params);
        }

        public IRpcService GetMiningService(string type, IPoolConfig poolConfig, IShareManager shareManager, IDaemonClient daemonClient)
        {
            var @params = new NamedParameterOverloads
            {
                {"poolConfig", poolConfig},
                {"shareManager", shareManager}, 
                {"daemonClient", daemonClient}
            };

            return _applicationContext.Container.Resolve<IRpcService>(type, @params);
        }

        #endregion

        #region hash algorithms

        /// <summary>
        /// Returns instance of the given hash algorithm.
        /// </summary>
        /// <param name="algorithm"></param>
        /// <returns></returns>
        public IHashAlgorithm GetHashAlgorithm(string algorithm)
        {
            return _applicationContext.Container.Resolve<IHashAlgorithm>(algorithm);
        }

        public IAlgorithmManager GetAlgorithmManager(IPoolManager poolManager)
        {
            return _applicationContext.Container.Resolve<IAlgorithmManager>();
        }

        #endregion

        #region storage objects

        public IStorageProvider GetStorageProvider(string type, IPoolConfig poolConfig, IStorageProviderConfig config)
        {
            var @params = new NamedParameterOverloads
            {
                {"poolConfig", poolConfig},
                {"config", config}
            };

            return _applicationContext.Container.Resolve<IStorageProvider>(type, @params);
        }

        public IStorageLayer GetStorageLayer(string type, IEnumerable<IStorageProvider> providers, IDaemonClient daemonClient, IPoolConfig poolConfig)
        {
            var @params = new NamedParameterOverloads
            {
                {"providers", providers},
                {"daemonClient", daemonClient},
                {"poolConfig", poolConfig}
            };

            return type != StorageLayers.Empty
                ? _applicationContext.Container.Resolve<IStorageLayer>(type, @params)
                : _applicationContext.Container.Resolve<IStorageLayer>(type);
        }

        public IMigrationManager GetMigrationManager(IMySqlProvider provider, IPoolConfig poolConfig)
        {
            var @params = new NamedParameterOverloads
            {
                {"provider", provider},
                {"poolConfig", poolConfig},
            };

            return _applicationContext.Container.Resolve<IMigrationManager>(@params);
        }

        #endregion

        #region web-server objects

        public IWebServer GetWebServer()
        {
            return _applicationContext.Container.Resolve<IWebServer>();
        }

        public INancyBootstrapper GetWebBootstrapper()
        {
            return _applicationContext.Container.Resolve<INancyBootstrapper>();
        }

        public IMetricsManager GetMetricsManager()
        {
            return _applicationContext.Container.Resolve<IMetricsManager>();
        }

        #endregion
    }
}
