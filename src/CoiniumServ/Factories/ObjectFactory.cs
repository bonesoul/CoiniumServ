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

using CoiniumServ.Banning;
using CoiniumServ.Coin.Config;
using CoiniumServ.Cryptology.Algorithms;
using CoiniumServ.Daemon;
using CoiniumServ.Daemon.Config;
using CoiniumServ.Jobs.Manager;
using CoiniumServ.Jobs.Tracker;
using CoiniumServ.Logging;
using CoiniumServ.Miners;
using CoiniumServ.Payments;
using CoiniumServ.Persistance;
using CoiniumServ.Pools;
using CoiniumServ.Pools.Config;
using CoiniumServ.Repository.Context;
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

        public IPoolManager GetPoolManager()
        {
            return _applicationContext.Container.Resolve<IPoolManager>();
        }

        public IPool GetPool(IPoolConfig poolConfig)
        {
            var @params = new NamedParameterOverloads
            {
                {"poolConfig", poolConfig},                
            };

            return _applicationContext.Container.Resolve<IPool>(@params);
        }

        #endregion

        #region pool objects

        /// <summary>
        /// Returns a new instance of daemon client.
        /// </summary>
        /// <param name="pool"></param>
        /// <param name="daemonConfig"></param>
        /// <returns></returns>
        public IDaemonClient GetDaemonClient(string pool, IDaemonConfig daemonConfig)
        {
            var @params = new NamedParameterOverloads
            {
                {"pool", pool},
                {"daemonConfig", daemonConfig},                
            };

            return _applicationContext.Container.Resolve<IDaemonClient>(@params);
        }

        public IMinerManager GetMiningManager(string pool, IDaemonClient daemonClient)
        {
            var @params = new NamedParameterOverloads
            {
                {"pool", pool},
                {"daemonClient", daemonClient},
            };

            return _applicationContext.Container.Resolve<IMinerManager>(@params);
        }

        public IJobManager GetJobManager(string pool, IDaemonClient daemonClient, IJobTracker jobTracker, IShareManager shareManager,
            IMinerManager minerManager, IHashAlgorithm hashAlgorithm, IWalletConfig walletConfig, IRewardsConfig rewardsConfig)
        {
            var @params = new NamedParameterOverloads
            {
                {"pool", pool},
                {"daemonClient", daemonClient},
                {"jobTracker", jobTracker},
                {"shareManager", shareManager},
                {"minerManager", minerManager},
                {"hashAlgorithm", hashAlgorithm},
                {"walletConfig", walletConfig},
                {"rewardsConfig", rewardsConfig},
            };

            return _applicationContext.Container.Resolve<IJobManager>(@params);
        }

        public IJobTracker GetJobTracker()
        {
            return _applicationContext.Container.Resolve<IJobTracker>();
        }

        public IShareManager GetShareManager(string pool, IDaemonClient daemonClient, IJobTracker jobTracker, IStorage storage)
        {
            var @params = new NamedParameterOverloads
            {
                {"pool", pool},
                {"daemonClient", daemonClient},
                {"jobTracker", jobTracker},
                {"storage", storage},
            };

            return _applicationContext.Container.Resolve<IShareManager>(@params);
        }

        public IPaymentProcessor GetPaymentProcessor(string pool, IDaemonClient daemonClient, IStorage storage,
            IWalletConfig walletConfig)
        {
            var @params = new NamedParameterOverloads
            {
                {"pool", pool},
                {"daemonClient", daemonClient},
                {"storage", storage},
                {"walletConfig", walletConfig},
            };

            return _applicationContext.Container.Resolve<IPaymentProcessor>(@params);
        }

        public IBanManager GetBanManager(string pool, IShareManager shareManager, IBanConfig banConfig)
        {
            var @params = new NamedParameterOverloads
            {
                {"pool", pool},
                {"shareManager", shareManager},
                {"banConfig", banConfig},
            };

            return _applicationContext.Container.Resolve<IBanManager>(@params);
        }

        public IVardiffManager GetVardiffManager(string pool, IShareManager shareManager, IVardiffConfig vardiffConfig)
        {
            var @params = new NamedParameterOverloads
            {
                {"pool", pool},
                {"shareManager", shareManager},
                {"vardiffConfig", vardiffConfig},
            };

            return _applicationContext.Container.Resolve<IVardiffManager>(@params);
        }

        #endregion

        #region pool statistics objects

        public IStatistics GetStatistics()
        {
            return _applicationContext.Container.Resolve<IStatistics>();
        }

        public IGlobal GetGlobalStatistics()
        {
            return _applicationContext.Container.Resolve<IGlobal>();
        }

        public IAlgorithms GetAlgorithmStatistics()
        {
            return _applicationContext.Container.Resolve<IAlgorithms>();
        }

        public IPools GetPoolStats()
        {
            return _applicationContext.Container.Resolve<IPools>();
        }

        public IPerPool GetPerPoolStats(IPoolConfig poolConfig, IDaemonClient daemonClient, IMinerManager minerManager, IHashAlgorithm hashAlgorithm, IBlocks blockStatistics, IStorage storage)
        {
            var @params = new NamedParameterOverloads
            {
                {"poolConfig", poolConfig},
                {"daemonClient", daemonClient},
                {"minerManager",minerManager},
                {"hashAlgorithm", hashAlgorithm},
                {"blockStatistics", blockStatistics},
                {"storage", storage},
            };

            return _applicationContext.Container.Resolve<IPerPool>(@params);
        }

        public ILatestBlocks GetLatestBlocks(IStorage storage)
        {
            var @params = new NamedParameterOverloads
            {
                {"storage", storage}
            };

            return _applicationContext.Container.Resolve<ILatestBlocks>(@params);
        }

        public IBlocks GetBlockStats(ILatestBlocks latestBlocks, IStorage storage)
        {
            var @params = new NamedParameterOverloads
            {
                {"latestBlocks", latestBlocks},
                {"storage", storage},
            };

            return _applicationContext.Container.Resolve<IBlocks>(@params);
        }

        #endregion

        #region server & service objects

        public IMiningServer GetMiningServer(string type, IPool pool, IMinerManager minerManager, IJobManager jobManager,
            IBanManager banManager, ICoinConfig coinConfig)
        {
            var @params = new NamedParameterOverloads
            {
                {"pool", pool},
                {"minerManager", minerManager},
                {"jobManager", jobManager},
                {"banManager", banManager},
                {"coinConfig", coinConfig}
            };

            return _applicationContext.Container.Resolve<IMiningServer>(type, @params);
        }

        public IRpcService GetMiningService(string type, ICoinConfig coinConfig, IShareManager shareManager, IDaemonClient daemonClient)
        {
            var @params = new NamedParameterOverloads
            {
                {"coinConfig", coinConfig},
                {"shareManager", shareManager}, 
                {"daemonClient", daemonClient}
            };

            return _applicationContext.Container.Resolve<IRpcService>(type, @params);
        }

        public IWebServer GetWebServer()
        {
            return _applicationContext.Container.Resolve<IWebServer>();
        }

        public INancyBootstrapper GetWebBootstrapper()
        {
            return _applicationContext.Container.Resolve<INancyBootstrapper>();
        }

        #endregion

        #region other objects

        public IStorage GetStorage(string type, IPoolConfig poolConfig)
        {
            var @params = new NamedParameterOverloads
            {
                {"poolConfig", poolConfig}
            };

            return _applicationContext.Container.Resolve<IStorage>(type, @params);
        }

        public ILogManager GetLogManager()
        {
            return _applicationContext.Container.Resolve<ILogManager>();
        }

        #endregion
    }
}
