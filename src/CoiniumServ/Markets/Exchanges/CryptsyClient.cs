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
using Newtonsoft.Json.Converters;
using Serilog;

namespace CoiniumServ.Markets.Exchanges
{
    public class CryptsyClient : ExchangeApi, ICryptsyClient
    {
        private const string PublicApiBase = "http://pubapi.cryptsy.com/";        
        private const string PublicApiEndpoint = "api.php";
        private const string PrivateApiBase = " https://api.cryptsy.com/";
        private const string PrivateApiEndpoint = "api";

        private readonly ILogger _logger;

        public CryptsyClient()
        {
            _logger = Log.ForContext<CryptsyClient>();
        }

        public async Task<IList<IMarketData>> GetMarkets()
        {
            var list = new List<IMarketData>();

            var @params = new Dictionary<string, string>
            {
                {"method", "marketdatav2"}
            };

            var data = await Request(PublicApiBase, PublicApiEndpoint, @params);

            try
            {
                foreach (var kvp in data.@return.markets)
                {
                    try
                    {
                        var buyOrders = (IList<dynamic>) kvp.Value.buyorders;
                        var sellOrders = (IList<dynamic>) kvp.Value.sellorders;

                        var entry = new MarketData
                        {
                            Exchange = Exchange.Cryptsy,
                            MarketCurrency = kvp.Value.primarycode.ToUpperInvariant(),
                            BaseCurrency = kvp.Value.secondarycode.ToUpperInvariant(),
                            Ask = double.Parse(sellOrders.First().price, CultureInfo.InvariantCulture),
                            Bid = double.Parse(buyOrders.First().price, CultureInfo.InvariantCulture),
                            VolumeInMarketCurrency = double.Parse(kvp.Value.volume, CultureInfo.InvariantCulture),
                        };
                        list.Add(entry);
                    }
                    catch (ArgumentNullException)
                    { } // just skip the exception that occurs when a field can not be read.
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
