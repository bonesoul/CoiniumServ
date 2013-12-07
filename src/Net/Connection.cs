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
using System.Net;
using System.Net.Sockets;

namespace coinium.Net
{
    public class Connection : IConnection
    {
        public bool IsConnected { get; private set; }
        public IPEndPoint RemoteEndPoint { get; private set; }
        public IPEndPoint LocalEndPoint { get; private set; }
        public IClient Client { get; set; }
        public Socket Socket { get; private set; }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        public int Receive(int start, int count)
        {
            throw new NotImplementedException();
        }

        public int Send(byte[] buffer)
        {
            throw new NotImplementedException();
        }

        public int Send(byte[] buffer, SocketFlags flags)
        {
            throw new NotImplementedException();
        }

        public int Send(byte[] buffer, int start, int count)
        {
            throw new NotImplementedException();
        }

        public int Send(byte[] buffer, int start, int count, SocketFlags flags)
        {
            throw new NotImplementedException();
        }

        public int Send(IEnumerable<byte> data)
        {
            throw new NotImplementedException();
        }

        public int Send(IEnumerable<byte> data, SocketFlags flags)
        {
            throw new NotImplementedException();
        }
    }
}
