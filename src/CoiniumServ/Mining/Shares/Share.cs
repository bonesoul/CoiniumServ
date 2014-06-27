#region License
// 
//     CoiniumServ - Crypto Currency Mining Pool Server Software
//     Copyright (C) 2013 - 2014, CoiniumServ Project - http://www.coinium.org
//     https://github.com/CoiniumServ/CoiniumServ
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
using Coinium.Coin.Coinbase;
using Coinium.Crypto;
using Coinium.Server.Stratum.Notifications;
using Coinium.Utils.Extensions;
using Coinium.Utils.Numerics;
using Serilog;

namespace Coinium.Mining.Shares
{
    public class Share : IShare
    {
        public bool Valid
        {
            get { return Error == ShareError.None; }
        }
        public bool Candidate { get; private set; }
        public ShareError Error { get; private set; }
        public IJob Job { get; private set; }
        public UInt32 NTime { get; private set; }
        public UInt32 Nonce { get; private set; }
        public UInt32 ExtraNonce1 { get; private set; }
        public UInt32 ExtraNonce2 { get; private set; }
        public byte[] CoinbaseBuffer { get; private set; }
        public Hash CoinbaseHash { get; private set; }
        public byte[] MerkleRoot { get; private set; }
        public byte[] HeaderBuffer { get; private set; }
        public byte[] HeaderHash { get; private set; }
        public BigInteger HeaderValue { get; private set; }
        public Double Difficulty { get; private set; }
        public double BlockDiffAdjusted { get; private set; }
        public byte[] BlockHex { get; private set; }
        public byte[] BlockHash { get; private set; }

        public Share(UInt64 jobId, IJob job, UInt32 extraNonce1, string extraNonce2, string nTimeString, string nonceString)
        {
            Error = ShareError.None;
            Job = job;

            // TODO: add extranonce2 size check!.

            if (Job == null)
            {
                Error = ShareError.JobNotFound;
                Log.ForContext<Share>().Warning("Job doesn't exist: {0}", jobId);
                return;
            }

            // check miner supplied parameters

            if (nTimeString.Length != 8)
            {
                Error = ShareError.IncorrectNTimeSize;
                Log.ForContext<Share>().Warning("Incorrect size of nTime");
                return;
            }

            NTime = Convert.ToUInt32(nTimeString, 16); // ntime for the share

            // TODO: add nTime out of range check

            if (nonceString.Length != 8)
            {
                Error = ShareError.IncorrectNonceSize;
                Log.ForContext<Share>().Warning("incorrect size of nonce");
                return;
            }

            Nonce = Convert.ToUInt32(nonceString, 16); // nonce supplied by the miner for the share.

            // TODO: add duplicate share check.

            // job supplied parameters.
            ExtraNonce1 = extraNonce1; // extra nonce1 assigned to job.
            ExtraNonce2 = Convert.ToUInt32(extraNonce2, 16); // extra nonce2 assigned to job.

            // construct the coinbase.
            CoinbaseBuffer = Serializers.SerializeCoinbase(Job, ExtraNonce1, ExtraNonce2); 
            CoinbaseHash = Coin.Coinbase.Utils.HashCoinbase(CoinbaseBuffer);

            // create the merkle root.
            MerkleRoot = Job.MerkleTree.WithFirst(CoinbaseHash).ReverseBuffer();

            // create the block headers
            HeaderBuffer = Serializers.SerializeHeader(Job, MerkleRoot, NTime, Nonce);
            HeaderHash = Job.HashAlgorithm.Hash(HeaderBuffer);
            HeaderValue = new BigInteger(HeaderHash);

            // calculate the share difficulty
            Difficulty = ((double)new BigRational(Job.HashAlgorithm.Difficulty, HeaderValue)) * Job.HashAlgorithm.Multiplier;

            // calculate the block difficulty
            BlockDiffAdjusted = Job.Difficulty * Job.HashAlgorithm.Multiplier;

            // check if block candicate
            if (Job.Target >= HeaderValue)
            {
                Candidate = true;
                BlockHex = Serializers.SerializeBlock(Job, HeaderBuffer, CoinbaseBuffer);
                BlockHash = HeaderBuffer.DoubleDigest().ReverseBuffer(); // TODO: make sure this is okay!
            }
            else
            {
                Candidate = false;
                BlockHash = HeaderBuffer.DoubleDigest().ReverseBuffer();

                // Check if share difficulty reaches miner difficulty.
                if (Difficulty / 16 < 0.99)
                {
                    // todo: add low difficulty share check.
                }
            }
        }
    }
}
