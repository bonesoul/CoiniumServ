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
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using CoiniumServ.Markets.Exchanges;
using CoiniumServ.Utils.Extensions;
using Serilog;

namespace CoiniumServ.Markets
{
    public class MarketManager : IMarketManager
    {
        private readonly Timer _timer;

        private readonly ILogger _logger;

        private readonly IList<IExchangeClient> _exchanges;

        private HashSet<IMarketData> _storage; 

        public MarketManager(IBittrexClient bittrexClient, IPoloniexClient poloniexClient, ICryptsyClient cryptsyClient)
        {
            _logger = Log.ForContext<MarketManager>();
            _storage = new HashSet<IMarketData>();
           
            // init the exchanges.
            _exchanges = new List<IExchangeClient>
            {
                bittrexClient,
                cryptsyClient,
                poloniexClient
            };

            // update the data initially
            _timer = new Timer(Run, null, Timeout.Infinite, Timeout.Infinite); // create the timer as disabled.
            Run(null);

            var ltc = GetMarketsFor("LTC", "BTC");
        }

        private void Run(object state)
        {
            UpdateMarkets();    
            _timer.Change(5 * 1000, Timeout.Infinite); // reset the timer.
        }

        private void UpdateMarkets()
        {
            var results = new HashSet<IMarketData>();

            var tasks = _exchanges.Select(exchange => exchange.GetMarkets()).ToList(); // tasks to update.

            foreach (var task in tasks)
            {
                foreach (var entry in task.Result)
                {
                    results.Add(entry);
                }
            }

            _storage = results;
        }

        public IEnumerable<IMarketData> GetMarketsFor(string marketCurrency, string baseCurrency)
        {
            return _storage.Where(x => x.MarketCurrency == marketCurrency && x.BaseCurrency == baseCurrency);
        }

        public IMarketData GetBestMarketFor(string marketCurrency, string baseCurrency)
        {
            try
            {
                return
                    _storage.Where(x => x.MarketCurrency == marketCurrency && x.BaseCurrency == baseCurrency)
                        .MaxBy(x => x.Bid);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public IQueryable<IMarketData> SearchFor(Expression<Func<IMarketData, bool>> predicate)
        {
            return _storage.AsQueryable().Where(predicate);
        }

        public IEnumerable<IMarketData> GetAll()
        {
            return _storage;
        }

        public IQueryable<IMarketData> GetAllAsQueryable()
        {
            return _storage.AsQueryable();
        }

        public IReadOnlyCollection<IMarketData> GetAllAsReadOnly()
        {
            throw new NotImplementedException();
        }

        public IEnumerator<IMarketData> GetEnumerator()
        {
            return _storage.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count { get { return _storage.Count; } }
    }
}
