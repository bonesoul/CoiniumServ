/*
 *   Coinium - Crypto Currency Pool Software - https://github.com/CoiniumServ/coinium
 *   Copyright (C) 2013 - 2014, Coinium Project - http://www.coinium.org
 *
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.IO;
using System.Text;
using AustinHarris.JsonRpc;
using Coinium.Core.Mining;
using Coinium.Core.RPC;
using Coinium.Net.Http;
using Serilog;

namespace Coinium.Core.Getwork
{
    public class GetworkMiner : IMiner
    {
        public bool Authenticated { get; private set; }

        public GetworkMiner()
        {
            this.Authenticated = false;
        }

        public void Parse(HttpRequestEventArgs e)
        {
            var context = e.Context;
            var httpRequest = context.Request;

            var encoding = Encoding.UTF8;

            var rpcResultHandler = new AsyncCallback(
                callback =>
                {
                    var asyncData = ((JsonRpcStateAsync) callback);

                    var result = asyncData.Result;
                    var data = Encoding.UTF8.GetBytes(result);
                    var rpcRequest = ((RpcRequest) asyncData.AsyncState);

                    rpcRequest.Response.ContentType = "application/json";
                    rpcRequest.Response.ContentEncoding = encoding;

                    rpcRequest.Response.ContentLength64 = data.Length;
                    rpcRequest.Response.OutputStream.Write(data,0,data.Length);                    

                    Log.Verbose("RPC-client send:\n{0}", result);
                });

            using (var reader = new StreamReader(httpRequest.InputStream, encoding))
            {
                var line = reader.ReadToEnd();
                var response = context.Response;

                var rpcRequest = new RpcRequest(line, response);

                var async = new JsonRpcStateAsync(rpcResultHandler, rpcRequest) { JsonRpc = line };
                JsonRpcProcessor.Process(async, this);
            }        
        }

        public bool Authenticate(string user, string password)
        {
            this.Authenticated = true;
            return this.Authenticated;
        }
    }
}
