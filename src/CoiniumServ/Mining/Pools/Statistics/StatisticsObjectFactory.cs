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

using CoiniumServ.Crypto.Algorithms;
using CoiniumServ.Daemon;
using CoiniumServ.Mining.Miners;
using CoiniumServ.Mining.Pools.Config;
using CoiniumServ.Persistance;
using CoiniumServ.Repository.Context;
using Nancy.TinyIoc;

namespace CoiniumServ.Mining.Pools.Statistics
{
    public class StatististicsObjectFactory : IStatisticsObjectFactory
    {
        /// <summary>
        /// The _kernel
        /// </summary>
        private readonly IApplicationContext _applicationContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatististicsObjectFactory" /> class.
        /// </summary>
        /// <param name="applicationContext">The application context.</param>
        public StatististicsObjectFactory(IApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

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
    }
}
