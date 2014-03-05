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

// classic server uses json-rpc 1.0 (over http) & jayrock.

using System.Net;
using Coinium.Net.Http;
using Serilog;

namespace Coinium.Core.ClassicJayrock
{
    public class ClassicJayrockServer : HttpServer
    {
        public ClassicJayrockServer(int port) 
            : base(port)
        {
            Log.Verbose("Classic server listening on port {0}.", this.Port);
            this.ProcessRequest += ProcessHttpRequest;
        }

        private void ProcessHttpRequest(HttpListenerContext context)
        {
            var miner = new ClassicJayrockMiner();
            miner.Parse(context);
        }
    }
}
