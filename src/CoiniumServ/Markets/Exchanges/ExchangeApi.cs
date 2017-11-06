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
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RestSharp;
using Serilog;

namespace CoiniumServ.Markets.Exchanges
{
    public class ExchangeApi
    {
        private readonly ExpandoObjectConverter _converter = new ExpandoObjectConverter();

        private readonly ILogger _logger;

        public ExchangeApi()
        {
            _logger = Log.ForContext<ExchangeApi>();
        }

        protected async Task<dynamic> Request(string @base, string endpoint, IEnumerable<KeyValuePair<string, string>> @params = null, bool userless = true, Method httpMethod = Method.GET)
        {
            try
            {
                var client = new RestClient(@base);
                var request = new RestRequest(endpoint, httpMethod);
                var cancellationTokenSource = new CancellationTokenSource();

                if (@params != null)
                {
                    foreach (var param in @params)
                    {
                        request.AddParameter(param.Key, param.Value);
                    }
                }

                var response = await client.ExecuteTaskAsync(request, cancellationTokenSource.Token);

                if (response.ResponseStatus == ResponseStatus.Completed)
                {
                    dynamic @object = JsonConvert.DeserializeObject<ExpandoObject>(response.Content, _converter);
                    return @object;
                }
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
            }

            return null;
        }
    }
}
