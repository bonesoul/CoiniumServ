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
using CoiniumServ.Coin.Coinbase;
using CoiniumServ.Cryptology;
using CoiniumServ.Cryptology.Algorithms;
using CoiniumServ.Daemon.Responses;
using CoiniumServ.Miners;
using CoiniumServ.Server.Mining.Stratum;
using CoiniumServ.Server.Mining.Stratum.Notifications;
using CoiniumServ.Utils.Extensions;
using CoiniumServ.Utils.Numerics;
using Serilog;

namespace CoiniumServ.Shares
{
    public class Share : IShare
    {
        public bool IsValid { get { return Error == ShareError.None; } }
        public bool IsBlockCandidate { get; private set; }
        public Block Block { get; private set; }
        public bool IsBlockAccepted { get { return Block != null; } }
        public IMiner Miner { get; private set; }
        public ShareError Error { get; private set; }
        public UInt64 JobId { get; private set; }
        public IJob Job { get; private set; }
        public int Height { get; private set; }
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

        public Share(IStratumMiner miner, UInt64 jobId, IJob job, string extraNonce2, string nTimeString, string nonceString)
        {
            Miner = miner;
            JobId = jobId;
            Job = job;
            Error = ShareError.None;

            // TODO: add extranonce2 size check!.
            // TODO: add duplicate share check.
            // TODO: add nTime out of range check

            if (Job == null)
            {
                Error = ShareError.JobNotFound;
                Log.ForContext<Share>().Warning("Job doesn't exist: {0}", JobId);
                return;
            }
            
            // check miner supplied nTime.
            if (nTimeString.Length != 8)
            {
                Error = ShareError.IncorrectNTimeSize;
                Log.ForContext<Share>().Warning("Incorrect size of nTime");
                return;
            }

            NTime = Convert.ToUInt32(nTimeString, 16); // ntime for the share

            // check miner supplied nonce.
            if (nonceString.Length != 8)
            {
                Error = ShareError.IncorrectNonceSize;
                Log.ForContext<Share>().Warning("incorrect size of nonce");
                return;
            }

            Nonce = Convert.ToUInt32(nonceString, 16); // nonce supplied by the miner for the share.

            // check miner supplied extraNonce2
            ExtraNonce2 = Convert.ToUInt32(extraNonce2, 16);

            // check job supplied parameters.
            Height = job.BlockTemplate.Height;
            ExtraNonce1 = miner.ExtraNonce; // extra nonce1 assigned to miner.

            // construct the coinbase.
            CoinbaseBuffer = Serializers.SerializeCoinbase(Job, ExtraNonce1, ExtraNonce2); 
            CoinbaseHash = Coin.Coinbase.Utils.HashCoinbase(CoinbaseBuffer);

            // create the merkle root.
            MerkleRoot = Job.MerkleTree.WithFirst(CoinbaseHash).ReverseBuffer();

            // create the block headers
            HeaderBuffer = Serializers.SerializeHeader(Job, MerkleRoot, NTime, Nonce);
            HeaderHash = Job.HashAlgorithm.Hash(HeaderBuffer, miner.Pool.Config.Coin.Options);
            HeaderValue = new BigInteger(HeaderHash);

            // calculate the share difficulty
            Difficulty = ((double)new BigRational(Algorithms.Diff1, HeaderValue)) * Job.HashAlgorithm.Multiplier;

            // calculate the block difficulty
            BlockDiffAdjusted = Job.Difficulty * Job.HashAlgorithm.Multiplier;

            // check if block candicate
            if (Job.Target >= HeaderValue)
            {
                IsBlockCandidate = true;
                BlockHex = Serializers.SerializeBlock(Job, HeaderBuffer, CoinbaseBuffer);
                BlockHash = HeaderBuffer.DoubleDigest().ReverseBuffer(); // TODO: make sure this is okay!
            }
            else
            {
                IsBlockCandidate = false;
                BlockHash = HeaderBuffer.DoubleDigest().ReverseBuffer();

                // Check if share difficulty reaches miner difficulty.
                if (Difficulty / 16 < 0.99)
                {
                    // todo: add low difficulty share check.
                }
            }
        }

        public void SetFoundBlock(Block block)
        {
            Block = block;
        }
    }
}
