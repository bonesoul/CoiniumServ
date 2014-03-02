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

using Coinium.Core.Services.Getwork;
using Coinium.Net;
using Coinium.Net.Sockets;
using Serilog;

namespace Coinium.Core.Servers
{
    /// <summary>
    /// Getwork protocol server implementation.
    /// </summary>
    public class GetworkServer : SocketServer
    {
        private static object[] _services =
        {
            new GetworkService()
        };

        public GetworkServer()
        {
            this.OnConnect += Getwork_OnConnect;
            this.OnDisconnect += Getwork_OnDisconnect;
            this.DataReceived += Getwork_DataReceived;
        }

        private void Getwork_OnConnect(object sender, ConnectionEventArgs e)
        {
            Log.Verbose("Stratum client connected: {0}", e.Connection.ToString());
            
            var miner = new GetworkMiner(e.Connection);
            e.Connection.Client = miner;
        }

        private void Getwork_OnDisconnect(object sender, ConnectionEventArgs e)
        {
            Log.Verbose("Stratum client disconnected: {0}", e.Connection.ToString());
        }

        private void Getwork_DataReceived(object sender, ConnectionDataEventArgs e)
        {
            var connection = (Connection)e.Connection;
            ((GetworkMiner)connection.Client).Parse(e);
        }
    }
}
