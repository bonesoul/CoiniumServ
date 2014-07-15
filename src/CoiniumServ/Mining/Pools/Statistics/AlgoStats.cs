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
using System.Collections;
using System.Collections.Generic;

namespace Coinium.Mining.Pools.Statistics
{
    public class AlgoStats : IAlgoStats
    {
        private readonly Dictionary<string, IPerAlgorithmStats> _algorithms;
        private readonly IPoolStats _poolStatistics;

        public AlgoStats(IPoolStats poolStatistics)
        {
            _poolStatistics = poolStatistics;
            _algorithms = new Dictionary<string, IPerAlgorithmStats>();
        }

        public void Recache(object state)
        {
            foreach (var pair in _algorithms)
            {
                pair.Value.Reset();
            }

            foreach (var pair in _poolStatistics)
            {
                if (!_algorithms.ContainsKey(pair.Value.Algorithm))
                    _algorithms.Add(pair.Value.Algorithm, new PerAlgorithmStats(pair.Value.Algorithm));

                _algorithms[pair.Value.Algorithm].Hashrate = pair.Value.Hashrate;
                _algorithms[pair.Value.Algorithm].WorkerCount = pair.Value.WorkerCount;
            }
        }

        public IEnumerator<KeyValuePair<string, IPerAlgorithmStats>> GetEnumerator()
        {
            return _algorithms.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
