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

// classic server uses json-rpc 1.0 (over http) & json-rpc.net (http://jsonrpc2.codeplex.com/)

using System.Net;
using Coinium.Miner;
using Coinium.Net.Server.Http;
using Coinium.Server.Config;

namespace Coinium.Server.Vanilla
{
    public class VanillaServer : HttpServer, IMiningServer
    {
        private readonly IMinerManager _minerManager;

        public VanillaServer(IMinerManager minerManager)
        {
            _minerManager = minerManager;
        }

        public void Initialize(IServerConfig serverConfig)
        {
            Initialize(serverConfig.Port);
            Config = serverConfig;
            ProcessRequest += ProcessHttpRequest;
        }

        public IServerConfig Config { get; private set; }

        private void ProcessHttpRequest(HttpListenerContext context)
        {
            var miner = _minerManager.Create<VanillaMiner>();
            miner.Parse(context);                        
        }
    }
}
