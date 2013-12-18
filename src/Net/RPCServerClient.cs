/*
 *   Coinium project - crypto currency pool software - https://github.com/raistlinthewiz/coinium
 *   Copyright (C) 2013 Hüseyin Uslu, Int6 Studios - http://www.coinium.org
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Serilog;
using coinium.Common.Extensions;

namespace coinium.Net
{
    class RPCServerClient:IClient
    {
        public IConnection Connection { get; set; }

        public RPCServerClient(IConnection connection)
        {
            this.Connection = connection;
        }

        public void Parse(ConnectionDataEventArgs e)
        {
            Log.Verbose("RPC-client recv:\n{0}", e.Data.Dump());
        }
    }
}
