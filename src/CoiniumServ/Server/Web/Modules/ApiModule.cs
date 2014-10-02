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
using CoiniumServ.Algorithms;
using CoiniumServ.Pools;
using CoiniumServ.Server.Web.Models;
using CoiniumServ.Statistics;
using Nancy;
using Nancy.Helpers;
using Newtonsoft.Json;

namespace CoiniumServ.Server.Web.Modules
{
    public class ApiModule: NancyModule
    {
        private static readonly Response PoolNotFound = JsonConvert.SerializeObject(new JsonError("Pool not found!"));
        private static readonly Response AlgorithmNotFound = JsonConvert.SerializeObject(new JsonError("Algorithm not found!"));

        public ApiModule(IStatisticsManager statisticsManager, IPoolManager poolManager, IAlgorithmManager algorithmManager)
            :base("/api")
        {
            Get["/"] = _ =>
            {
                // include common data required by layout.
                ViewBag.Header = "Public API";

                // return our view
                return View["api", new ApiModel
                {
                    BaseUrl = Request.Url.SiteBase,
                    Coin = poolManager.First().Config.Coin
                }];
            };


            Get["/pools"] = _ =>
            {
                var response = (Response) poolManager.ServiceResponse;
                response.ContentType = "application/json";
                return response;
            };

            Get["/pool/{slug}"] = _ =>
            {
                var pool = poolManager.Get(HttpUtility.HtmlEncode(_.slug)); // query the requested pool.

                var response = pool != null ? (Response)pool.ServiceResponse : PoolNotFound;
                response.ContentType = "application/json";
                return response;
            };

            Get["/algorithms"] = _ =>
            {
                var response = (Response)algorithmManager.ServiceResponse;
                response.ContentType = "application/json";
                return response;
            };

            Get["/algorithm/{slug}"] = _ =>
            {
                var algorithm = algorithmManager.Get(HttpUtility.HtmlEncode(_.slug)); // query the requested pool.

                var response = algorithm != null ? (Response)algorithm.ServiceResponse : AlgorithmNotFound;
                response.ContentType = "application/json";
                return response;
            };

            Get["/global"] = _ =>
            {
                var response = (Response) statisticsManager.ServiceResponse;
                response.ContentType = "application/json";
                return response;
            };          
        }
    }
}
