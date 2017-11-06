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
