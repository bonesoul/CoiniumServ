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
using Serilog;

namespace CoiniumServ.Mining.Software
{
    public class SoftwareRepository:ISoftwareRepository
    {
        private readonly IList<IMiningSoftware> _storage;

        private readonly ILogger _logger;

        public SoftwareRepository(IObjectFactory objectFactory, IConfigManager configManager)
        {
            _logger = Log.ForContext<SoftwareRepository>();
            _storage = new List<IMiningSoftware>(); // initialize the pool storage.

            // loop through all enabled pool configurations.
            foreach (var config in configManager.SoftwareRepositoryConfig)
            {
                var entry = objectFactory.GetMiningSoftware(config); // create pool for the given configuration.
                _storage.Add(entry); // add it to storage.
            }
        }

        public IEnumerator<IMiningSoftware> GetEnumerator()
        {
            return _storage.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IQueryable<IMiningSoftware> SearchFor(Expression<Func<IMiningSoftware, bool>> predicate)
        {
            return _storage.AsQueryable().Where(predicate);
        }

        public IEnumerable<IMiningSoftware> GetAll()
        {
            return _storage;
        }

        public IQueryable<IMiningSoftware> GetAllAsQueryable()
        {
            return _storage.AsQueryable();
        }

        public IReadOnlyCollection<IMiningSoftware> GetAllAsReadOnly()
        {
            return new ReadOnlyCollection<IMiningSoftware>(_storage);
        }

        public int Count { get { return _storage.Count; } }
    }
}
