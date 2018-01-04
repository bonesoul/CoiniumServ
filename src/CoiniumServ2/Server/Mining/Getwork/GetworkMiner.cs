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
