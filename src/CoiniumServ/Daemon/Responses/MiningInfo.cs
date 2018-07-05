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

        public double GenProcLimit { get; set; }

        public double HashesPerSec { get; set; }

        public double PooledTx { get; set; }

        public bool Testnet { get; set; }

        // coins may report network hash in different fields; networkhashps, networkmhps, netmhashps
        // we have a member for each of them and then set the actual NetworkHashPerSec in OnDeserializedMethod;
        // we set NetMHps, NetMHashps, NetworkGhps, NetworkMhps and NetworkHashps as private because we won't to expose them
        // to outer world but only NetworkHashPerSec.

        [JsonProperty("netmh/s")] // added to support unit and possibly some other coins
        private double NetMHps { get; set; }

        [JsonProperty]
        private double NetMHashps { get; set; }

        [JsonProperty]
        private double NetworkGhps { get; set; }

        [JsonProperty]
        private double NetworkMhps { get; set; }

        [JsonProperty]
        private double NetworkHashps { get; set; }
    
        [JsonIgnore]
        public double NetworkHashPerSec { get; set; }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            NetworkHashPerSec = 0;

            if (NetMHps > 0)
                NetworkHashPerSec = (double)(NetMHps * 1000 * 1000);
            else if (NetMHashps > 0)
                NetworkHashPerSec = (double)(NetMHashps * 1000 * 1000);
            else if (NetworkGhps > 0)
                NetworkHashPerSec = (double)(NetworkGhps * 1000 * 1000 * 1000);
            else if (NetworkMhps > 0)
                NetworkHashPerSec = (double)(NetworkMhps * 1000 * 1000);
            else if (NetworkHashps > 0)
                NetworkHashPerSec = NetworkHashps;
        }
    }
}