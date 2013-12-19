/*
 *   Coinium project - crypto currency pool software - https://github.com/raistlinthewiz/coinium
 *   Copyright (C) 2013 Hüseyin Uslu, Int6 Studios - http://www.coinium.org
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
using System.Text;
using AustinHarris.JsonRpc;
using Serilog;
using coinium.Common.Extensions;
using coinium.Mining;

namespace coinium.Net.RPC.Server
{
    class RPCClient : IClient, IMiner
    {
        public IConnection Connection { get; set; }

        public RPCClient(IConnection connection)
        {
            this.Connection = connection;
        }

        public void Parse(ConnectionDataEventArgs e)
        {
            Log.Verbose("RPC-client recv:\n{0}", e.Data.Dump());

            var rpcResultHandler = new AsyncCallback(
                callback =>
                {
                    var asyncData = ((JsonRpcStateAsync)callback);
                    var result = asyncData.Result + "\n"; // quick hack.
                    var client = ((RPCClient)asyncData.AsyncState);
                    var data = Encoding.UTF8.GetBytes(result);
                    Log.Verbose("RPC-client send:\n{0}", data.Dump());
                    client.Connection.Send(data);

                });


            var line = e.Data.ToEncodedString();
            line = line.Replace("\n", ""); // quick hack!

            var async = new JsonRpcStateAsync(rpcResultHandler, this) { JsonRpc = line };
            JsonRpcProcessor.Process(async, this);
        }
    }
}
