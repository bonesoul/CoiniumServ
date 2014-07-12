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
using System.Threading;

namespace Coinium.Mining.Pools.Statistics
{
    public class GlobalStatistics:IGlobalStatistics
    {
        public Int32 Workers { get; private set; }
        public UInt64 Hashrate { get; private set; }
        public IList<IPerAlgorithmStatistics> Algorithms { get; private set; }

        private readonly IDictionary<string, IPerAlgorithmStatistics> _algorithms;

        private readonly IPoolManager _poolManager;

        private readonly Timer _timer;
        private const int TimerExpiration = 10;

        public GlobalStatistics(IPoolManager poolManager)
        {
            _poolManager = poolManager;

            _algorithms = new Dictionary<string, IPerAlgorithmStatistics>();

            _timer = new Timer(Recache, null, Timeout.Infinite, Timeout.Infinite); // create the timer as disabled.
            Recache(null); // recache data initially.
        }

        private void Recache(object state)
        {
            Hashrate = 0;
            Workers = 0;

            foreach (var pair in _algorithms)
            {
                pair.Value.Reset();
            }

            foreach (var pool in _poolManager.GetPools())
            {
                Hashrate += pool.Statistics.Hashrate;
                Workers += pool.Statistics.Miners.Count;

                if (!_algorithms.ContainsKey(pool.Config.Coin.Algorithm))
                    _algorithms.Add(pool.Config.Coin.Algorithm, new PerAlgorithmStatistics(pool.Config.Coin.Algorithm));

                _algorithms[pool.Config.Coin.Algorithm].Hashrate += pool.Statistics.Hashrate;
                _algorithms[pool.Config.Coin.Algorithm].Workers += pool.Statistics.Miners.Count;
            }

            Algorithms = _algorithms.Values.ToList();

            // reset the recache timer.
            _timer.Change(TimerExpiration * 1000, Timeout.Infinite);
        }
    }
}
