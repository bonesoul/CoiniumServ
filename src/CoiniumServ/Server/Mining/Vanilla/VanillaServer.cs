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

using System.Net;

// classic server uses json-rpc 1.0 (over http) & json-rpc.net (http://jsonrpc2.codeplex.com/)
using CoiniumServ.Coin.Config;
using CoiniumServ.Jobs.Manager;
using CoiniumServ.Miners;
using CoiniumServ.Networking.Server.Http.Basic;
using CoiniumServ.Pools;
using Serilog;

namespace CoiniumServ.Server.Mining.Vanilla
{
    public class VanillaServer : HttpServer, IMiningServer
    {
        public IServerConfig Config { get; private set; }

        private readonly IMinerManager _minerManager;

        private readonly IJobManager _jobManager;

        private readonly IPool _pool;

        private readonly ILogger _logger;

        public VanillaServer(IPool pool, IMinerManager minerManager, IJobManager jobManager, ICoinConfig coinConfig)
        {
            _pool = pool;
            _minerManager = minerManager;
            _jobManager = jobManager;
            _logger = Log.ForContext<VanillaServer>().ForContext("Component", coinConfig.Name);
        }

        public void Initialize(IServerConfig config)
        {
            Config = config;
            BindIP = config.BindInterface;
            Port = config.Port;

            Initialize();
            ProcessRequest += ProcessHttpRequest;

            _logger.Information("Vanilla server listening on {0:l}:{1}", BindIP, Port);
        }

        private void ProcessHttpRequest(HttpListenerContext context)
        {
            var miner = _minerManager.Create<VanillaMiner>(_pool);
            miner.Parse(context);                        
        }
    }
}
