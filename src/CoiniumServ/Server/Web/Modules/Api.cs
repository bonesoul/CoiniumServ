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
using System.Collections.Generic;
using System.Dynamic;
using Coinium.Mining.Pools;
using Coinium.Mining.Pools.Statistics;
using Nancy;
using Newtonsoft.Json;

namespace Coinium.Server.Web.Modules
{
    public class ApiModule: NancyModule
    {
        public ApiModule(IPoolManager poolManager, IStatistics statistics)
        {
            Get["/api"] = _ =>
            {
                return "Not implemented yet";
            };

            Get["/api/global"] = _ =>
            {
                var algorithms = new Dictionary<string, ExpandoObject>();
                
                //foreach (var algo in poolManager.Statistics.Algorithms)
                //{
                //    dynamic @obj = new ExpandoObject();
                //    algorithms.Add(algo.Name, @obj);
                //    @obj.hashrate = algo.Hashrate;
                //    @obj.workers = algo.Workers;
                //}                

                //var globalStats = new
                //{
                //    hashrate = poolManager.Statistics.Hashrate,
                //    workers = poolManager.Statistics.Workers,
                //    algorithms = algorithms,
                //};

                //return Response.AsJson(globalStats);

                return null;
            };

            Get["/api/pools"] = _ =>
            {
                return Response.AsJson(statistics);
            };
        }
    }
}
