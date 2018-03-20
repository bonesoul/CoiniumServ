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
