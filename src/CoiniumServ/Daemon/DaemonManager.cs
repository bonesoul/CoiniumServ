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
