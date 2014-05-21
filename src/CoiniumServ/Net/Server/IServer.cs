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

namespace Coinium.Net.Server
{
    /// <summary>
    /// Server interface that any implementations should extend.
    /// </summary>
    public interface IServer
    {
        /// <summary>
        /// The IP address of the interface the server binded.
        /// </summary>
        string BindIP { get; }

        /// <summary>
        /// The listening port for the server.
        /// </summary>
        int Port { get; }

        /// <summary>
        /// Is server currently listening for connections?
        /// </summary>
        bool IsListening { get; }

        /// <summary>
        /// Starts a server instance.
        /// </summary>
        /// <returns></returns>
        bool Start();

        /// <summary>
        /// Stops the server instance.
        /// </summary>
        /// <returns></returns>
        bool Stop();
    }
}
