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

using Coinium.Core.Mining;
using Coinium.Core.Services.Stratum;
using Coinium.Net;
using Serilog;

namespace Coinium.Core.Servers
{
    public class StratumServer:Server
    {
        private static object[] _services =
        {
            new StratumService()
        };

        public StratumServer()
        {
            this.OnConnect += RPCServerNew_OnConnect;
            this.OnDisconnect += RPCServerNew_OnDisconnect;
            this.DataReceived += RPCServerNew_DataReceived;
        }

        void RPCServerNew_OnConnect(object sender, ConnectionEventArgs e)
        {
            Log.Verbose("RPC-client connected: {0}", e.Connection.ToString());
            
            var miner = new Miner(e.Connection);
            e.Connection.Client = miner;
        }

        void RPCServerNew_OnDisconnect(object sender, ConnectionEventArgs e)
        {
            Log.Verbose("RPC-client disconnected: {0}", e.Connection.ToString());
        }

        void RPCServerNew_DataReceived(object sender, ConnectionDataEventArgs e)
        {
            var connection = (Connection)e.Connection;
            ((Miner)connection.Client).Parse(e);
        }
    }
}
