#region License
// 
//     CoiniumServ - Crypto Currency Mining Pool Server Software
//     Copyright (C) 2013 - 2014, CoiniumServ Project - http://www.coinium.org
//     https://github.com/CoiniumServ/CoiniumServ
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
using Coinium.Mining.Miners;
using Coinium.Mining.Pools;
using Coinium.Server.Stratum.Notifications;
using Coinium.Services.Rpc.Http;
using Coinium.Utils.Extensions;
using Serilog;

namespace Coinium.Server.Vanilla
{
    public class VanillaMiner : IMiner
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
        /// Is the miner subscribed?
        /// </summary>
        public bool Subscribed { get; private set; }

        /// <summary>
        /// Is the miner authenticated?
        /// </summary>
        public bool Authenticated { get; private set; }

        public IPool Pool { get; private set; }

        /// <summary>
        /// Can we send new mining job's to miner?
        /// </summary>
        public bool SupportsJobNotifications { get; private set; }

        private readonly IMinerManager _minerManager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="minerManager"></param>
        public VanillaMiner(int id, IMinerManager minerManager)
        {
            Id = id; // the id of the miner.
            _minerManager = minerManager;

            Subscribed = true; // vanilla miners are subscribed by default.
            Authenticated = false; // miner has to authenticate.
            SupportsJobNotifications = false; // vanilla miner's doesn't support new mining job notifications.
        }

        public bool Authenticate(string user, string password)
        {
            Username = user;

            Authenticated = _minerManager.Authenticate(this);

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
                });

            using (var reader = new StreamReader(httpRequest.InputStream, Encoding.UTF8))
            {
                var line = reader.ReadToEnd();

                Log.ForContext<VanillaMiner>().Verbose(line.PrettifyJson());

                var rpcRequest = new HttpServiceRequest(line, httpContext);
                var rpcContext = new HttpServiceContext(this, rpcRequest);

                var async = new JsonRpcStateAsync(rpcResultHandler, rpcContext) { JsonRpc = line };
                JsonRpcProcessor.Process(async, rpcContext);
            }        
        }

        /// <summary>
        /// Sends difficulty to the miner.
        /// </summary>
        /// <param name="difficulty"></param>
        public void SendDifficulty(Difficulty difficulty)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Sends a new mining job to the miner.
        /// </summary>
        public void SendJob(Job job)
        {
            throw new NotSupportedException();
        }
    }
}
