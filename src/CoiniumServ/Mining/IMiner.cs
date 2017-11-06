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
using CoiniumServ.Accounts;
using CoiniumServ.Pools;
using Newtonsoft.Json;

namespace CoiniumServ.Mining
{
    /// <summary>
    /// Miner interface that any implementations should extend.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public interface IMiner
    {
        /// <summary>
        /// Unique subscription id for identifying the miner.
        /// </summary>
        [JsonProperty("id")]
        int Id { get; }

        /// <summary>
        /// Account for the miner.
        /// </summary>
        IAccount Account { get; set; }

        /// <summary>
        /// Username of the miner.
        /// </summary>
        [JsonProperty("username")]
        string Username { get; }

        /// <summary>
        /// The pool miner is connected to.
        /// </summary>
        IPool Pool { get; }

        /// <summary>
        /// Is the miner authenticated.
        /// </summary>
        [JsonProperty("authenticated")]
        bool Authenticated { get; set; }

        int ValidShareCount { get; set; }

        int InvalidShareCount { get; set; }

        MinerSoftware Software { get; }

        Version SoftwareVersion { get; }

        /// <summary>
        /// Authenticates the miner.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        bool Authenticate(string user, string password);
    }
}
