/*
 *   Coinium - Crypto Currency Pool Software - https://github.com/CoiniumServ/CoiniumServ
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
using System.Net;
using System.Net.Sockets;

namespace Coinium.Net.Sockets
{
    /// <summary>
    /// Connection interface.
    /// </summary>
    public interface IConnection
    {
        /// <summary>
        /// Returns true if there exists an active connection.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Returns remote endpoint.
        /// </summary>
        IPEndPoint RemoteEndPoint { get; }

        /// <summary>
        /// Returns local endpoint.
        /// </summary>
        IPEndPoint LocalEndPoint { get; }

        /// <summary>
        /// Gets or sets bound client.
        /// </summary>
        IClient Client { get; set; }

        /// <summary>
        /// Gets underlying socket.
        /// </summary>
        Socket Socket { get; }

        /// <summary>
        /// Kills the connection to remote endpoint.
        /// </summary>
        void Disconnect();

        // Read bytes from the Sokcet into the buffer in a non-blocking call.
        // This allows us to read no more than the specified count number of bytes.
        int Receive(int start, int count);

        /// <summary>
        /// Sends byte buffer to remote endpoint.
        /// </summary>
        /// <param name="buffer">Byte buffer to send.</param>
        /// <returns>Returns count of sent bytes.</returns>
        int Send(byte[] buffer);

        /// <summary>
        /// Sends byte buffer to remote endpoint.
        /// </summary>
        /// <param name="buffer">Byte buffer to send.</param>
        /// <param name="flags">Sockets flags to use.</param>
        /// <returns>Returns count of sent bytes.</returns>
        int Send(byte[] buffer, SocketFlags flags);

        /// <summary>
        /// Sends byte buffer to remote endpoint.
        /// </summary>
        /// <param name="buffer">Byte buffer to send.</param>
        /// <param name="start">Start index to read from buffer.</param>
        /// <param name="count">Count of bytes to send.</param>
        /// <returns>Returns count of sent bytes.</returns>
        int Send(byte[] buffer, int start, int count);

        /// <summary>
        /// Sends byte buffer to remote endpoint.
        /// </summary>
        /// <param name="buffer">Byte buffer to send.</param>
        /// <param name="start">Start index to read from buffer.</param>
        /// <param name="count">Count of bytes to send.</param>
        /// <param name="flags">Sockets flags to use.</param>
        /// <returns>Returns count of sent bytes.</returns>
        int Send(byte[] buffer, int start, int count, SocketFlags flags);

        /// <summary>
        /// Sends an enumarable byte buffer to remote endpoint.
        /// </summary>
        /// <param name="data">Enumrable byte buffer to send.</param>
        /// <returns>Returns count of sent bytes.</returns>
        int Send(IEnumerable<byte> data);

        /// <summary>
        /// Sends an enumarable byte buffer to remote endpoint.
        /// </summary>
        /// <param name="data">Enumrable byte buffer to send.</param>
        /// <param name="flags">Sockets flags to use.</param>
        /// <returns>Returns count of sent bytes.</returns>
        int Send(IEnumerable<byte> data, SocketFlags flags);
    }
}
