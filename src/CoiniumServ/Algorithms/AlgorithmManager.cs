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
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using CoiniumServ.Container;
using CoiniumServ.Pools;
using CoiniumServ.Utils.Numerics;
using Newtonsoft.Json;

namespace CoiniumServ.Algorithms
{
    public class AlgorithmManager:IAlgorithmManager
    {
        /// <summary>
        /// Global diff1
        /// </summary>
        public static BigInteger Diff1 { get; private set; }

        private readonly IList<IHashAlgorithmStatistics> _storage;

        public int Count { get { return _storage.Count; } }

        public string ServiceResponse { get; private set; }

        public AlgorithmManager(IPoolManager poolManager, IObjectFactory objectFactory)
        {
            _storage = new List<IHashAlgorithmStatistics>();

            // add algorithms
            foreach (var pool in poolManager.GetAll())
            {
                var query = _storage.FirstOrDefault(x => x.Name == pool.Config.Coin.Algorithm);

                if (query != null)
                    continue;

                var statistics = objectFactory.GetHashAlgorithmStatistics(pool.Config.Coin.Algorithm);

                _storage.Add(statistics);
            }

            // assign pools to hash algorithms
            foreach (var item in _storage)
            {
                var algorithm = item;
                var pools = poolManager.GetAll().Where(p => p.Config.Coin.Algorithm == algorithm.Name);
                algorithm.AssignPools(pools);
            }
        }

        static AlgorithmManager()
        {
            Diff1 = BigInteger.Parse("00000000ffff0000000000000000000000000000000000000000000000000000", NumberStyles.HexNumber);
        }

        public IQueryable<IHashAlgorithmStatistics> SearchFor(Expression<Func<IHashAlgorithmStatistics, bool>> predicate)
        {
            return _storage.AsQueryable().Where(predicate);
        }

        public IEnumerable<IHashAlgorithmStatistics> GetAll()
        {
            return _storage;
        }

        public IQueryable<IHashAlgorithmStatistics> GetAllAsQueryable()
        {
            return _storage.AsQueryable();
        }

        public IReadOnlyCollection<IHashAlgorithmStatistics> GetAllAsReadOnly()
        {
            return new ReadOnlyCollection<IHashAlgorithmStatistics>(_storage);
        }

        public void Recache()
        {
            foreach (var algorithm in _storage)
            {
                if (algorithm.Count == 0)
                    continue;

                algorithm.Recache();
            }

            // cache the json-service response
            var cache = _storage.ToDictionary(algo => algo.Name);
            ServiceResponse = JsonConvert.SerializeObject(cache, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        }

        public IHashAlgorithmStatistics Get(string name)
        {
            return _storage.FirstOrDefault(p => p.Name.ToLower().Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerator<IHashAlgorithmStatistics> GetEnumerator()
        {
            return _storage.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
