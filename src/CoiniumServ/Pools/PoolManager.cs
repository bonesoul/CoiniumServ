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
using CoiniumServ.Configuration;
using CoiniumServ.Factories;
using Serilog;

namespace CoiniumServ.Pools
{
    public class PoolManager : IPoolManager
    {
        public IReadOnlyCollection<IPool> Pools { get { return _pools.Values.ToList(); } }

        private readonly Dictionary<string, IPool> _pools; 

        private readonly ILogger _logger;

        public PoolManager(IObjectFactory objectFactory , IConfigManager configManager)
        {
            _logger = Log.ForContext<PoolManager>();
            _pools = new Dictionary<string, IPool>();

            foreach (var config in configManager.PoolConfigs)
            {
                _pools.Add(config.Coin.Symbol, objectFactory.GetPool(config));
            }
        }

        public IPool GetBySymbol(string symbol)
        {
            return _pools.Values.FirstOrDefault(pair => pair.Config.Coin.Symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase));
        }

        public void Run()
        {
            foreach (var kvp in _pools)
            {
                kvp.Value.Start();
            }
        }
    }
}
