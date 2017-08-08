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
using CoiniumServ.Coin.Coinbase;
using CoiniumServ.Cryptology;
using CoiniumServ.Daemon.Responses;
using CoiniumServ.Jobs;
using CoiniumServ.Logging;
using CoiniumServ.Mining;
using CoiniumServ.Server.Mining.Stratum;
using CoiniumServ.Utils.Extensions;
using CoiniumServ.Utils.Helpers;
using CoiniumServ.Utils.Numerics;


namespace CoiniumServ.Shares
{
    public class Share : Loggee<Share>, IShare
	{
        public bool IsValid { get { return Error == ShareError.None; } }
        public bool IsBlockCandidate { get; private set; }
        public Block Block { get; private set; }
        public Transaction GenerationTransaction { get; private set; }
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

		public Share(IStratumMiner miner, UInt64 jobId, IJob job, string extraNonce2, string nTimeString, 
                     string nonceString)
        {
            Miner = miner;
            JobId = jobId;
            Job = job;
            Error = ShareError.None;

            var submitTime = TimeHelpers.NowInUnixTimestamp(); // time we recieved the share from miner.

            if (Job == null)
            {
                Error = ShareError.JobNotFound;
                return;
            }

            // check size of miner supplied extraNonce2
            if (extraNonce2.Length / 2 != ExtraNonce.ExpectedExtraNonce2Size)
            {
                Error = ShareError.IncorrectExtraNonce2Size;
                return;
            }
            ExtraNonce2 = Convert.ToUInt32(extraNonce2, 16); // set extraNonce2 for the share.

            // check size of miner supplied nTime.
            if (nTimeString.Length != 8)
            {
                Error = ShareError.IncorrectNTimeSize;
                return;
            }
            NTime = Convert.ToUInt32(nTimeString, 16); // read ntime for the share

            // make sure NTime is within range.
            if (NTime < job.BlockTemplate.CurTime || NTime > submitTime + 7200)
            {
                Error = ShareError.NTimeOutOfRange;
                return;
            }

            // check size of miner supplied nonce.
            if (nonceString.Length != 8)
            {
                Error = ShareError.IncorrectNonceSize;
                return;
            }
            Nonce = Convert.ToUInt32(nonceString, 16); // nonce supplied by the miner for the share.

            // set job supplied parameters.
            Height = job.BlockTemplate.Height; // associated job's block height.
            ExtraNonce1 = miner.ExtraNonce; // extra nonce1 assigned to miner.

            // check for duplicate shares.
            if (!Job.RegisterShare(this)) // try to register share with the job and see if it's duplicated or not.
            {
                Error = ShareError.DuplicateShare;
                return;
            }

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
            Difficulty = ((double)new BigRational(AlgorithmManager.Diff1, HeaderValue)) * Job.HashAlgorithm.Multiplier;

            // calculate the block difficulty
            BlockDiffAdjusted = Job.Difficulty * Job.HashAlgorithm.Multiplier;


            /*
             * Test false pozitive block candidates: negative bigints were the problem
            byte[] testbytes = new byte[] { 
                0xf7, 0xdf, 0xed, 0xbd, 
                0x9a, 0x2b, 0xa5, 0x1f,
                0x7b, 0x0d, 0x68, 0x76,
                0xbe, 0x1f, 0x18, 0xd6,
                0x2d, 0x49, 0x94, 0x91,
                0x69, 0x11, 0x39, 0x41,
                0xdf, 0x1f, 0x25, 0xdb,
                0x9b, 0x4e, 0x97, 0xb7
            };
            string teststr = testbytes.ReverseBuffer().ToHexString();
            HeaderValue = new BigInteger(testbytes);
            */

            // check if block candicate
            if (Job.Target >= HeaderValue)
            //if (true) //for Debug only
            {
				
                IsBlockCandidate = true;
                BlockHex = Serializers.SerializeBlock(Job, HeaderBuffer, CoinbaseBuffer, miner.Pool.Config.Coin.Options.IsProofOfStakeHybrid);
                BlockHash = HeaderBuffer.DoubleDigest().ReverseBuffer();

				try
				{
                    _logger.Debug("Job.Target is greater than or equal HeaderValue(POW-SCRYPT)!!!:\n{9}\n{10}\n\n" +
                                  "Big-Endian values for Block Header:\n" +
								  "job.BlockTemplate.Version={0}\n" +
								  "job.PreviousBlockHash={1}\n" +
								  "MerkleRoot={2}\n" +
								  "NTime={3}\n" +
								  "job.EncodedDifficulty={4}\n" +
								  "Nonce={5}\n" +
								  "==============\n" +
								  "result={6}\n\n" +
                                  "Big-Endian:\n" +
                                  "BlockHex={7}\n" +
                                  "BlockHash(2xSHA256)={8}\n",
								  job.BlockTemplate.Version,
								  BitConverter.ToString(job.PreviousBlockHash.HexToByteArray()).Replace("-", string.Empty),
								  BitConverter.ToString(MerkleRoot).Replace("-", string.Empty),
								  NTime,
								  job.EncodedDifficulty,
								  Nonce,
                                  BitConverter.ToString(HeaderBuffer).Replace("-", string.Empty),
								  BlockHex,
                                  BitConverter.ToString(BlockHash).Replace("-", string.Empty),
								  Job.Target.ToByteArray().ReverseBuffer().ToHexString(),
                                  HeaderValue.ToByteArray().ReverseBuffer().ToHexString()
							);

				}
				catch (Exception e)
				{
					_logger.Error("Something has happened while logging: ", e);
				}
            }
            else
            {
                IsBlockCandidate = false;
                BlockHash = HeaderBuffer.DoubleDigest().ReverseBuffer();

                // Check if share difficulty reaches miner difficulty.
                var lowDifficulty = Difficulty / miner.Difficulty < 0.99; // share difficulty should be equal or more then miner's target difficulty.

                if (!lowDifficulty) // if share difficulty is high enough to match miner's current difficulty.
                    return; // just accept the share.

                if (Difficulty >= miner.PreviousDifficulty) // if the difficulty matches miner's previous difficulty before the last vardiff triggered difficulty change
                    return; // still accept the share.

                // if the share difficulty can't match miner's current difficulty or previous difficulty                
                Error = ShareError.LowDifficultyShare; // then just reject the share with low difficult share error.
            }
        }

