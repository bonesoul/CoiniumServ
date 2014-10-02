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
using CoiniumServ.Jobs;
using CoiniumServ.Mining;
using CoiniumServ.Vardiff;
using Newtonsoft.Json;

namespace CoiniumServ.Server.Mining.Stratum
{
    [JsonObject(MemberSerialization.OptIn)]
    public interface IStratumMiner:IMiner, IVardiffMiner
    {
        /// <summary>
        /// Hex-encoded, per-connection unique string which will be used for coinbase serialization later. (http://mining.bitcoin.cz/stratum-mining)
        /// </summary>
        UInt32 ExtraNonce { get; }

        /// <summary>
        /// Is the miner subscribed?
        /// </summary>
        bool Subscribed { get; }

        [JsonProperty("difficulty")]
        float Difficulty { get; }

        float PreviousDifficulty { get; }

        /// <summary>
        /// Sends message of the day to miner.
        /// </summary>
        void SendMessage(string message);

        /// <summary>
        /// Sets a new difficulty to the miner and sends it.
        /// </summary>
        void SetDifficulty(float difficulty);

        /// <summary>
        /// Sends a new mining job to the miner.
        /// </summary>
        void SendJob(IJob job);

        void Subscribe(string signature);
    }
}
