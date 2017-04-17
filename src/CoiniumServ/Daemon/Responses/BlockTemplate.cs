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

namespace CoiniumServ.Daemon.Responses
{
    /// <summary>
    /// getblocktemplate is the new decentralized Bitcoin mining protocol, openly developed by the Bitcoin community over mid 2012. It supercedes the old getwork mining protocol.
    /// <remarks>
    /// https://en.bitcoin.it/wiki/Getblocktemplate
    /// https://github.com/bitcoin/bips/blob/master/bip-0022.mediawiki
    /// </remarks>
    /// </summary>
    public class BlockTemplate : IBlockTemplate
    {
        /// <summary>
        /// the compressed difficulty in hexadecimal
        /// </summary>
        public string Bits { get; set; }

        /// <summary>
        /// the current time as seen by the server (recommended for block time) - note this is not necessarily the system clock, and must fall within the mintime/maxtime rules
        /// </summary>
        public UInt32 CurTime { get; set; }

        /// <summary>
        /// the height of the block we are looking for
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// the hash of the previous block, in big-endian hexadecimal
        /// </summary>
        public string PreviousBlockHash { get; set; }

        /// <summary>
        /// number of sigops allowed in blocks
        /// </summary>
        public int SigOpLimit { get; set; }

        /// <summary>
        /// number of bytes allowed in blocks
        /// </summary>
        public int SizeLimit { get; set; }

        /// <summary>
        /// Objects containing information for Bitcoin transactions (excluding coinbase)
        /// </summary>
        public BlockTemplateTransaction[] Transactions { get; set; }

        /// <summary>
        /// always 1 or 2 (at least for bitcoin) - clients MUST understand the implications of the version they use (eg, comply with BIP 0034 for version 2)
        /// </summary>
        public UInt32 Version { get; set; }

        /// <summary>
        /// <see cref="CoinBaseAux"/>
        /// </summary>
        public CoinBaseAux CoinBaseAux { get; set; }

        /// <summary>
        /// information for coinbase transaction
        /// </summary>
        public int CoinbaseTxt { get; set; }

        /// <summary>
        /// total funds available for the coinbase (in Satoshis)
        /// </summary>
        public Int64 Coinbasevalue { get; set; }

        /// <summary>
        /// if provided, this value must be returned with results (see Block Submission)
        /// </summary>
        public int WorkId { get; set; }

        // extra ones - based on https://github.com/CoiniumServ/CoiniumServ/wiki/Litecoin-Testnet-Stream

        public string Target { get; set; }

        public UInt32 MinTime { get; set; }

        public List<string> Mutable { get; set; }

        public string NonceRange { get; set; }
    }

    /// <summary>
    /// data that SHOULD be included in the coinbase's scriptSig content. Only the values (hexadecimal byte-for-byte) in this Object should be included, not the keys. 
    /// This does not include the block height, which is required to be included in the scriptSig by BIP 0034. It is advisable to encode values inside "PUSH" opcodes, 
    /// so as to not inadvertantly expend SIGOPs (which are counted toward limits, despite not being executed).
    /// </summary>
    public class CoinBaseAux
    {
        public string Flags { get; set; }
    }
}
