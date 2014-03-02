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
using System.Text;
using AustinHarris.JsonRpc;
using Coinium.Common.Extensions;
using Coinium.Core.Mining;
using Coinium.Net;
using Serilog;

namespace Coinium.Core.Services.Getwork
{
    public class GetworkMiner : IClient, IMiner
    {
        public IConnection Connection { get; private set; }

        public bool Authenticated { get; private set; }

        public GetworkMiner(IConnection connection)
        {
            this.Connection = connection;
            this.Authenticated = false;
        }

        public void Parse(ConnectionDataEventArgs e)
        {
            Log.Verbose("RPC-client recv:\n{0}", e.Data.Dump());

            var rpcResultHandler = new AsyncCallback(
                callback =>
                {
                    var asyncData = ((JsonRpcStateAsync)callback);
                    var result = asyncData.Result + "\n"; // quick hack.
                    var client = ((GetworkMiner)asyncData.AsyncState);

                    var data = Encoding.UTF8.GetBytes(result);
                    client.Connection.Send(data);

                    Log.Verbose("RPC-client send:\n{0}", data.Dump());
                });

            var line = e.Data.ToEncodedString();
            line = line.Replace("\n", ""); // quick hack!

            var async = new JsonRpcStateAsync(rpcResultHandler, this) { JsonRpc = line };
            JsonRpcProcessor.Process(async, this);
        }

        public bool Authenticate(string user, string password)
        {
            this.Authenticated = true;
            return this.Authenticated;
        }
    }
}
