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

using System;
using CoiniumServ.Server.Web.Service;
using Newtonsoft.Json;

namespace CoiniumServ.Pools
{
    [JsonObject(MemberSerialization.OptIn)]
    public interface INetworkInfo: IJsonService
    {
        /// <summary>
        /// Network difficulty
        /// </summary>
        [JsonProperty("difficulty")]
        double Difficulty { get; }

        /// <summary>
        /// Current round.
        /// </summary>
        [JsonProperty("round")]
        int Round { get; }

        /// <summary>
        /// Network hashrate
        /// </summary>
        [JsonProperty("hashrate")]
        UInt64 Hashrate { get; }
        
        /// <summary>
        /// Reward for next block in coins.
        /// </summary>
        [JsonProperty("reward")]
        UInt64 Reward { get; }

        /// <summary>
        /// Coin version.
        /// </summary>
        [JsonProperty("version")]
        string CoinVersion { get; }

        /// <summary>
        /// Protocol version
        /// </summary>
        [JsonProperty("protocol")]
        int ProtocolVersion { get; }

        /// <summary>
        /// Wallet version
        /// </summary>
        [JsonProperty("wallet")]
        int WalletVersion { get; }

        /// <summary>
        /// Is testnet?
        /// </summary>
        [JsonProperty("testnet")]
        bool Testnet { get; }

        /// <summary>
        /// Count of connected peers.
        /// </summary>
        [JsonProperty("connections")]
        long Connections { get; }

        /// <summary>
        /// Any errors reported by coin network
        /// </summary>
        [JsonProperty("errors")]
        string Errors { get; }

        /// <summary>
        /// Is the network connection healthy?
        /// </summary>
        [JsonProperty("healthy")]
        bool Healthy { get; }
    }
}
