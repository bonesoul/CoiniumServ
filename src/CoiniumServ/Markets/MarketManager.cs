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
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using CoiniumServ.Configuration;
using CoiniumServ.Markets.Exchanges;
using CoiniumServ.Utils.Extensions;
using Serilog;

namespace CoiniumServ.Markets
{
    public class MarketManager : IMarketManager
    {
        public event EventHandler Update;

        private readonly Stopwatch _stopWatch = new Stopwatch();

        private readonly Timer _timer;

        private readonly IList<IExchangeClient> _exchanges;

        private HashSet<IMarketData> _storage;

        private readonly IMarketsConfig _config;

        private readonly ILogger _logger;

        public int Count { get { return _storage.Count; } }

        public MarketManager(IConfigManager configManager, IBittrexClient bittrexClient, IPoloniexClient poloniexClient, ICryptsyClient cryptsyClient)
        {
            _logger = Log.ForContext<MarketManager>();
            _storage = new HashSet<IMarketData>();
            _config = configManager.MarketsConfig;
           
            // init the exchanges.
            _exchanges = new List<IExchangeClient>
            {
                bittrexClient,
                cryptsyClient,
                poloniexClient
            };

            // update the data initially
            _timer = new Timer(Run, null, 1, Timeout.Infinite); // schedule the timer for the first run.
        }

        private void Run(object state)
        {
            _stopWatch.Start();

            UpdateMarkets();

            _logger.Debug("Recached market data - took {0:0.000} seconds", (float)_stopWatch.ElapsedMilliseconds / 1000);
            _stopWatch.Reset();

            _timer.Change(_config.UpdateInterval * 1000, Timeout.Infinite); // reset the timer.
        }

        private void UpdateMarkets()
        {
            var results = new HashSet<IMarketData>();

            var tasks = _exchanges.Select(exchange => exchange.GetMarkets()).ToList(); // tasks to update.

            foreach (var task in tasks)
            {
                if (task.Result == null)
                    continue;

                foreach (var entry in task.Result)
                {
                    results.Add(entry);
                }
            }

            _storage = results;

            OnUpdate(EventArgs.Empty); // notify the listeners about the data update.
        }

        private void OnUpdate(EventArgs e)
        {
            var handler = Update;

            if (handler != null)
                handler(this, e);
        }

        public IEnumerable<IMarketData> GetMarketsFor(string marketCurrency, string baseCurrency)
        {
            return _storage.Where(x => x.MarketCurrency == marketCurrency && x.BaseCurrency == baseCurrency).OrderByDescending(x => x.Bid);
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
    }
}
