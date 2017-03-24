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
using System.Net;
using System.Net.Sockets;
using CoiniumServ.Banning;
using CoiniumServ.Jobs.Manager;
using CoiniumServ.Mining;
using CoiniumServ.Pools;
using CoiniumServ.Server.Mining.Stratum.Sockets;
using Serilog;

// stratum server uses json-rpc 2.0 (over raw sockets) & json-rpc.net (http://jsonrpc2.codeplex.com/)
// classic server handles getwork & getblocktemplate miners over http.

namespace CoiniumServ.Server.Mining.Stratum
{
    /// <summary>
    /// Stratum protocol server implementation.
    /// </summary>
    public class StratumServer : SocketServer, IMiningServer
    {
        public IServerConfig Config { get; private set; }
        
        private readonly IPool _pool;

        private readonly IMinerManager _minerManager;

        private readonly IJobManager _jobManager;

        private readonly IBanManager _banManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="StratumServer"/> class.
        /// </summary>
        /// <param name="pool"></param>
        /// <param name="minerManager">The miner manager.</param>
        /// <param name="jobManager"></param>
        /// <param name="banManager"></param>
        /// <param name="poolConfig"></param>
        public StratumServer(IPoolConfig poolConfig, IPool pool, IMinerManager minerManager, IJobManager jobManager, IBanManager banManager)
        {
            _pool = pool;
            _minerManager = minerManager;
            _jobManager = jobManager;
            _banManager = banManager;
            _logger = Log.ForContext<StratumServer>().ForContext("Component", poolConfig.Coin.Name);
        }

        /// <summary>
        /// Initializes the specified pool.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public void Initialize(IServerConfig config)
        {
            Config = config;
            BindInterface = config.BindInterface;
            Port = config.Port;

            ClientConnected += OnClientConnection;
            ClientDisconnected += OnClientDisconnect;
            BannedConnection += OnBannedConnection;
            DataReceived += OnDataReceived;
        }

        /// <summary>
        /// Starts the server.
        /// </summary>
        /// <returns></returns>
        public override bool Start()
        {
            var success = Listen(BindInterface, Port);
            _logger.Information("Stratum server listening on {0:l}:{1}", BindInterface, Port);
            return success;
        }

        /// <summary>
        /// Stops the server.
        /// </summary>
        /// <returns></returns>
        public override bool Stop()
        {
            throw new NotImplementedException();
        }

        public override bool IsBanned(Socket socket)
        {
            if (socket == null) // we should have a valid socket data.
                return true; // else just behave the client as banned.

            var endpoint = (IPEndPoint) socket.RemoteEndPoint; // get the remote endpoint for socket.
            
            if (endpoint == null || endpoint.Address == null) // if we don't have an endpoint information, basically we can't determine the ip miner
                return false; // in case, we just allow him to get connected as we can ban him later based on his behaviours.

            return _banManager.IsBanned(endpoint.Address);
        }

        /// <summary>
        /// Client on connectin handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClientConnection(object sender, ConnectionEventArgs e)
        {
            _logger.Debug("Stratum client connected: {0}", e.Connection.ToString());

            // TODO: remove the jobManager dependency by instead injecting extranonce counter.
            var miner = _minerManager.Create<StratumMiner>(_jobManager.ExtraNonce.Next(), e.Connection, _pool);
            e.Connection.Client = miner;           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClientDisconnect(object sender, ConnectionEventArgs e)
        {
            _logger.Debug("Stratum client disconnected: {0}", e.Connection.ToString());

            _minerManager.Remove(e.Connection);
        }

        private void OnBannedConnection(object sender, BannedConnectionEventArgs e)
        {
            _logger.Debug("Rejected connection from banned ip: {0:l}", e.Endpoint.Address.ToString());
        }

        /// <summary>
        /// Client data recieve handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDataReceived(object sender, ConnectionDataEventArgs e)
        {
            if (e.Connection == null)
                return;

            var connection = (Connection) e.Connection;
            if (connection.Client == null)
                return;

            ((StratumMiner) connection.Client).Parse(e);
        }
    }
}
