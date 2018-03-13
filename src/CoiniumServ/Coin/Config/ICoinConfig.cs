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

using CoiniumServ.Configuration;
using Newtonsoft.Json;

namespace CoiniumServ.Coin.Config
{
    [JsonObject(MemberSerialization.OptIn)]
    public interface ICoinConfig:IConfig
    {
        /// <summary>
        /// name of the coin
        /// </summary>
        [JsonProperty("name")]
        string Name { get; }

        /// <summary>
        /// 3 or 4 letter symbol for the coin
        /// </summary>
        [JsonProperty("symbol")]
        string Symbol { get; }

        /// <summary>
        /// The algorithm used by the coin.
        /// </summary>
        [JsonProperty("algorithm")]
        string Algorithm { get; }
        
        /// <summary>
        /// Does this Coin use the new getaddressinfo RPC call? true = yes, false = no
        /// </summary>
        [JsonProperty("rpcupdate")]
        bool RpcUpdate { get; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("site")]
        string Site { get; }

        ICoinOptions Options { get; }

        IBlockExplorerOptions BlockExplorer { get; }

        dynamic Extra { get; }
    }
}
