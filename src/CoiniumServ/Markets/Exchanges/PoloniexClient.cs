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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Serilog;

namespace CoiniumServ.Markets.Exchanges
{
    public class PoloniexClient : ExchangeApi, IPoloniexClient
    {
        private const string ApiBase = "https://poloniex.com/";
        private const string PublicApiEndpoint = "public";
        private const string PrivateApiEndpoint = "tradingApi";

        private readonly ILogger _logger;

        public PoloniexClient()
        {
            _logger = Log.ForContext<PoloniexClient>();
        }

        public async Task<IList<IMarketData>> GetMarkets()
        {
            var list = new List<IMarketData>();

            var @params = new Dictionary<string, string>
            {
                {"command", "returnTicker"}
            };

            var data = await Request(ApiBase, PublicApiEndpoint, @params);

            try
            {
                foreach (var kvp in data)
                {
                    try
                    {
                        string name = kvp.Key;
                        var temp = name.Split('_');

                        var entry = new MarketData
                        {
                            Exchange = Exchange.Poloniex,
                            MarketCurrency = temp.Last().ToUpperInvariant(),
                            BaseCurrency = temp.First().ToUpperInvariant(),
                            Ask = double.Parse(kvp.Value.lowestAsk, CultureInfo.InvariantCulture),
                            Bid = double.Parse(kvp.Value.highestBid, CultureInfo.InvariantCulture),
                            VolumeInMarketCurrency = double.Parse(kvp.Value.quoteVolume, CultureInfo.InvariantCulture),
                            VolumeInBaseCurrency = double.Parse(kvp.Value.baseVolume, CultureInfo.InvariantCulture),
                        };

                        list.Add(entry);
                    }
                    catch (Exception e)
                    {
                        _logger.Error(e.Message);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
            }

            return list;
        }
    }
}
