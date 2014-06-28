#region License
// 
//     CoiniumServ - Crypto Currency Mining Pool Server Software
//     Copyright (C) 2013 - 2014, CoiniumServ Project - http://www.coinium.org
//     https://github.com/CoiniumServ/CoiniumServ
// 
//     This software is dual-licensed: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
//    
//     For the terms of this license, see licenses/gpl_v3.txt.
// 
//     Alternatively, you can license this software under a commercial
//     license or white-label it as set out in licenses/commercial.txt.
// 
#endregion

using Coinium.Mining.Pools;
using Coinium.Server.Stratum.Notifications;

namespace Coinium.Mining.Miners
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

        IPool Pool { get; }

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
