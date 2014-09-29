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
using System.Runtime.Serialization;
using CoiniumServ.Daemon.Converters;
using Newtonsoft.Json;

namespace CoiniumServ.Daemon.Responses
{
    public class MiningInfo
    {
        public int Blocks { get; set; }

        public int CurrentBockSize { get; set; }

        public int CurrentBlockTx { get; set; }

        [JsonConverter(typeof(DifficultyConverter))]
        public double Difficulty { get; set; }

        public string Errors { get; set; }

        public bool Generate { get; set; }

        public int GenProcLimit { get; set; }

        public int HashesPerSec { get; set; }

        public int PooledTx { get; set; }

        public bool Testnet { get; set; }

        // coins may report network hash in different fields; networkhashps, networkmhps, netmhashps
        // we have a member for each of them and then set the actual NetworkHashPerSec in OnDeserializedMethod;
        // we set NetMHashps, NetworkGhps, NetworkMhps and NetworkHashps as private because we won't to expose them
        // to outer world but only NetworkHashPerSec.

        [JsonProperty]
        private double NetMHashps { get; set; }

        [JsonProperty]
        private double NetworkGhps { get; set; }

        [JsonProperty]
        private double NetworkMhps { get; set; }

        [JsonProperty]
        private UInt64 NetworkHashps { get; set; }

        [JsonIgnore]
        public UInt64 NetworkHashPerSec { get; set; }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            NetworkHashPerSec = 0;

            if (NetMHashps > 0)
                NetworkHashPerSec = (UInt64)(NetMHashps*1000*1000);
            else if (NetworkGhps > 0)
                NetworkHashPerSec = (UInt64)(NetworkGhps * 1000 * 1000 * 1000);
            else if (NetworkMhps > 0)
                NetworkHashPerSec = (UInt64)(NetworkMhps * 1000 * 1000);
            else if (NetworkHashps > 0)
                NetworkHashPerSec = NetworkHashps;
        }
    }
}
