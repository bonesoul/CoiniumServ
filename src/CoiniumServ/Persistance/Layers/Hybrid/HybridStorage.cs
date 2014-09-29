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
using CoiniumServ.Daemon;
using CoiniumServ.Persistance.Providers;
using CoiniumServ.Persistance.Providers.MySql;
using CoiniumServ.Persistance.Providers.Redis;
using CoiniumServ.Pools;
using Serilog;

namespace CoiniumServ.Persistance.Layers.Hybrid
{
    public partial class HybridStorage : IStorageLayer
    {
        public bool IsEnabled { get; private set; }

        private readonly string _coin;

        private readonly IDaemonClient _daemonClient;

        private readonly IRedisProvider _redisProvider;

        private readonly IMySqlProvider _mySqlProvider;

        private readonly ILogger _logger;
        
        public HybridStorage(IEnumerable<IStorageProvider> providers, IDaemonClient daemonClient, IPoolConfig poolConfig)
        {
            _daemonClient = daemonClient;
            _logger = Log.ForContext<HybridStorage>().ForContext("Component", poolConfig.Coin.Name);
            _coin = poolConfig.Coin.Name.ToLower(); // pool's associated coin name.

            // try loading providers.
            try
            {
                foreach (var provider in providers)
                {
                    if (provider is IRedisProvider)
                        _redisProvider = (IRedisProvider)provider;
                    else if (provider is IMySqlProvider)
                        _mySqlProvider = (IMySqlProvider)provider;
                }

                IsEnabled = (_redisProvider != null && _mySqlProvider != null);
            }
            catch (Exception e)
            {
                _logger.Error("Error initializing hybrid storage; {0:l}", e.Message);
            }
        }
    }
}
