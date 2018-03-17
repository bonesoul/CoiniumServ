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
using System.Linq;
using System.Linq.Expressions;
using CoiniumServ.Pools;
using Newtonsoft.Json;

namespace CoiniumServ.Algorithms
{
    public class HashAlgorithmStatistics:IHashAlgorithmStatistics
    {
        public string Name { get; private set; }

        public Int32 MinerCount { get; private set; }

        public double Hashrate { get; private set; }

        public int Count { get { return _storage.Count; } }

        public string ServiceResponse { get; private set; }

        private readonly IList<IPool> _storage;

        public HashAlgorithmStatistics(string name)
        {
            Name = name;
            _storage = new List<IPool>(); // initialize the pool storage.   
        }

        public IEnumerator<IPool> GetEnumerator()
        {
            return _storage.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IQueryable<IPool> SearchFor(Expression<Func<IPool, bool>> predicate)
        {
            return _storage.AsQueryable().Where(predicate);
        }

        public IEnumerable<IPool> GetAll()
        {
            return _storage;
        }

        public IQueryable<IPool> GetAllAsQueryable()
        {
            return _storage.AsQueryable();
        }

        public IReadOnlyCollection<IPool> GetAllAsReadOnly()
        {
            return new ReadOnlyCollection<IPool>(_storage);
        }

        public void Recache()
        {
            MinerCount = 0;
            Hashrate = 0;

            foreach (var pool in _storage)
            {
                if (!pool.Initialized)
                    continue;
                
                MinerCount += pool.MinerManager.Count;
                Hashrate += pool.Hashrate;
            }

            // cache the json-service response
            ServiceResponse = JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        }

        public void AssignPools(IEnumerable<IPool> pools)
        {
            foreach (var pool in pools)
            {
                _storage.Add(pool);
            }
        }
    }
}
