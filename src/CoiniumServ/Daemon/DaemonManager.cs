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
using Serilog;

namespace CoiniumServ.Daemon
{
    public class DaemonManager :IDaemonManager
    {
        private readonly IList<IDaemonClient> _storage;
        private readonly IPoolManager _poolManager;

        private readonly ILogger _logger;

        public DaemonManager(IPoolManager poolManager)
        {
            _logger = Log.ForContext<PoolManager>();
            _storage = new List<IDaemonClient>(); // initialize the daemon storage.
            _poolManager = poolManager;

            ReadPoolDaemons();
        }

        private void ReadPoolDaemons()
        {
            // loop through enabled pools and get their daemons.
            foreach (var pool in _poolManager)
            {
                _storage.Add(pool.Daemon);
            }
        }

        public IEnumerator<IDaemonClient> GetEnumerator()
        {
            return _storage.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IQueryable<IDaemonClient> SearchFor(Expression<Func<IDaemonClient, bool>> predicate)
        {
            return _storage.AsQueryable().Where(predicate);
        }

        public IEnumerable<IDaemonClient> GetAll()
        {
            return _storage;
        }

        public IQueryable<IDaemonClient> GetAllAsQueryable()
        {
            return _storage.AsQueryable();
        }

        public IReadOnlyCollection<IDaemonClient> GetAllAsReadOnly()
        {
            return new ReadOnlyCollection<IDaemonClient>(_storage);
        }

        public int Count { get; private set; }
    }
}
