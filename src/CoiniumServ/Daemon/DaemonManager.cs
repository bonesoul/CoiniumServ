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
using CoiniumServ.Pools;
using Serilog;

namespace CoiniumServ.Daemon
{
    public class DaemonManager :IDaemonManager
    {
        private readonly IPoolManager _poolManager;
        private readonly IConfigManager _configManager;
        private readonly IObjectFactory _objectFactory;

        private readonly IDictionary<string, IDaemonClient> _storage;        

        private readonly ILogger _logger;

        public DaemonManager(IPoolManager poolManager, IConfigManager configManager, IObjectFactory objectFactory)
        {
            // TODO: let all daemon's initialzied by daemon-manager.

            _logger = Log.ForContext<DaemonManager>();
            
            _poolManager = poolManager;
            _configManager = configManager;
            _objectFactory = objectFactory;

            _storage = new Dictionary<string, IDaemonClient>(); // initialize the daemon storage.

            ReadPoolDaemons(); // read pool daemons.
        }

        private void ReadPoolDaemons()
        {
            // loop through enabled pools and get their daemons.
            foreach (var pool in _poolManager)
            {
                _storage.Add(pool.Config.Coin.Symbol, pool.Daemon);
            }
        }

        public IEnumerator<IDaemonClient> GetEnumerator()
        {
            return _storage.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IQueryable<IDaemonClient> SearchFor(Expression<Func<IDaemonClient, bool>> predicate)
        {
            return _storage.Values.AsQueryable().Where(predicate);
        }

        public IEnumerable<IDaemonClient> GetAll()
        {
            return _storage.Values;
        }

        public IQueryable<IDaemonClient> GetAllAsQueryable()
        {
            return _storage.Values.AsQueryable();
        }

        public IReadOnlyCollection<IDaemonClient> GetAllAsReadOnly()
        {
            return new ReadOnlyCollection<IDaemonClient>(_storage.Values.ToList());
        }

        public int Count { get; private set; }
    }
}
