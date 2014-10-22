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
using CoiniumServ.Utils.Extensions;
using Serilog;

namespace CoiniumServ.Markets
{
    public class MarketManager : IMarketManager
    {
        private readonly ILogger _logger;

        private readonly HashSet<IMarketData> _storage; 

        public MarketManager(IBittrexClient bittrexClient)
        {
            _logger = Log.ForContext<MarketManager>();
            _storage = new HashSet<IMarketData>();

            var bittrexTask = bittrexClient.GetMarkets();
            
            foreach (var entry in bittrexTask.Result)
            {
                _storage.Add(entry);
            }
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
            catch (Exception e)
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
