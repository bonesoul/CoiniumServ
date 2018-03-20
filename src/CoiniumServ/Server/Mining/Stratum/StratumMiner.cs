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
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using AustinHarris.JsonRpc;
using CoiniumServ.Accounts;
using CoiniumServ.Jobs;
using CoiniumServ.Logging;
using CoiniumServ.Mining;
using CoiniumServ.Persistance.Layers;
using CoiniumServ.Pools;
using CoiniumServ.Server.Mining.Stratum.Errors;
using CoiniumServ.Server.Mining.Stratum.Sockets;
using CoiniumServ.Utils.Buffers;
using CoiniumServ.Utils.Extensions;
using Newtonsoft.Json;
using Serilog;

namespace CoiniumServ.Server.Mining.Stratum
{
    // TODO: Add tls support!

    [DebuggerDisplay("Id: {Id}, Username: {Username}, Connection: {Connection}, Difficulty: {Difficulty}")]
    public class StratumMiner : IClient, IStratumMiner
    {
        /// <summary>
        /// Miner's connection.
        /// </summary>
        public IConnection Connection { get; private set; }

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
        /// Is the miner subscribed?
        /// </summary>
        public bool Subscribed { get; private set; }

        /// <summary>
        /// Is the miner authenticated?
        /// </summary>
        public bool Authenticated { get; set; }

        public int ValidShareCount { get; set; }

        public int InvalidShareCount { get; set; }

        public IPool Pool { get; private set; }
        
        public float Difficulty { get; set; }

        public float PreviousDifficulty { get; set; }

        /// <summary>
        /// Hex-encoded, per-connection unique string which will be used for coinbase serialization later. (http://mining.bitcoin.cz/stratum-mining)
        /// </summary>
        public uint ExtraNonce { get; private set; }

        public int LastVardiffTimestamp { get; set; }

        public int LastVardiffRetarget { get; set; }

        public IRingBuffer VardiffBuffer { get; set; }

        public MinerSoftware Software { get; private set; }

        public Version SoftwareVersion { get; private set; }

        private readonly AsyncCallback _rpcResultHandler;

        private readonly IMinerManager _minerManager;

        private readonly IStorageLayer _storageLayer;

        private readonly ILogger _logger;

        private readonly ILogger _packetLogger;

        /// <summary>
        /// Creates a new miner instance.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="extraNonce"></param>
        /// <param name="connection"></param>
        /// <param name="pool"></param>
        /// <param name="minerManager"></param>
        /// <param name="storageLayer"></param>
        public StratumMiner(int id, UInt32 extraNonce, IConnection connection, IPool pool, IMinerManager minerManager, IStorageLayer storageLayer)
        {
            Id = id; // the id of the miner.
            ExtraNonce = extraNonce;
            Connection = connection; // the underlying connection.
            Pool = pool;
            _minerManager = minerManager;
            _storageLayer = storageLayer;

            Subscribed = false; // miner has to subscribe.
            Authenticated = false; // miner has to authenticate.

            _logger = Log.ForContext<StratumMiner>().ForContext("Component", pool.Config.Coin.Name);
            _packetLogger = LogManager.PacketLogger.ForContext<StratumMiner>().ForContext("Component", pool.Config.Coin.Name);

            _rpcResultHandler = callback =>
            {
                var asyncData = ((JsonRpcStateAsync)callback); // get the async data.
                var result = asyncData.Result + "\n"; // read the result.
                var response = Encoding.UTF8.GetBytes(result); // set the response.

                Connection.Send(response); // send the response.

                _packetLogger.Verbose("tx: {0}", result.PrettifyJson());
            };
        }

        /// <summary>
        /// Authenticates the miner.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool Authenticate(string user, string password)
        {
            Username = user;
            _minerManager.Authenticate(this);

            if(!Authenticated)
                JsonRpcContext.SetException(new AuthenticationError(Username));

            return Authenticated;
        }

        /// <summary>
        /// Subscribes the miner to mining service.
        /// </summary>
        public void Subscribe(string signature)
        {
            Subscribed = true;

            // identify the miner software.
            try
            {
                var data = signature.Split('/');
                var software = data[0].ToLower();
                var version = data[1];

                switch (software)
                {
                    case "bfgminer":
                        Software = MinerSoftware.BfgMiner;
                        break;
                    case "ccminer":
                        Software = MinerSoftware.CCMiner;
                        break;
                    case "cgminer":
                        Software = MinerSoftware.CGMiner;
                        break;
                    case "cudaminer":
                        Software = MinerSoftware.CudaMiner;
                        break;
                    default:
                        Software = MinerSoftware.Unknown;
                        break;
                }

                SoftwareVersion = new Version(version);
            }
            catch (Exception) // on unknown signature
            {
                Software = MinerSoftware.Unknown;
                SoftwareVersion = new Version();
            }
        }

        /// <summary>
        /// Parses the incoming data.
        /// </summary>
        /// <param name="e"></param>
        public void Parse(ConnectionDataEventArgs e)
        {
            var data = e.Data.ToEncodedString(); // read the data.
            var lines = Regex.Split(data, @"\r?\n|\r"); // get all lines with the recieved data.

            foreach (var line in lines) // loop through all lines
            {
                if (string.IsNullOrWhiteSpace(line)) // if line doesn't contain any data.
                    continue; // just skip it.

                ProcessRequest(line); // process the json-rpc request.
            }
        }

        private void ProcessRequest(string line)
        {
            try
            {
                var rpcContext = new StratumContext(this); // set the context.

                _packetLogger.Verbose("rx: {0}", line.PrettifyJson());

                var async = new JsonRpcStateAsync(_rpcResultHandler, rpcContext) {JsonRpc = line};
                JsonRpcProcessor.Process(Pool.Config.Coin.Name, async, rpcContext);
            }
            catch (JsonReaderException e) // if client sent an invalid message
            {
                _logger.Error("Disconnecting miner {0:l} as we recieved an invalid json-rpc request - {1:l}",
                    Username ?? Connection.RemoteEndPoint.ToString(), e.Message);

                Connection.Disconnect(); // disconnect him.
            }
        }

        /// <summary>
        /// Sends message of the day to miner.
        /// </summary>
        public void SendMessage(string message)
        {
            var notification = new JsonRequest
            {
                Id = null,
                Method = "client.show_message",
                Params = new List<object> { message }
            };

            Send(notification);
        }

        /// <summary>
        /// Sets a new difficulty to the miner and sends it.
        /// </summary>
        public void SetDifficulty(float difficulty)
        {
            if (Difficulty == difficulty) // if new difficulty is the same with current one,
                return; // just skip.

            PreviousDifficulty = Difficulty; // store the previous difficulty (so we can still accept shares targeted for last difficulty when vardiff sets a new difficulty).
            Difficulty = difficulty;

            var notification = new JsonRequest
            {
                Id = null,
                Method = "mining.set_difficulty",
                Params = new List<object>{ Difficulty }
            };

            Send(notification); //send the difficulty update message.
            _storageLayer.UpdateDifficulty(this); // store the new difficulty within persistance layer.
        }

        /// <summary>
        /// Sends a mining-job to miner.
        /// </summary>
        public void SendJob(IJob job)
        {
            var notification = new JsonRequest
            {
                Id = null,
                Method = "mining.notify",
                Params = job
            };

            Send(notification);
        }

        private void Send(JsonRequest request)
        {
            var json = JsonConvert.SerializeObject(request) + "\n";

            var data = Encoding.UTF8.GetBytes(json);
            Connection.Send(data);

            _packetLogger.Verbose("tx: {0}", data.ToEncodedString().PrettifyJson());
        }
    }
}
