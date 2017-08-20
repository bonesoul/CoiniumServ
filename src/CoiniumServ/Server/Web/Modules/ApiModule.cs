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
