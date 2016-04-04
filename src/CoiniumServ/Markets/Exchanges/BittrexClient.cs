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
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CSharp.RuntimeBinder;
using Serilog;

namespace CoiniumServ.Markets.Exchanges
{
    public class BittrexClient: ExchangeApi, IBittrexClient
    {
        private const string ApiBase = "https://bittrex.com/api/v1.1/";

        private readonly ILogger _logger;

        public BittrexClient()
        {
            _logger = Log.ForContext<BittrexClient>();
        }

        public async Task<IList<IMarketData>> GetMarkets()
        {
            var list = new List<IMarketData>();
            var data = await Request(ApiBase, "public/getmarketsummaries");

            try
            {
                foreach (var market in data.@result)
                {
                    try
                    {
                        string name = market.MarketName;
                        var temp = name.Split('-');

                        var entry = new MarketData
                        {
                            Exchange = Exchange.Bittrex,
                            MarketCurrency = temp.Last().ToUpperInvariant(),
                            BaseCurrency = temp.First().ToUpperInvariant(),
                            Ask = market.Ask,
                            Bid = market.Bid,
                            VolumeInMarketCurrency = market.Volume,
                            VolumeInBaseCurrency = market.BaseVolume
                        };

                        list.Add(entry);
                    }
                    catch (RuntimeBinderException)
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
