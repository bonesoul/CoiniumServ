#region License
// 
//     CoiniumServ - Crypto Currency Mining Pool Server Software
//     Copyright (C) 2013 - 2014, CoiniumServ Project - http://www.coinium.org
//     http://www.coiniumserv.com - https://github.com/CoiniumServ/CoiniumServ
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

using CoiniumServ.Pools;

namespace CoiniumServ.Miners
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
        /// The pool miner is connected to.
        /// </summary>
        IPool Pool { get; }

        /// <summary>
        /// Is the miner authenticated.
        /// </summary>
        bool Authenticated { get; set; }

        int ValidShares { get; set; }

        int InvalidShares { get; set; }

        /// <summary>
        /// Authenticates the miner.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        bool Authenticate(string user, string password);
    }
}
