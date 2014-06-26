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

using System;
using Coinium.Server.Stratum.Notifications;

namespace Coinium.Miner
{
    /// <summary>
    /// Miner interface that any implementations should extend.
    /// </summary>
    public interface IMiner
    {
        /// <summary>
        /// Unique subscription id for identifying the miner.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Username of the miner.
        /// </summary>
        string Username { get; }

        /// <summary>
        /// Is the miner subscribed?
        /// </summary>
        bool Subscribed { get; }

        /// <summary>
        /// Is the miner authenticated.
        /// </summary>
        bool Authenticated { get; }

        /// <summary>
        /// Authenticates the miner.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        bool Authenticate(string user, string password);

        /// <summary>
        /// Can we send new mining job's to miner?
        /// </summary>
        bool SupportsJobNotifications { get; }

        /// <summary>
        /// Sends difficulty to the miner.
        /// </summary>
        /// <param name="difficulty"></param>
        void SendDifficulty(Difficulty difficulty);

        /// <summary>
        /// Sends a new mining job to the miner.
        /// </summary>
        void SendJob(Job job);
    }
}
