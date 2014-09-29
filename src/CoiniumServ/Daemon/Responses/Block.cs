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
using CoiniumServ.Daemon.Converters;
using Newtonsoft.Json;

namespace CoiniumServ.Daemon.Responses
{
    public class Block
    {
        public string Hash { get; set; }

        public Int32 Confirmations { get; set; }

        public Int32 Size { get; set; }

        public Int32 Height { get; set; }

        public Int32 Version { get; set; }

        /// <summary>
        /// Every transaction has a hash associated with it. In a block, all of the transaction hashes in the block are themselves hashed (sometimes several times -- the exact process is complex), and the result is the Merkle root. In other words, the Merkle root is the hash of all the hashes of all the transactions in the block. The Merkle root is included in the block header. With this scheme, it is possible to securely verify that a transaction has been accepted by the network (and get the number of confirmations) by downloading just the tiny block headers and Merkle tree -- downloading the entire block chain is unnecessary.
        /// </summary>
        public string MerkleRoot { get; set; }

        public List<string> Tx { get; set; }

        [JsonConverter(typeof(TimeConverter))]
        public Int32 Time { get; set; }

        public UInt32 Nonce { get; set; }

        public string Bits { get; set; }

        public double Difficulty { get; set; }

        public string NextBlockHash { get; set; }
    }
}
