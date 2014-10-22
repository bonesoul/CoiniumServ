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
