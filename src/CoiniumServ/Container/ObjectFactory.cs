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

using System.Collections.Generic;
using CoiniumServ.Accounts;
using CoiniumServ.Algorithms;
using CoiniumServ.Banning;
using CoiniumServ.Blocks;
using CoiniumServ.Coin.Config;
using CoiniumServ.Container.Context;
using CoiniumServ.Daemon;
using CoiniumServ.Daemon.Config;
using CoiniumServ.Jobs.Manager;
using CoiniumServ.Jobs.Tracker;
using CoiniumServ.Logging;
using CoiniumServ.Mining;
using CoiniumServ.Mining.Software;
using CoiniumServ.Payments;
using CoiniumServ.Persistance.Blocks;
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

namespace CoiniumServ.Container
{
    /// <summary>
    /// Object factory that creates instances of objects
    /// </summary>
    public class ObjectFactory : IObjectFactory
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

        public IDaemonManager GetPaymentDaemonManager()
        {
            return _applicationContext.Container.Resolve<IDaemonManager>();
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
        public IDaemonClient GetDaemonClient(IDaemonConfig daemonConfig, ICoinConfig coinConfig)
        {
            var @params = new NamedParameterOverloads
            {
                {"daemonConfig", daemonConfig},
                {"coinConfig", coinConfig}
            };

            return _applicationContext.Container.Resolve<IDaemonClient>(@params);
        }

        public IMinerManager GetMinerManager(IPoolConfig poolConfig, IStorageLayer storageLayer, IAccountManager accountManager)
        {
            var @params = new NamedParameterOverloads
            {
                {"poolConfig", poolConfig},
                {"storageLayer", storageLayer},
                {"accountManager", accountManager},
            };

            return _applicationContext.Container.Resolve<IMinerManager>(@params);
        }

        public IJobManager GetJobManager(IPoolConfig poolConfig, IDaemonClient daemonClient, IJobTracker jobTracker, IShareManager shareManager, IMinerManager minerManager, IHashAlgorithm hashAlgorithm)
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

        public IShareManager GetShareManager(IPoolConfig poolConfig, IDaemonClient daemonClient, IJobTracker jobTracker, IStorageLayer storageLayer)
        {
            var @params = new NamedParameterOverloads
            {
                {"poolConfig", poolConfig},
                {"daemonClient", daemonClient},
                {"jobTracker", jobTracker},
                {"storageLayer", storageLayer}
            };

            return _applicationContext.Container.Resolve<IShareManager>(@params);
        }

        public IBlockProcessor GetBlockProcessor(IPoolConfig poolConfig, IDaemonClient daemonClient, IStorageLayer storageLayer)
        {
            var @params = new NamedParameterOverloads
            {
                {"poolConfig", poolConfig},
                {"daemonClient", daemonClient},
                {"storageLayer", storageLayer},
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

        public IBlockRepository GetBlockRepository(IStorageLayer storageLayer)
        {
            var @params = new NamedParameterOverloads
            {
                {"storageLayer", storageLayer},
            };

            return _applicationContext.Container.Resolve<IBlockRepository>(@params);
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

        public IAccountManager GetAccountManager(IStorageLayer storageLayer, IPoolConfig poolConfig)
        {
            var @params = new NamedParameterOverloads
            {
                {"storageLayer", storageLayer},
                {"poolConfig", poolConfig},
            };

            return _applicationContext.Container.Resolve<IAccountManager>(@params);
        }

        #endregion

        #region payment objects

        public IPaymentManager GetPaymentManager(IPoolConfig poolConfig, IBlockProcessor blockProcessor, IBlockAccounter blockAccounter, IPaymentProcessor paymentProcessor)
        {
            var @params = new NamedParameterOverloads
            {
                {"poolConfig", poolConfig},
                {"blockProcessor", blockProcessor},
                {"blockAccounter", blockAccounter},
                {"paymentProcessor", paymentProcessor}
            };

            return _applicationContext.Container.Resolve<IPaymentManager>(@params);
        }

        public IBlockAccounter GetBlockAccounter(IPoolConfig poolConfig, IStorageLayer storageLayer, IAccountManager accountManager)
        {
            var @params = new NamedParameterOverloads
            {
                {"poolConfig", poolConfig},
                {"storageLayer", storageLayer},
                {"accountManager", accountManager}
            };

            return _applicationContext.Container.Resolve<IBlockAccounter>(@params);
        }

        public IPaymentProcessor GetPaymentProcessor(IPoolConfig poolConfig, IStorageLayer storageLayer, IDaemonClient daemonClient,
            IAccountManager accountManager)
        {
            var @params = new NamedParameterOverloads
            {
                {"poolConfig", poolConfig},
                {"storageLayer", storageLayer},
                {"daemonClient", daemonClient},
                {"accountManager", accountManager},
            };

            return _applicationContext.Container.Resolve<IPaymentProcessor>(@params);
        }

        public IPaymentRound GetPaymentRound(IPersistedBlock block, IStorageLayer storageLayer, IAccountManager accountManager)
        {
            var @params = new NamedParameterOverloads
            {
                {"block", block},
                {"storageLayer", storageLayer},
                {"accountManager", accountManager}
            };

            return _applicationContext.Container.Resolve<IPaymentRound>(@params);
        }

        public IPaymentRepository GetPaymentRepository(IStorageLayer storageLayer)
        {
            var @params = new NamedParameterOverloads
            {
                {"storageLayer", storageLayer}
            };

            return _applicationContext.Container.Resolve<IPaymentRepository>(@params);
        }

        #endregion

        #region hash algorithms

        public IHashAlgorithm GetHashAlgorithm(ICoinConfig coinConfig)
        {
            var @params = new NamedParameterOverloads
            {
                {"coinConfig", coinConfig},
            };

            return _applicationContext.Container.Resolve<IHashAlgorithm>(coinConfig.Algorithm, @params);
        }

        public IAlgorithmManager GetAlgorithmManager()
        {
            return _applicationContext.Container.Resolve<IAlgorithmManager>();
        }

        public IHashAlgorithmStatistics GetHashAlgorithmStatistics(string name)
        {
            var @params = new NamedParameterOverloads
            {
                {"name", name}
            };

            return _applicationContext.Container.Resolve<IHashAlgorithmStatistics>(@params);
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

        public ISoftwareRepository GetSoftwareRepository()
        {
            return _applicationContext.Container.Resolve<ISoftwareRepository>();
        }

        #endregion

        #region mining software

        public IMiningSoftware GetMiningSoftware(IMiningSoftwareConfig config)
        {
            var @params = new NamedParameterOverloads
            {
                {"config", config}
            };

            return _applicationContext.Container.Resolve<IMiningSoftware>(@params);
        }

        #endregion
    }
}
