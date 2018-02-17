#region License
// 
//     MIT License
//
//     CoiniumServ - Crypto Currency Mining Pool Server Software
//     Copyright (C) 2013 - 2017, CoiniumServ Project
//     Hüseyin Uslu, shalafiraistlin at gmail dot com
//     https://github.com/bonesoul/CoiniumServ
// 
//     Permission is hereby granted, free of charge, to any person obtaining a copy
//     of this software and associated documentation files (the "Software"), to deal
//     in the Software without restriction, including without limitation the rights
//     to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//     copies of the Software, and to permit persons to whom the Software is
//     furnished to do so, subject to the following conditions:
//     
//     The above copyright notice and this permission notice shall be included in all
//     copies or substantial portions of the Software.
//     
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//     FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//     LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//     OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//     SOFTWARE.
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
        double Hashrate { get; }
        
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
