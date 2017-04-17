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
