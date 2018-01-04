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

namespace CoiniumServ.Daemon.Responses
{
    // Documentation: 
    // https://en.bitcoin.it/wiki/Getwork
    // https://github.com/sinisterchipmunk/bitpool/wiki/Bitcoin-Mining-Pool-Developer's-Reference
    // https://bitcointalk.org/index.php?topic=51281.0

    // Note: Do not rename member field names with uppercase ones as it will break getwork protocol @ json-rpc 1.0.

    public class Getwork
    {        
        /// <summary>
        /// This should be advertised iff the miner supports generating its own midstates. In this case, the pool may decide to omit the now-deprecated "midstate" and "hash1" fields in the work response.
        /// </summary>
        public string midstate { get; set; }

        /// <summary>
        /// Pre-processed SHA-2 input chunks, in little-endian order, as a hexadecimal-encoded string
        /// </summary>
        public string data { get; set; }

        public string hash1 { get; set; }

        /// <summary>
        /// Proof-of-work hash target as a hexadecimal-encoded string
        /// </summary>
        public string target { get; set; }

        /// <summary>
        /// Brief specification of proof-of-work algorithm. Not provided by bitcoind or most poolservers.
        /// </summary>
        public string algorithm { get; set; }
    }
}
