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

using System.Linq;
using CoiniumServ.Coin.Config;
using CoiniumServ.Server.Web.Modules.Models;
using CoiniumServ.Statistics;
using Nancy;
using Newtonsoft.Json;

namespace CoiniumServ.Server.Web.Modules
{
    public class ApiModule: NancyModule
    {
        public ApiModule(IStatistics statistics)
        {
            Get["/api/"] = _ => View["api", new ApiModel
            {
                BaseUrl = Request.Url.SiteBase,
                Coin = statistics.Pools.First().Value.Config.Coin
            }];

            Get["/api/global"] = _ => Response.AsJson(statistics.Global.GetResponseObject());
            
            Get["/api/pools"] = _ => Response.AsJson(statistics.Pools.GetResponseObject());

            Get["/api/pool/{slug}"] = _ =>
            {
                var pool = statistics.Pools.GetBySymbol(_.slug);

                Response response;

                if (pool == null)
                    response = JsonConvert.SerializeObject(new JsonError("Pool not found!"));
                else
                    response = (Response)pool.Json;

                response.ContentType = "application/json";
                return response; 
            };

            Get["/api/algorithms"] = _ => Response.AsJson(statistics.Algorithms.GetResponseObject());

            Get["/api/algorithm/{slug}"] = _ =>
            {
                var algorithm = statistics.Algorithms.GetByName(_.slug);

                Response response;

                if (algorithm == null)
                    response = JsonConvert.SerializeObject(new JsonError("Algorithm not found!"));
                else
                    response = (Response)algorithm.Json;                                      
                    
                response.ContentType = "application/json";
                return response;                
            };
        }
    }

    public class ApiModel
    {
        public string BaseUrl { get; set; }
        public ICoinConfig Coin { get; set; }
    }
}
