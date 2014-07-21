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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using CoiniumServ.Coin.Coinbase;
using CoiniumServ.Cryptology.Algorithms;
using CoiniumServ.Cryptology.Merkle;
using CoiniumServ.Daemon.Responses;
using CoiniumServ.Transactions;
using CoiniumServ.Transactions.Utils;
using CoiniumServ.Utils.Extensions;
using CoiniumServ.Utils.Numerics;
using Gibbed.IO;
using Newtonsoft.Json;

namespace CoiniumServ.Server.Mining.Stratum.Notifications
{
    [JsonArray]
    public class Job : IJob
    {
        /// <summary>
        /// ID of the job. Use this ID while submitting share generated from this job.
        /// </summary>
        [JsonIgnore]
        public UInt64 Id { get; private set; }

        public string PreviousBlockHash { get; private set; }

        /// <summary>
        /// Hash of previous block.
        /// </summary>
        [JsonIgnore]
        public string PreviousBlockHashReversed { get; private set; }

        /// <summary>
        /// Initial part of coinbase transaction.
        /// <remarks>The miner inserts ExtraNonce1 and ExtraNonce2 after this section of the coinbase. (https://www.btcguild.com/new_protocol.php)</remarks>
        /// </summary>
        [JsonIgnore]
        public string CoinbaseInitial { get; private set; }

        /// <summary>
        /// Final part of coinbase transaction.
        /// <remarks>The miner appends this after the first part of the coinbase and the two ExtraNonce values. (https://www.btcguild.com/new_protocol.php)</remarks>
        /// </summary>
        [JsonIgnore]
        public string CoinbaseFinal { get; private set; }

        /// <summary>
        /// Coin's block version.
        /// </summary>
        [JsonIgnore]
        public string Version { get; private set; }

        /// <summary>
        /// Encoded current network difficulty.
        /// </summary>
        [JsonIgnore]
        public string EncodedDifficulty { get; private set; }

        public BigInteger Target { get; private set; }

        /// <summary>
        /// Job difficulty.
        /// </summary>
        public double Difficulty { get; private set; }

        /// <summary>
        /// The current time. nTime rolling should be supported, but should not increase faster than actual time.
        /// </summary>
        [JsonIgnore]
        public string nTime { get; private set; }

        /// <summary>
        /// When true, server indicates that submitting shares from previous jobs don't have a sense and such shares will be rejected. When this flag is set, miner should also drop all previous jobs, so job_ids can be eventually rotated. (http://mining.bitcoin.cz/stratum-mining)
        /// <remarks>f true, miners should abort their current work and immediately use the new job. If false, they can still use the current job, but should move to the new one after exhausting the current nonce range. (https://www.btcguild.com/new_protocol.php)</remarks>
        /// </summary>
        [JsonIgnore]
        public bool CleanJobs { get; set; }

        /// <summary>
        /// The assigned hash algorithm for the job.
        /// </summary>
        public IHashAlgorithm HashAlgorithm { get; private set; }

        /// <summary>
        /// Associated block template.
        /// </summary>
        public IBlockTemplate BlockTemplate { get; private set; }

        /// <summary>
        /// Associated generation transaction.
        /// </summary>
        public IGenerationTransaction GenerationTransaction { get; private set; }

        /// <summary>
        /// Merkle tree associated to blockTemplate transactions.
        /// </summary>
        public IMerkleTree MerkleTree { get; private set; }

        /// <summary>
        /// Creates a new instance of JobNotification.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="algorithm"></param>
        /// <param name="blockTemplate"></param>
        /// <param name="generationTransaction"></param>
        public Job(UInt64 id, IHashAlgorithm algorithm, IBlockTemplate blockTemplate, IGenerationTransaction generationTransaction)
        {
            // init the values.
            Id = id;
            HashAlgorithm = algorithm;
            BlockTemplate = blockTemplate;
            GenerationTransaction = generationTransaction;
            PreviousBlockHash = blockTemplate.PreviousBlockHash.HexToByteArray().ToHexString();
            PreviousBlockHashReversed = blockTemplate.PreviousBlockHash.HexToByteArray().ReverseByteOrder().ToHexString();
            CoinbaseInitial = generationTransaction.Initial.ToHexString();
            CoinbaseFinal = generationTransaction.Final.ToHexString();

            // calculate the merkle tree
            this.MerkleTree = new MerkleTree(BlockTemplate.Transactions.GetHashList());
        
            // set version
            Version = BitConverter.GetBytes(blockTemplate.Version.BigEndian()).ToHexString();

            // set the encoded difficulty (bits)
            EncodedDifficulty = blockTemplate.Bits;

            // set the target
            Target = string.IsNullOrEmpty(blockTemplate.Target)
                ? EncodedDifficulty.BigIntFromBitsHex()
                : BigInteger.Parse(blockTemplate.Target, NumberStyles.HexNumber);

            // set the block diff
            Difficulty = ((double)new BigRational(Algorithms.Diff1, Target));

            // set the ntime
            nTime = BitConverter.GetBytes(blockTemplate.CurTime.BigEndian()).ToHexString();
        }

        public IEnumerator<object> GetEnumerator()
        {
            var data = new List<object>
            {
                Id.ToString("x"),
                PreviousBlockHashReversed,
                CoinbaseInitial,
                CoinbaseFinal,
                MerkleTree.Branches,
                Version,
                EncodedDifficulty,
                nTime,
                CleanJobs
            };

            return data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
