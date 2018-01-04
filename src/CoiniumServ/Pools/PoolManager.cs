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
using CoiniumServ.Configuration;
using CoiniumServ.Container;
using Newtonsoft.Json;
using Serilog;

namespace CoiniumServ.Pools
{
    public class PoolManager : IPoolManager
    {
        public int Count { get { return _storage.Count; } }

        public string ServiceResponse { get; private set; }

        private readonly IList<IPool> _storage; 

        private readonly ILogger _logger;

        public PoolManager(IObjectFactory objectFactory , IConfigManager configManager)
        {
            _logger = Log.ForContext<PoolManager>();
            _storage = new List<IPool>(); // initialize the pool storage.

            // loop through all enabled pool configurations.
            foreach (var config in configManager.PoolConfigs)
            {
                var pool = objectFactory.GetPool(config); // create pool for the given configuration.
                
                if(pool.Config.Enabled) // make sure pool was succesfully initialized.
                    _storage.Add(pool); // add it to storage.
            }

            Run(); // run the initialized pools.
        }

        private void Run()
        {
            // run the initialized pools
            foreach (var pool in _storage)
            {
                // var t = new Thread(pool.Initialize);
                // t.Start();
                pool.Initialize();
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

        public void Recache()
        {
            try
            {
                foreach (var pool in _storage) // recache per-pool stats
                {
                    pool.Recache();
                }

                // cache the json-service response
                var cache = _storage.ToDictionary(pool => pool.Config.Coin.Symbol.ToLower());
                ServiceResponse = JsonConvert.SerializeObject(cache, Formatting.Indented, new JsonSerializerSettings {ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
            }
            catch (Exception e)
            {
                _logger.Error("Error recaching statistics; {0:l}", e.Message);
            }
        }

        public IPool Get(string symbol)
        {
            return _storage.FirstOrDefault(p => p.Config.Coin.Symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerator<IPool> GetEnumerator()
        {
            return _storage.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
