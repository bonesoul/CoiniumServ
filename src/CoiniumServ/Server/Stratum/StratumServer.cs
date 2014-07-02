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
using Coinium.Mining.Jobs.Manager;
using Coinium.Mining.Miners;
using Coinium.Mining.Pools;
using Coinium.Net.Server.Sockets;
using Coinium.Server.Config;
using Serilog;

// stratum server uses json-rpc 2.0 (over raw sockets) & json-rpc.net (http://jsonrpc2.codeplex.com/)
// classic server handles getwork & getblocktemplate miners over http.

namespace Coinium.Server.Stratum
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

        /// <summary>
        /// Initializes a new instance of the <see cref="StratumServer"/> class.
        /// </summary>
        /// <param name="pool"></param>
        /// <param name="minerManager">The miner manager.</param>
        /// <param name="jobManager"></param>
        public StratumServer(IPool pool, IMinerManager minerManager, IJobManager jobManager)
        {
            _pool = pool;
            _minerManager = minerManager;
            _jobManager = jobManager;
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

            ClientConnected += StratumServer_ClientConnected;
            ClientDisconnected += StratumServer_ClientDisconnected;
            DataReceived += StratumServer_DataReceived;
        }

        /// <summary>
        /// Starts the server.
        /// </summary>
        /// <returns></returns>
        public override bool Start()
        {
            var success = Listen(BindIP, Port);
            return success;
        }

        /// <summary>
        /// Stops the server.
        /// </summary>
        /// <returns></returns>
        public override bool Stop()
        {
            return true;
        }

        /// <summary>
        /// Client on connectin handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StratumServer_ClientConnected(object sender, ConnectionEventArgs e)
        {
            Log.ForContext<StratumServer>().Information("Stratum client connected: {0}", e.Connection.ToString());

            // TODO: remove the jobManager dependency by instead injecting extranonce counter.
            var miner = _minerManager.Create<StratumMiner>(_jobManager.ExtraNonce.NextExtraNonce(), e.Connection, _pool);
            e.Connection.Client = miner;           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StratumServer_ClientDisconnected(object sender, ConnectionEventArgs e)
        {
            Log.ForContext<StratumServer>().Information("Stratum client disconnected: {0}", e.Connection.ToString());

            _minerManager.Remove(e.Connection);
        }

        /// <summary>
        /// Client data recieve handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StratumServer_DataReceived(object sender, ConnectionDataEventArgs e)
        {
            var connection = (Connection)e.Connection;
            ((StratumMiner)connection.Client).Parse(e);
        }        
    }
}
