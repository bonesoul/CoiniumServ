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
using System.Dynamic;
using System.Linq;
using CoiniumServ.Pools;
using Newtonsoft.Json;

namespace CoiniumServ.Statistics
{
    public class Pools:IPools
    {
        public string Json { get; private set; }

        private readonly Dictionary<string, IPerPool> _pools;
        private readonly IPoolManager _poolManager;
        private readonly Dictionary<string, ExpandoObject> _response;

        public Pools(IPoolManager poolManager)
        {
            _poolManager = poolManager;
            _pools = new Dictionary<string, IPerPool>();
            _response = new Dictionary<string, ExpandoObject>();

            foreach (var pool in poolManager.Pools)
            {
                _pools.Add(pool.Config.Coin.Name, pool.Statistics);
            }
        }

        public IPerPool GetBySymbol(string symbol)
        {
            return _pools.Values.FirstOrDefault(pair => pair.Config.Coin.Symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerator<KeyValuePair<string, IPerPool>> GetEnumerator()
        {
            return _pools.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Recache(object state)
        {
            // recache data.
            foreach (var pool in _poolManager.Pools)
            {
                pool.Statistics.Recache(state);
            }

            // recache response.
            _response.Clear();

            foreach (var pool in _poolManager.Pools)
            {
                _response.Add(pool.Config.Coin.Symbol.ToLower(), (ExpandoObject)pool.Statistics.GetResponseObject());
            }

            Json = JsonConvert.SerializeObject(_response);
        }

        public object GetResponseObject()
        {
            return _response;
        }
    }
}
