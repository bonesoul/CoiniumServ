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
