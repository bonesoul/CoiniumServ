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
using System.Collections.Generic;
using CoiniumServ.Accounts;
using CoiniumServ.Algorithms;
using CoiniumServ.Daemon;
using CoiniumServ.Mining;
using CoiniumServ.Payments;
using CoiniumServ.Server.Web.Service;
using Newtonsoft.Json;

namespace CoiniumServ.Pools
{
    [JsonObject(MemberSerialization.OptIn)]
    public interface IPool : IJsonService
    {
        /// <summary>
        /// Is the pool initialized?
        /// </summary>
        bool Initialized { get; }

        [JsonProperty("hashrate")]
        double Hashrate { get; }

        [JsonProperty("round")]
        Dictionary<string, double> RoundShares { get; }

        [JsonProperty("config")]
        IPoolConfig Config { get; }

        IHashAlgorithm HashAlgorithm { get; }

        [JsonProperty("miners")]
        IMinerManager MinerManager { get; }

        [JsonProperty("network")]
        INetworkInfo NetworkInfo { get; }

        [JsonProperty("blocks")]
        IBlockRepository BlockRepository { get; }

        IPaymentRepository PaymentRepository { get; }

        /// <summary>
        /// Coin daemon assigned to pool.
        /// </summary>
        IDaemonClient Daemon { get; }

        IAccountManager AccountManager { get; }

        void Initialize();
    }
}
