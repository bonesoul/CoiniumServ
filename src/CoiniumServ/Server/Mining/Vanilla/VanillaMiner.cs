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
using System.IO;
using System.Net;
using System.Text;
using AustinHarris.JsonRpc;
using CoiniumServ.Factories;
using CoiniumServ.Logging;
using CoiniumServ.Miners;
using CoiniumServ.Pools;
using CoiniumServ.Server.Mining.Vanilla.Service;
using CoiniumServ.Utils.Extensions;
using Serilog;

namespace CoiniumServ.Server.Mining.Vanilla
{
    public class VanillaMiner : IVanillaMiner
    {
        /// <summary>
        /// Unique subscription id for identifying the miner.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Username of the miner.
        /// </summary>
        public string Username { get; private set; }

        /// <summary>
        /// Is the miner authenticated?
        /// </summary>
        public bool Authenticated { get; set; }

        public int ValidShares { get; set; }
        public int InvalidShares { get; set; }

        public IPool Pool { get; private set; }

        private readonly IMinerManager _minerManager;

        private readonly ILogger _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pool"></param>
        /// <param name="minerManager"></param>
        /// <param name="logManager"></param>
        public VanillaMiner(int id, IPool pool, IMinerManager minerManager)
        {
            Id = id; // the id of the miner.
            Pool = pool;
            _minerManager = minerManager;

            Authenticated = false; // miner has to authenticate.

            _logger = LogManager.PacketLogger.ForContext<VanillaMiner>().ForContext("Component", pool.Config.Coin.Name);
        }

        public bool Authenticate(string user, string password)
        {
            Username = user;
            _minerManager.Authenticate(this);

            return Authenticated;
        }

        public void Parse(HttpListenerContext httpContext)
        {
            var httpRequest = httpContext.Request;

            var rpcResultHandler = new AsyncCallback(
                callback =>
                {
                    var asyncData = ((JsonRpcStateAsync) callback);
                    var result = asyncData.Result;
                    var response = Encoding.UTF8.GetBytes(result);

                    var context = (HttpServiceContext)asyncData.AsyncState;

                    context.Request.Response.ContentType = "application/json";
                    context.Request.Response.ContentEncoding = Encoding.UTF8;
                    context.Request.Response.ContentLength64 = response.Length;
                    context.Request.Response.OutputStream.Write(response, 0, response.Length);

                    _logger.Verbose("tx: {0}", result.PrettifyJson());
                });

            using (var reader = new StreamReader(httpRequest.InputStream, Encoding.UTF8))
            {
                var line = reader.ReadToEnd();
                _logger.Verbose("rx: {0}", line.PrettifyJson());

                var rpcRequest = new HttpServiceRequest(line, httpContext);
                var rpcContext = new HttpServiceContext(this, rpcRequest);

                var async = new JsonRpcStateAsync(rpcResultHandler, rpcContext) { JsonRpc = line };
                JsonRpcProcessor.Process(Pool.Config.Coin.Name, async, rpcContext);
            }        
        }
    }
}
