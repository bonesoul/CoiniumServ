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
using System.Linq;
using System.Linq.Expressions;
using CoiniumServ.Pools;
using Newtonsoft.Json;

namespace CoiniumServ.Algorithms
{
    public class HashAlgorithmBase : IHashAlgorithm
    {
        public string Name { get; private set; }

        public virtual uint Multiplier { get; protected set; }

        private readonly IList<IPool> _storage;

        public HashAlgorithmBase()
        {
            Name = this.GetType().Name.ToLower(); // set the algorithm name.
            _storage = new List<IPool>(); // initialize the pool storage.            
        }
        
        public virtual byte[] Hash(byte[] input, dynamic config)
        {
            throw new NotImplementedException();
        }

        public void AssignPools(IEnumerable<IPool> pools)
        {
            foreach (var pool in pools)
            {
                _storage.Add(pool);
            }
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

        public int Count { get { return _storage.Count; } }

        public IEnumerator<IPool> GetEnumerator()
        {
            return _storage.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int MinerCount { get; private set; }
        public ulong Hashrate { get; private set; }

        public string ServiceResponse { get; private set; }

        public void Recache()
        {
            MinerCount = 0;
            Hashrate = 0;

            foreach (var pool in _storage)
            {
                MinerCount += pool.MinerManager.Count;
                Hashrate += pool.Hashrate;
            }

            // cache the json-service response
            ServiceResponse = JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        }
    }
}
