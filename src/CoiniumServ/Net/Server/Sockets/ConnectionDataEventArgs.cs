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

using System.Collections.Generic;
using System.Linq;

namespace Coinium.Net.Server.Sockets
{
    public sealed class ConnectionDataEventArgs : ConnectionEventArgs
    {
        public IEnumerable<byte> Data { get; private set; }

        public ConnectionDataEventArgs(IConnection connection, IEnumerable<byte> data)
            : base(connection)
        {
            Data = data ?? new byte[0];
        }

        public override string ToString()
        {
            return Connection.RemoteEndPoint != null
                ? string.Format("{0}: {1} bytes", Connection.RemoteEndPoint, Data.Count())
                : string.Format("Not Connected: {0} bytes", Data.Count());
        }
    }
}
