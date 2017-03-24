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
