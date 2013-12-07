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
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using AustinHarris.JsonRpc;
using coinium.Net.RPC.Server.Responses;
using Serilog;

namespace coinium.Net.RPC.Server
{
    public class RPCServer
    {
        public RPCServer()
        { }

        private static object[] _services =
        {
            new MiningService()
        };

        public void Start()
        {
            var thread = new Thread(new ThreadStart(ServerThread));
            thread.Start();
        }

        private static void ServerThread()
        {
            var server = new TcpListener(IPAddress.Parse("127.0.0.1"), 3333);
            server.Start();

            while (true)
            {
                try
                {
                    using (var client = server.AcceptTcpClient())
                    {
                        var rpcResultHandler = new AsyncCallback(
                            callback =>
                            {
                                var async = ((JsonRpcStateAsync)callback);
                                var result = async.Result;
                                var writer = ((StreamWriter)async.AsyncState);

                                writer.WriteLine(result);
                                writer.Flush();
                                Log.Debug("RESPONSE: {Response}", result);
                            });

                        using (var stream = client.GetStream())
                        {
                            var reader = new StreamReader(stream, Encoding.UTF8);
                            var writer = new StreamWriter(stream, new UTF8Encoding(false));

                            while (!reader.EndOfStream)
                            {
                                var line = reader.ReadLine();
                                
                                var async = new JsonRpcStateAsync(rpcResultHandler, writer) { JsonRpc = line };
                                JsonRpcProcessor.Process(async, writer);

                                Log.Debug("REQUEST: {Request}", line);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e, "RPCServer exception");
                }
            }
        }
    }

    public class MiningService : JsonRpcService
    {
        [JsonRpcMethod("mining.subscribe")]
        public SubscribeResponse SubscribeMiner(string miner)
        {
            var response = new SubscribeResponse
            {
                ExtraNonce1 = "08000002",
                ExtraNonce2Size = 4
            };

            return response;
        }

        [JsonRpcMethod("mining.authorize")]
        public bool AuthorizeMiner(string user, string password)
        {
            return true;
        }
    }
}