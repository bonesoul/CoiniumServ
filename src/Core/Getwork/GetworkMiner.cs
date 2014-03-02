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
using Coinium.Common.Extensions;
using Coinium.Core.Mining;
using Coinium.Net.Http;
using Coinium.Net.Sockets;
using Mono.Net;
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
            Log.Verbose("RPC-client recv:\n{0}", e.Data);

            var rpcResultHandler = new AsyncCallback(
                callback =>
                {
                    var asyncData = ((JsonRpcStateAsync)callback);
                    var result = asyncData.Result + "\n"; // quick hack.
                    var data = Encoding.UTF8.GetBytes(result);        
                    var response = ((HttpListenerResponse)asyncData.AsyncState);

                    response.ContentLength64 = result.Length;
                    response.OutputStream.Write(data, 0, data.Length);
                    response.OutputStream.Flush();
                });

            var line = e.Data;
            line = line.Replace("\n", ""); // quick hack!

            var async = new JsonRpcStateAsync(rpcResultHandler, e.Response) { JsonRpc = line };
            JsonRpcProcessor.Process(async, this);
        }

        public bool Authenticate(string user, string password)
        {
            this.Authenticated = true;
            return this.Authenticated;
        }
    }
}
