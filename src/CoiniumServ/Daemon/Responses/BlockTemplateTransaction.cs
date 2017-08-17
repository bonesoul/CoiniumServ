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
using CoiniumServ.Logging;

namespace CoiniumServ.Daemon.Responses
{
    /// <summary>
    /// <remarks>
    /// https://github.com/bitcoin/bips/blob/master/bip-0022.mediawiki#Transactions%20Object%20Format
    /// </remarks>
    /// </summary>
    public class BlockTemplateTransaction:Loggee<BlockTemplateTransaction>
    {

        /// <summary>
        /// transaction data encoded in hexadecimal (byte-for-byte)
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// other transactions before this one (by 1-based index in "transactions" list) that must be present in the final block if this one is; 
        /// if key is not present, dependencies are unknown and clients MUST NOT assume there aren't any
        /// </summary>
        public int[] Depends { get; set; }

        /// <summary>
        /// difference in value between transaction inputs and outputs (in Satoshis); for coinbase transactions, this is a negative Number of the 
        /// total collected block fees (ie, not including the block subsidy); if key is not present, fee is unknown and clients MUST NOT assume there isn't one
        /// </summary>
        public UInt64 Fee { get; set; }

        /// <summary>
        /// hash/id encoded in little-endian hexadecimal
        /// </summary>
        public string Hash { get; set; }

        /// <summary>
        /// if provided and true, this transaction must be in the final block
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// total number of SigOps, as counted for purposes of block limits; if key is not present, sigop count is unknown 
        /// and clients MUST NOT assume there aren't any
        /// </summary>
        public int Sigops { get; set; }

        /// <summary>
        /// BIP-145:
        /// transaction id encoded in hexadecimal; required for transactions with witness data
        /// </summary>
        public string Txid { get; set; }

        /// <summary>
        /// BIP-145:
        /// numeric weight of the transaction, as counted for purposes of the block's weightlimit; 
        /// if key is not present, weight is unknown and clients MUST NOT assume it is zero, 
        /// although they MAY choose to calculate it themselves
        /// </summary>
        public UInt32 Weight { get; set; }



        protected override void DescribeYourself()
        {
            _logger.Debug(
                "Data={0}\n" +
                "Fee={1}\n" +
                "Hash={2}\n" +
                "Txid={3}\n" +
                "Weight={4}\n" +
                "Sigops={5}\n",
                Data,
                Fee,
                Hash,
                Txid,
                Weight,
                Sigops
            );
        }
    }
}
