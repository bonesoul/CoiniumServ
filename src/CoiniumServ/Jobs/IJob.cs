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
using CoiniumServ.Algorithms;
using CoiniumServ.Cryptology.Merkle;
using CoiniumServ.Daemon.Responses;
using CoiniumServ.Shares;
using CoiniumServ.Transactions;
using CoiniumServ.Utils.Numerics;
using Newtonsoft.Json;

namespace CoiniumServ.Jobs
{

    [JsonArray]
    public interface IJob : IEnumerable<object>
    {
        /// <summary>
        /// ID of the job. Use this ID while submitting share generated from this job.
        /// </summary>
        UInt64 Id { get; }

        /// <summary>
        /// Height of the block we are looking for.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Hash of previous block.
        /// </summary>
        string PreviousBlockHash { get; }

        string PreviousBlockHashReversed { get;  }

        /// <summary>
        /// Initial part of coinbase transaction.
        /// <remarks>The miner inserts ExtraNonce1 and ExtraNonce2 after this section of the coinbase. (https://www.btcguild.com/new_protocol.php)</remarks>
        /// </summary>
        string CoinbaseInitial { get; }

        /// <summary>
        /// Final part of coinbase transaction.
        /// <remarks>The miner appends this after the first part of the coinbase and the two ExtraNonce values. (https://www.btcguild.com/new_protocol.php)</remarks>
        /// </summary>
        string CoinbaseFinal { get; }

        /// <summary>
        /// Coin's block version.
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Encoded current network difficulty.
        /// </summary>
        string EncodedDifficulty { get; }

        BigInteger Target { get; }

        /// <summary>
        /// Job difficulty.
        /// </summary>
        double Difficulty { get; }

        /// <summary>
        /// The current time. nTime rolling should be supported, but should not increase faster than actual time.
        /// </summary>
        string NTime { get; }

        /// <summary>
        /// When true, server indicates that submitting shares from previous jobs don't have a sense and such shares will be rejected. When this flag is set, miner should also drop all previous jobs, so job_ids can be eventually rotated. (http://mining.bitcoin.cz/stratum-mining)
        /// <remarks>f true, miners should abort their current work and immediately use the new job. If false, they can still use the current job, but should move to the new one after exhausting the current nonce range. (https://www.btcguild.com/new_protocol.php)</remarks>
        /// </summary>
        bool CleanJobs { get; set; }

        /// <summary>
        /// Creation time of the job.
        /// </summary>
        int CreationTime { get; }

        /// <summary>
        /// The assigned hash algorithm for the job.
        /// </summary>
        IHashAlgorithm HashAlgorithm { get; }

        /// <summary>
        /// Associated block template.
        /// </summary>
        IBlockTemplate BlockTemplate { get; }

        /// <summary>
        /// Associated generation transaction.
        /// </summary>
        IGenerationTransaction GenerationTransaction { get; }

        /// <summary>
        /// Merkle tree associated to blockTemplate transactions.
        /// </summary>
        IMerkleTree MerkleTree { get; }

        new IEnumerator<object> GetEnumerator();

        bool RegisterShare(IShare share);
    }
}
