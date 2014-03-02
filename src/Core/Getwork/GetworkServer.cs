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

using System;
using System.IO;
using System.Text;
using AustinHarris.JsonRpc;
using Coinium.Common.Extensions;
using Coinium.Net.Http;
using Serilog;

namespace Coinium.Core.Getwork
{
    /// <summary>
    /// Getwork protocol server implementation.
    /// </summary>
    public class GetworkServer : HttpServer
    {
        private static object[] _services =
        {
            new GetworkService()
        };

        public GetworkServer(int port) 
            : base(port)
        {
            this.OnHttpRequest += Getwork_DataReceived;
        }

        private void Getwork_DataReceived(object sender, HttpRequestEventArgs e)
        {
            var miner = new GetworkMiner();
            miner.Parse(e);            
        }
    }
}
