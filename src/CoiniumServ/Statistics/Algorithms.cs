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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CoiniumServ.Pools;
using Newtonsoft.Json;

namespace CoiniumServ.Statistics
{
    public class Algorithms : IAlgorithms
    {
        public string Json { get; private set; }

        private readonly Dictionary<string, IPerAlgorithm> _algorithms;
        private readonly Dictionary<string, object> _response;
        private readonly IPoolManager _poolManager;

        public Algorithms(IPoolManager poolManager)
        {
            _poolManager = poolManager;
            _algorithms = new Dictionary<string, IPerAlgorithm>();
            _response = new Dictionary<string, object>();
        }

        public void Recache(object state)
        {
            // recache data.
            foreach (var pair in _algorithms)
            {
                pair.Value.Reset();
            }

            foreach (var pool in _poolManager.Pools)
            {
                if (!_algorithms.ContainsKey(pool.Config.Coin.Algorithm))
                    _algorithms.Add(pool.Config.Coin.Algorithm, new PerAlgorithm(pool.Config.Coin.Algorithm));

                _algorithms[pool.Config.Coin.Algorithm].Recache(pool.Statistics.Hashrate, pool.Statistics.WorkerCount);                
            }

            // recache response.
            _response.Clear();

            foreach (var pair in _algorithms)
            {
                var algorithm = pair.Value;
                _response.Add(algorithm.Name, algorithm.GetResponseObject());
            }

            Json = JsonConvert.SerializeObject(_response);
        }

        public IPerAlgorithm GetByName(string name)
        {
            return _algorithms.Values.FirstOrDefault(pair => pair.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerator<KeyValuePair<string, IPerAlgorithm>> GetEnumerator()
        {
            return _algorithms.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public object GetResponseObject()
        {
            return _response;
        }
    }
}
