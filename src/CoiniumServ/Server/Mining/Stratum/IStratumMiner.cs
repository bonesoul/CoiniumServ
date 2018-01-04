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
