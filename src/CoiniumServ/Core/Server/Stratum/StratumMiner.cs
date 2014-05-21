/*
 *   CoiniumServ - crypto currency pool software - https://github.com/CoiniumServ/CoiniumServ
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
using Coinium.Core.Mining.Events;
using Coinium.Core.RPC.Sockets;
using Coinium.Core.Server.Stratum.Notifications;
using Coinium.Net.Server.Sockets;
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
        /// Is the miner authenticated?
        /// </summary>
        public bool Authenticated { get; private set; }

                /// <summary>
        /// Sends a new mining job to the miner.
        /// </summary>
        public bool SupportsJobNotifications { get; private set; }

        /// <summary>
        /// Event that fires when a miner authenticates.
        /// </summary>
        public event EventHandler OnAuthenticate;

        /// <summary>
        /// Creates a new miner instance.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="connection"></param>
        public StratumMiner(int id, IConnection connection)
        {
            this.Id = id; // the id of the miner.
            this.Connection = connection; // the underlying connection.
            this.SupportsJobNotifications = true; // stratum miner'ssupports new mining job notifications.

            this.Subscribed = false;
            this.Authenticated = false;
        }

        /// <summary>
        /// Parses the incoming data.
        /// </summary>
        /// <param name="e"></param>
        public void Parse(ConnectionDataEventArgs e)
        {
            Log.Verbose("Stratum recv:\n{0}", e.Data.ToEncodedString());

            var rpcResultHandler = new AsyncCallback(
                callback =>
                {
                    var asyncData = ((JsonRpcStateAsync)callback);
                    var result = asyncData.Result + "\n"; // quick hack.
                    var response = Encoding.UTF8.GetBytes(result);

                    var context = (SocketsRpcContext) asyncData.AsyncState;
                    var miner = (StratumMiner)context.Miner;                                         
                                
                    miner.Connection.Send(response);

                    Log.Verbose("Stratum send:\n{0}", result);
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

            // notify any listeners about the miner's authentication.
            var handler = OnAuthenticate;
            if (handler != null)
                handler(this, new MinerAuthenticationEventArgs(this));

            return this.Authenticated;
        }

        /// <summary>
        /// Sends difficulty to the miner.
        /// </summary>
        /// <param name="difficulty"></param>
        public void SendDifficulty(Difficulty difficulty)
        {
            var notification = new JsonRequest
            {
                Id = null,
                Method = "mining.set_difficulty",
                Params = difficulty
            };

            var json = JsonConvert.SerializeObject(notification) + "\n";

            var data = Encoding.UTF8.GetBytes(json);
            this.Connection.Send(data);

            Log.Verbose("Stratum send:\n{0}", data.ToEncodedString());
        }

        /// <summary>
        /// Sends a mining-job to miner.
        /// </summary>
        public void SendJob(Job job)
        {
            var notification = new JsonRequest
            {
                Id = null,
                Method = "mining.notify",
                Params = job
            };

            var json = JsonConvert.SerializeObject(notification) + "\n";

            var data = Encoding.UTF8.GetBytes(json);
            this.Connection.Send(data);

            Log.Verbose("Stratum send:\n{0}", data.ToEncodedString());
        }
    }
}
