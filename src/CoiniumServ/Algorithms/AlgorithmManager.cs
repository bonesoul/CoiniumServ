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
using CoiniumServ.Pools;
using CoiniumServ.Utils.Numerics;
using Newtonsoft.Json;

namespace CoiniumServ.Algorithms
{
    public class AlgorithmManager:IAlgorithmManager
    {
        public const string Blake = "blake";
        public const string Fresh = "fresh";
        public const string Fugue = "fugue";
        public const string Groestl = "groestl";
        public const string Keccak = "keccak";
        public const string Nist5 = "nist5";
        public const string Qubit = "qubit";
        public const string Scrypt = "scrypt";
        public const string ScryptOg = "scrypt-og";
        public const string Sha1 = "sha1";
        public const string Sha256 = "sha256";
        public const string Shavite3 = "shavite3";
        public const string Skein = "skein";
        public const string X11 = "x11";
        public const string X13 = "x13";
        public const string X14 = "x14";
        public const string X15 = "x15";
        public const string X17 = "x17";

        // todo: add hefty1, qubit support

        /// <summary>
        /// Global diff1
        /// </summary>
        public static BigInteger Diff1 { get; private set; }

        static AlgorithmManager()
        {
            Diff1 = BigInteger.Parse("00000000ffff0000000000000000000000000000000000000000000000000000", NumberStyles.HexNumber);                                    
        }

        private readonly IList<IHashAlgorithm> _storage; 

        public AlgorithmManager(IPoolManager poolManager)
        {
            _storage = new List<IHashAlgorithm>();

            // add algorithms
            foreach (var pool in poolManager.GetAll())
            {
                if (_storage.Contains(pool.HashAlgorithm))
                    continue;
                
                _storage.Add(pool.HashAlgorithm);
            }

            // assign pools to hash algorithms
            foreach (var item in _storage)
            {
                var algorithm = item;
                var pools = poolManager.GetAll().Where(p => p.Config.Coin.Algorithm == algorithm.GetType().Name.ToLower());
                algorithm.AssignPools(pools);
            }
        }

        public IQueryable<IHashAlgorithm> SearchFor(Expression<Func<IHashAlgorithm, bool>> predicate)
        {
            return _storage.AsQueryable().Where(predicate);
        }

        public IEnumerable<IHashAlgorithm> GetAll()
        {
            return _storage;
        }

        public IQueryable<IHashAlgorithm> GetAllAsQueryable()
        {
            return _storage.AsQueryable();
        }

        public IReadOnlyCollection<IHashAlgorithm> GetAllAsReadOnly()
        {
            return new ReadOnlyCollection<IHashAlgorithm>(_storage);
        }

        public int Count { get { return _storage.Count; }}

        public string ServiceResponse { get; private set; }

        public void Recache()
        {
            foreach (var algorithm in _storage)
            {
                if (algorithm.Count == 0)
                    continue;

                algorithm.Recache();
            }

            // cache the json-service response
            var cache = _storage.ToDictionary(algo => algo.GetType().Name.ToLower());
            ServiceResponse = JsonConvert.SerializeObject(cache, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        }

        public IHashAlgorithm Get(string name)
        {
            return _storage.FirstOrDefault(p => p.GetType().Name.ToLower().Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerator<IHashAlgorithm> GetEnumerator()
        {
            return _storage.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
