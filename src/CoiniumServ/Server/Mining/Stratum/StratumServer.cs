#region License
// 
//     CoiniumServ - Crypto Currency Mining Pool Server Software
//     Copyright (C) 2013 - 2014, CoiniumServ Project - http://www.coinium.org
//     http://www.coiniumserv.com - https://github.com/CoiniumServ/CoiniumServ
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
using System.Net;
using System.Net.Sockets;
using CoiniumServ.Banning;
using CoiniumServ.Coin.Config;
using CoiniumServ.Jobs.Manager;
using CoiniumServ.Miners;
using CoiniumServ.Networking.Server.Sockets;
using CoiniumServ.Pools;
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

        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="StratumServer"/> class.
        /// </summary>
        /// <param name="pool"></param>
        /// <param name="minerManager">The miner manager.</param>
        /// <param name="jobManager"></param>
        /// <param name="banManager"></param>
        /// <param name="coinConfig"></param>
        public StratumServer(IPool pool, IMinerManager minerManager, IJobManager jobManager, IBanManager banManager, ICoinConfig coinConfig)
        {
            _pool = pool;
            _minerManager = minerManager;
            _jobManager = jobManager;
            _banManager = banManager;
            _logger = Log.ForContext<StratumServer>().ForContext("Component", coinConfig.Name);
        }

        /// <summary>
        /// Initializes the specified pool.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public void Initialize(IServerConfig config)
        {
            Config = config;
            BindIP = config.BindInterface;
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
            var success = Listen(BindIP, Port);
            _logger.Information("Stratum server listening on {0:l}:{1}", BindIP, Port);
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
            _logger.Information("Stratum client connected: {0}", e.Connection.ToString());

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
            _logger.Information("Stratum client disconnected: {0}", e.Connection.ToString());

            _minerManager.Remove(e.Connection);
        }

        private void OnBannedConnection(object sender, BannedConnectionEventArgs e)
        {
            _logger.Information("Rejected connection from banned ip: {0:l}", e.Endpoint.Address.ToString());
        }

        /// <summary>
        /// Client data recieve handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDataReceived(object sender, ConnectionDataEventArgs e)
        {
            var connection = (Connection)e.Connection;
            ((StratumMiner)connection.Client).Parse(e);
        }
    }
}
