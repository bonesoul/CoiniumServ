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
using CoiniumServ.Accounts;
using CoiniumServ.Logging;
using CoiniumServ.Mining;
using CoiniumServ.Pools;
using CoiniumServ.Utils.Extensions;
using Newtonsoft.Json;
using Serilog;

namespace CoiniumServ.Server.Mining.Getwork
{
    public class GetworkMiner : IGetworkMiner
    {
        /// <summary>
        /// Unique subscription id for identifying the miner.
        /// </summary>
        public int Id { get; private set; }

        public IAccount Account { get; set; }

        /// <summary>
        /// Username of the miner.
        /// </summary>
        public string Username { get; private set; }

        /// <summary>
        /// Is the miner authenticated?
        /// </summary>
        public bool Authenticated { get; set; }

        public int ValidShareCount { get; set; }
        public int InvalidShareCount { get; set; }

        public IPool Pool { get; private set; }

        public MinerSoftware Software { get; private set; }
        public Version SoftwareVersion { get; private set; }

        private readonly IMinerManager _minerManager;

        private readonly ILogger _logger;

        private readonly ILogger _packetLogger;

        private readonly AsyncCallback _rpcResultHandler;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pool"></param>
        /// <param name="minerManager"></param>
        public GetworkMiner(int id, IPool pool, IMinerManager minerManager)
        {
            Id = id; // the id of the miner.
            Pool = pool;
            _minerManager = minerManager;

            Authenticated = false; // miner has to authenticate.

            Software = MinerSoftware.Unknown;
            SoftwareVersion = new Version();

            _logger = Log.ForContext<GetworkMiner>().ForContext("Component", pool.Config.Coin.Name);
            _packetLogger = LogManager.PacketLogger.ForContext<GetworkMiner>().ForContext("Component", pool.Config.Coin.Name);

            _rpcResultHandler = callback =>
            {
                var asyncData = ((JsonRpcStateAsync) callback);
                var result = asyncData.Result;
                var response = Encoding.UTF8.GetBytes(result);
                var context = (GetworkContext) asyncData.AsyncState;

                context.Response.ContentType = "application/json";
                context.Response.ContentEncoding = Encoding.UTF8;
                context.Response.ContentLength64 = response.Length;
                context.Response.OutputStream.Write(response, 0, response.Length);

                _packetLogger.Verbose("tx: {0}", result.PrettifyJson());
            };
        }

        public bool Authenticate(string user, string password)
        {
            Username = user;
            _minerManager.Authenticate(this);

            return Authenticated;
        }

        public void Parse(HttpListenerContext httpContext)
        {
            try
            {
                var httpRequest = httpContext.Request;

                using (var reader = new StreamReader(httpRequest.InputStream, Encoding.UTF8))
                {
                    var line = reader.ReadToEnd();
                    var rpcContext = new GetworkContext(this, httpContext);

                    _packetLogger.Verbose("rx: {0}", line.PrettifyJson());

                    var async = new JsonRpcStateAsync(_rpcResultHandler, rpcContext) {JsonRpc = line};
                    JsonRpcProcessor.Process(Pool.Config.Coin.Name, async, rpcContext);
                }
            }
            catch (JsonReaderException e) // if client sent an invalid message
            {
                _logger.Error("Skipping invalid json-rpc request - {0:l}", e.Message);
            }
        }
    }
}