        public void SetFoundBlock(Block block, Transaction genTx)
        {
            Block = block;
            GenerationTransaction = genTx;
        }


        protected override void DescribeYourself()
        {
				_logger.Debug(
					"\nBlockDiffAdjusted={0}\n" +
					"Difficulty={1}\n" +
					"ExtraNonce1={2}\n" +
					"ExtraNonce2={3}\n" +
					"Height={4}\n" +
					"NTime={5}\n" +
					"Nonce={6}",
					BlockDiffAdjusted,
					Difficulty,
					ExtraNonce1,
					ExtraNonce2,
					Height,
					NTime,
					Nonce
			    );
				LogMeSafelyHexString(BlockHash, "BlockHash");
				LogMeSafelyHexString(CoinbaseBuffer, "CoinbaseBuffer");
				//LogMeSafely(CoinbaseHash, "CoinbaseHash");
            _logger.Debug("CoinbaseHash={0}", CoinbaseHash.ToString());
            _logger.Debug("HeaderBuffer={0}",BitConverter.ToString(HeaderBuffer).Replace("-", string.Empty));
				LogMeSafelyHexString(HeaderHash, "HeaderHash");
            _logger.Debug("HeaderValue={0}",BitConverter.ToString(HeaderValue.ToByteArray()).Replace("-", string.Empty));
				LogMeSafelyHexString(MerkleRoot, "MerkleRoot");

				Job.DescribeYourselfSafely();
        }
    }
}
