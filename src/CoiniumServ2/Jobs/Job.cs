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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using CoiniumServ.Algorithms;
using CoiniumServ.Coin.Coinbase;
using CoiniumServ.Cryptology.Merkle;
using CoiniumServ.Daemon.Responses;
using CoiniumServ.Shares;
using CoiniumServ.Transactions;
using CoiniumServ.Transactions.Utils;
using CoiniumServ.Utils.Extensions;
using CoiniumServ.Utils.Helpers;
using CoiniumServ.Utils.Numerics;
using Gibbed.IO;

namespace CoiniumServ.Jobs
{
    public class Job : IJob
    {
        public UInt64 Id { get; private set; }

        public int Height { get; private set; }

        public string PreviousBlockHash { get; private set; }

        public string PreviousBlockHashReversed { get; private set; }

        public string CoinbaseInitial { get; private set; }

        public string CoinbaseFinal { get; private set; }

        public string Version { get; private set; }

        public string EncodedDifficulty { get; private set; }

        public BigInteger Target { get; private set; }

        public double Difficulty { get; private set; }

        public string NTime { get; private set; }

        public bool CleanJobs { get; set; }

        public int CreationTime { get; private set; }

        public IHashAlgorithm HashAlgorithm { get; private set; }

        public IBlockTemplate BlockTemplate { get; private set; }

        public IGenerationTransaction GenerationTransaction { get; private set; }

        public IMerkleTree MerkleTree { get; private set; }

        /// <summary>
        /// List of shares submitted by miners in order to determine duplicate shares.
        /// </summary>
        private readonly IList<UInt64> _shares;

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
            Height = blockTemplate.Height;
            GenerationTransaction = generationTransaction;
            PreviousBlockHash = blockTemplate.PreviousBlockHash.HexToByteArray().ToHexString();
            PreviousBlockHashReversed = blockTemplate.PreviousBlockHash.HexToByteArray().ReverseByteOrder().ToHexString();
            CoinbaseInitial = generationTransaction.Initial.ToHexString();
            CoinbaseFinal = generationTransaction.Final.ToHexString();
            CreationTime = TimeHelpers.NowInUnixTimestamp();

            _shares = new List<UInt64>();

            // calculate the merkle tree
            MerkleTree = new MerkleTree(BlockTemplate.Transactions.GetHashList());
        
            // set version
            Version = BitConverter.GetBytes(blockTemplate.Version.BigEndian()).ToHexString();

            // set the encoded difficulty (bits)
            EncodedDifficulty = blockTemplate.Bits;

            // set the target
            Target = string.IsNullOrEmpty(blockTemplate.Target)
                ? EncodedDifficulty.BigIntFromBitsHex()
                : BigInteger.Parse(blockTemplate.Target, NumberStyles.HexNumber);

            // set the block diff
            Difficulty = ((double)new BigRational(AlgorithmManager.Diff1, Target));

            // set the ntime
            NTime = BitConverter.GetBytes(blockTemplate.CurTime.BigEndian()).ToHexString();
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
                NTime,
                CleanJobs
            };

            return data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool RegisterShare(IShare share)
        {
            var submissionId = (UInt64) (share.ExtraNonce1 + share.ExtraNonce2 + share.NTime + share.Nonce); // simply hash the share by summing them..

            if(_shares.Contains(submissionId)) // if our list already contain the share
                return false; // it basically means we hit a duplicate share.

            _shares.Add(submissionId); // if the code flows here, that basically means we just recieved a new share.
            return true;
        }
    }
}
