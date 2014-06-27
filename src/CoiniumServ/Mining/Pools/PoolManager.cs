#region License
// 
//     CoiniumServ - Crypto Currency Mining Pool Server Software
//     Copyright (C) 2013 - 2014, CoiniumServ Project - http://www.coinium.org
//     https://github.com/CoiniumServ/CoiniumServ
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
using Coinium.Common.Configuration;
using Coinium.Common.Helpers.IO;
using Coinium.Mining.Pools.Config;
using Serilog;

namespace Coinium.Mining.Pools
{
    public class PoolManager : IPoolManager
    {
        private readonly List<IPool> _pools = new List<IPool>();

        private readonly IPoolFactory _poolFactory;

        private readonly IPoolConfigFactory _poolConfigFactory;

        public PoolManager(IPoolFactory poolFactory, IPoolConfigFactory poolConfigFactory)
        {
            _poolFactory = poolFactory;
            _poolConfigFactory = poolConfigFactory;
        }

        public void Run()
        {
            LoadConfigs();            
        }

        public void LoadConfigs()
        {
            const string configRoot = "config/pools";

            var enabledPools = new List<string>();
            var files = FileHelpers.GetFilesByExtensionRecursive(configRoot, ".json");

            foreach (var file in files)
            {
                var poolConfig = _poolConfigFactory.Get(JsonConfigReader.Read(file));
                
                if (!poolConfig.Enabled) // skip pools that are not enabled.
                    continue;

                enabledPools.Add(poolConfig.Coin.Name);

                AddPool(poolConfig);
            }

            Log.ForContext<PoolManager>().Information("Discovered a total of {0} enabled pool configurations: {1}.", _pools.Count, enabledPools);
        }

        public IPool AddPool(IPoolConfig poolConfig)
        {
            var pool = _poolFactory.Create(poolConfig);
            _pools.Add(pool);

            return pool;
        }

        public IList<IPool> GetPools()
        {
            return _pools;
        }
    }
}
