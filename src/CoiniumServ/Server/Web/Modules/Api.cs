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
using CoiniumServ.Cryptology.Algorithms;
using CoiniumServ.Pools;
using CoiniumServ.Server.Web.Models;
using CoiniumServ.Statistics.New;
using Nancy;
using Newtonsoft.Json;

namespace CoiniumServ.Server.Web.Modules
{
    public class ApiModule: NancyModule
    {
        private static readonly Response PoolNotFound = JsonConvert.SerializeObject(new JsonError("Pool not found!"));

        // TODO: use base("/api");
        public ApiModule(IStatisticsManager statisticsManager, IPoolManager poolManager, IAlgorithmManager algorithmManager)
        {
            Get["/api/"] = _ =>
            {
                // include common data required by layout.
                ViewBag.Title = "API";
                ViewBag.Heading = "Public API";
                ViewBag.Pools = poolManager;
                ViewBag.LastUpdate = statisticsManager.LastUpdate.ToString("HH:mm:ss tt zz"); // last statistics update.

                // return our view
                return View["api", new ApiModel
                {
                    BaseUrl = Request.Url.SiteBase,
                    Coin = poolManager.First().Config.Coin
                }];
            };


            Get["/api/new/pools"] = _ =>
            {
                var response = (Response) poolManager.ServiceResponse;
                response.ContentType = "application/json";
                return response;
            };

            Get["/api/new/pool/{slug}"] = _ =>
            {
                var pool = poolManager.Get(_.slug); // query the requested pool.

                var response = pool != null ? (Response)pool.ServiceResponse : PoolNotFound;
                response.ContentType = "application/json";
                return response;
            };

            Get["/api/new/algorithms"] = _ =>
            {
                var response = (Response)algorithmManager.ServiceResponse;
                response.ContentType = "application/json";
                return response;
            };

            Get["/api/new/algorithm/{slug}"] = _ =>
            {
                var algorithm = algorithmManager.Get(_.slug); // query the requested pool.

                var response = algorithm != null ? (Response)algorithm.ServiceResponse : PoolNotFound;
                response.ContentType = "application/json";
                return response;
            };

            Get["/api/new/global"] = _ =>
            {
                var response = (Response) statisticsManager.ServiceResponse;
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
