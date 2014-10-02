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
using CoiniumServ.Cryptology;
using CoiniumServ.Daemon.Responses;
using CoiniumServ.Jobs;
using CoiniumServ.Mining;
using CoiniumServ.Utils.Numerics;

namespace CoiniumServ.Shares
{
    public interface IShare
    {
        /// <summary>
        /// Is a valid share.
        /// </summary>
        bool IsValid { get; }
        /// <summary>
        /// Does it contain a block candicate.
        /// </summary>
        bool IsBlockCandidate { get; }

        Block Block { get; }

        Transaction GenerationTransaction { get; }

        /// <summary>
        /// Is the block data accepted by the coin daemon?
        /// </summary>
        bool IsBlockAccepted { get; }

        IMiner Miner { get; }

        ShareError Error { get; }

        UInt64 JobId { get; }

        IJob Job { get; }

        int Height { get; }

        UInt32 NTime { get; }

        UInt32 Nonce { get; }

        UInt32 ExtraNonce1 { get; }

        UInt32 ExtraNonce2 { get; }

        byte[] CoinbaseBuffer { get; }

        Hash CoinbaseHash { get; }

        /// <summary>
        /// Every transaction has a hash associated with it. In a block, all of the transaction hashes in the block are themselves hashed (sometimes several times -- the exact process is complex), and the result is the Merkle root. In other words, the Merkle root is the hash of all the hashes of all the transactions in the block. The Merkle root is included in the block header. With this scheme, it is possible to securely verify that a transaction has been accepted by the network (and get the number of confirmations) by downloading just the tiny block headers and Merkle tree -- downloading the entire block chain is unnecessary.
        /// </summary>
        byte[] MerkleRoot { get; }

        byte[] HeaderBuffer { get; }

        byte[] HeaderHash { get; }

        BigInteger HeaderValue { get; }

        Double Difficulty { get; }

        Double BlockDiffAdjusted { get; }

        byte[] BlockHex { get; }

        byte[] BlockHash { get; }

        void SetFoundBlock(Block block, Transaction genTx);
    }
}
