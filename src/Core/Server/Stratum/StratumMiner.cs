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
using System.Collections.Generic;
using System.Text;
using AustinHarris.JsonRpc;
using Coinium.Common.Extensions;
using Coinium.Core.Mining;
using Coinium.Core.Server.Stratum.Notifications;
using Coinium.Net.RPC.Sockets;
using Coinium.Net.Sockets;
using Newtonsoft.Json;
using Serilog;

namespace Coinium.Core.Server.Stratum
{
    public class StratumMiner : IClient, IMiner
    {
        /// <summary>
        /// Miner's connection.
        /// </summary>
        public IConnection Connection { get; private set; }

        /// <summary>
        /// Unique subscription id for identifying the miner.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Is the miner subscribed?
        /// </summary>
        public bool Subscribed { get; private set; }

        /// <summary>
        /// Is the miner authenticated.
        /// </summary>
        public bool Authenticated { get; private set; }

        /// <summary>
        /// Creates a new miner instance.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="connection"></param>
        public StratumMiner(int id, IConnection connection)
        {
            this.Id = id; // the id of the miner.
            this.Connection = connection; // the underlying connection.

            this.Subscribed = false;
            this.Authenticated = false;
        }

        /// <summary>
        /// Parses the incoming data.
        /// </summary>
        /// <param name="e"></param>
        public void Parse(ConnectionDataEventArgs e)
        {
            Log.Verbose("RPC-client recv:\n{0}", e.Data.ToEncodedString());

            var rpcResultHandler = new AsyncCallback(
                callback =>
                {
                    var asyncData = ((JsonRpcStateAsync)callback);
                    var result = asyncData.Result + "\n"; // quick hack.
                    var data = Encoding.UTF8.GetBytes(result);

                    var context = (SocketsRpcContext) asyncData.AsyncState;

                    var miner = (StratumMiner)context.Miner;                                         
                                
                    miner.Connection.Send(data);

                    Log.Verbose("RPC-client send:\n{0}", result);

                    // send an initial job after miner subsribes.
                    if (miner.Authenticated && miner.Subscribed)
                        this.SendJob();
                });

            var line = e.Data.ToEncodedString();
            line = line.Replace("\n", ""); // quick hack!

            var rpcRequest = new SocketsRpcRequest(line);
            var rpcContext = new SocketsRpcContext(this, rpcRequest);

            var async = new JsonRpcStateAsync(rpcResultHandler, rpcContext) { JsonRpc = line };
            JsonRpcProcessor.Process(async, rpcContext);
        }

        /// <summary>
        /// Subscribes the miner to mining service.
        /// </summary>
        public void Subscribe()
        {
            this.Subscribed = true;
        }

        /// <summary>
        /// Authenticates the miner.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool Authenticate(string user, string password)
        {
            this.Authenticated = true;
            return this.Authenticated;
        }

        /// <summary>
        /// Sends a mining-job to miner.
        /// </summary>
        private void SendJob()
        {
            var notification = new JsonRequest
            {
                Id = null,
                Method = "mining.notify",
                Params = new JobNotification()
                {
                    Id = "2b50",
                    PreviousBlockHash = "8f0472e229d7f803642efac67a3daeabbe9bda7ac7872ea19e3978cf3e65261d",
                    CoinbaseInitial = "01000000010000000000000000000000000000000000000000000000000000000000000000ffffffff2703c33a07062f503253482f047028a25208",
                    CoinbaseFinal = "0d2f7374726174756d506f6f6c2f0000000001fe7aaa2a010000001976a9145b771921a9b47ee8104da7e4710b5f633d95fa7388ac00000000",
                    MerkleBranches = new List<object>
                    {
                        "795ec58b7ebf9522ad15b76207c8f40e569cfd478c2e2c30bfa06c8a6d24609d",
                        "e76544a1b9d7550280c49bf965d5882c65f8cd4b9711202cc9bd311f76b438ac",
                        "01acd5a2a3142d735e4847a5b512aef35f22f67eb2d7be1f4dcdfca3c4031a43",
                        "4c561433d42406ca777fb1aa730a6b96317ca68c2341b5f0f52afc2ae1b6235d"
                    },
                    BlockVersion = "00000002",
                    NetworkDifficulty = "1b1d88f6",
                    nTime = "52a22871",
                    CleanJobs = false
                }
            };

            var json = JsonConvert.SerializeObject(notification) + "\n";

            var data = Encoding.UTF8.GetBytes(json);
            this.Connection.Send(data);

            Log.Verbose("RPC-client send:\n{0}", data.ToEncodedString());
        }
    }
}
