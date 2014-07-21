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

using CoiniumServ.Cryptology.Algorithms;
using CoiniumServ.Daemon;
using CoiniumServ.Daemon.Config;
using CoiniumServ.Mining.Banning;
using CoiniumServ.Mining.Jobs.Manager;
using CoiniumServ.Mining.Jobs.Tracker;
using CoiniumServ.Mining.Miners;
using CoiniumServ.Mining.Pools;
using CoiniumServ.Mining.Pools.Config;
using CoiniumServ.Mining.Shares;
using CoiniumServ.Persistance;
using CoiniumServ.Repository.Context;
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

        #endregion
    }
}
