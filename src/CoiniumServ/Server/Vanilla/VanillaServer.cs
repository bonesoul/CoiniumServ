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
using Coinium.Mining.Jobs.Manager;
using Coinium.Mining.Miners;
using Coinium.Mining.Pools;
using Coinium.Net.Server.Http.Basic;
using Coinium.Server.Config;

// classic server uses json-rpc 1.0 (over http) & json-rpc.net (http://jsonrpc2.codeplex.com/)

namespace Coinium.Server.Vanilla
{
    public class VanillaServer : HttpServer, IMiningServer
    {
        public IServerConfig Config { get; private set; }

        private readonly IMinerManager _minerManager;

        private readonly IJobManager _jobManager;

        private readonly IPool _pool;

        public VanillaServer(IPool pool, IMinerManager minerManager, IJobManager jobManager)
        {
            _pool = pool;
            _minerManager = minerManager;
            _jobManager = jobManager;
        }

        public void Initialize(IServerConfig serverConfig)
        {
            Initialize(serverConfig.Port);
            Config = serverConfig;
            ProcessRequest += ProcessHttpRequest;
        }

        private void ProcessHttpRequest(HttpListenerContext context)
        {
            var miner = _minerManager.Create<VanillaMiner>(_pool);
            miner.Parse(context);                        
        }
    }
}
