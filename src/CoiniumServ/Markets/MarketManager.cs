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

using System.Collections.Generic;
using CoiniumServ.Pools;
using Serilog;

namespace CoiniumServ.Markets
{
    public class MarketManager : IMarketManager
    {
        private readonly ILogger _logger;

        private IDictionary<string, IDictionary<Exchange, IMarketData>> _markets =
            new Dictionary<string, IDictionary<Exchange, IMarketData>>();

        public MarketManager(IPoolManager poolManager, IBittrexClient bittrexClient)
        {
            _logger = Log.ForContext<MarketManager>();

            var bittrexTask = bittrexClient.GetMarkets();

            foreach (var entry in bittrexTask.Result)
            {
                if (entry.BaseCurrency != "BTC") // ignore non-BTC based markets.
                    continue;

                var name = string.Format("{0}/{1}", entry.BaseCurrency, entry.MarketCurrency);
                
                if (!_markets.ContainsKey(name))
                {
                    var storage = new Dictionary<Exchange, IMarketData>();
                    _markets.Add(name, storage);
                }

                var market = _markets[name];

            }
        }
    }
}
