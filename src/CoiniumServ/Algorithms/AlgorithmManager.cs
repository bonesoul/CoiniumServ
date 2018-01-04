#region License
// 
//     MIT License
//
//     CoiniumServ - Crypto Currency Mining Pool Server Software
//     Copyright (C) 2013 - 2017, CoiniumServ Project
//     Hüseyin Uslu, shalafiraistlin at gmail dot com
//     https://github.com/bonesoul/CoiniumServ
// 
//     Permission is hereby granted, free of charge, to any person obtaining a copy
//     of this software and associated documentation files (the "Software"), to deal
//     in the Software without restriction, including without limitation the rights
//     to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//     copies of the Software, and to permit persons to whom the Software is
//     furnished to do so, subject to the following conditions:
//     
//     The above copyright notice and this permission notice shall be included in all
//     copies or substantial portions of the Software.
//     
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//     FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//     LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//     OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//     SOFTWARE.
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
