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
using CoiniumServ.Jobs.Manager;
using CoiniumServ.Mining;
using CoiniumServ.Pools;
using Serilog;

// classic server uses json-rpc 1.0 (over http) & json-rpc.net (http://jsonrpc2.codeplex.com/)

namespace CoiniumServ.Server.Mining.Getwork
{
    public class GetworkServer : HttpServer, IMiningServer
    {
        public IServerConfig Config { get; private set; }

        private readonly IMinerManager _minerManager;

        private readonly IPool _pool;

        private readonly ILogger _logger;

        public GetworkServer(IPoolConfig poolConfig, IPool pool, IMinerManager minerManager, IJobManager jobManager)
        {
            _pool = pool;
            _minerManager = minerManager;
            _logger = Log.ForContext<GetworkServer>().ForContext("Component", poolConfig.Coin.Name);
        }

        public void Initialize(IServerConfig config)
        {
            Config = config;
            BindInterface = config.BindInterface;
            Port = config.Port;

            Initialize();
            ProcessRequest += ProcessHttpRequest;

            _logger.Information("Getwork server listening on {0:l}:{1}", BindInterface, Port);
        }

        private void ProcessHttpRequest(HttpListenerContext context)
        {
            var miner = _minerManager.Create<GetworkMiner>(_pool);
            miner.Parse(context);                        
        }
    }
}
