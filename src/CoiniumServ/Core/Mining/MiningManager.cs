/*
 *   CoiniumServ - crypto currency pool software - https://github.com/CoiniumServ/CoiniumServ
 *   Copyright (C) 2013 - 2014, Coinium Project - http://www.coinium.org
 *
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Coinium.Common.Constants;
using Coinium.Common.Extensions;
using Coinium.Core.Coin;
using Coinium.Core.Coin.Processors;
using Coinium.Core.Coin.Daemon;
using Coinium.Core.Crypto;
using Coinium.Core.Server.Stratum;
using Coinium.Core.Server.Stratum.Notifications;
using Coinium.Net.Server.Sockets;
using Gibbed.IO;
using Org.BouncyCastle.Math;
using Serilog;

namespace Coinium.Core.Mining
{
    /// <summary>
    /// Miner manager that manages all connected miners over different ports.
    /// </summary>
    public class MiningManager : IMiningManager
    {
        private int _counter; // counter for assigining unique id's to miners.

        private readonly Dictionary<int, IMiner> _miners = new Dictionary<int, IMiner>(); // Dictionary that holds id <-> miner pairs. 

        private readonly Dictionary<UInt64, Job> _jobs = new Dictionary<UInt64, Job>();

        private Timer _timer;

        private BigInteger diff1;

        private readonly ICoinProcessorFactory _coinProcessorFactory;

        public MiningManager(ICoinProcessorFactory coinProcessorFactory)
        {
            _coinProcessorFactory = coinProcessorFactory;

            this.diff1 = new BigInteger("00000000ffff0000000000000000000000000000000000000000000000000000", 16);
            this._timer = new Timer(BroadcastJobs, null, TimeSpan.Zero, new TimeSpan(0, 0, 0, 10)); // setup a timer to broadcast jobs.
            this.BroadcastJobs(null);

            Log.Verbose("MinerManager() init..");
        }

        /// <summary>
        /// Creates a new instance of IMiner type.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public T Create<T>(IConnection connection) where T : IMiner
        {
            var instance = Activator.CreateInstance(typeof(T), new object[] { this._counter++, connection });  // create an instance of the miner.
            var miner = (IMiner) instance;
            this._miners.Add(miner.Id, miner); // add it to our collection.           

            return (T)miner;
        }

        /// <summary>
        /// Creates a new instance of IMiner type.
        /// </summary>
        /// <returns></returns>
        public T Create<T>() where T : IMiner
        {
            var instance = Activator.CreateInstance(typeof(T), new object[] { this._counter++ }); // create an instance of the miner.
            var miner = (IMiner)instance;
            this._miners.Add(miner.Id, miner); // add it to our collection.

            return (T)miner;
        }

        /// <summary>
        /// Broadcasts to miners.
        /// </summary>
        /// <example>
        /// sample communication: http://bitcoin.stackexchange.com/a/23112/8899
        /// </example>
        /// <param name="state"></param>
        private void BroadcastJobs(object state)
        {

            var blockTemplate = DaemonManager.Instance.Client.GetBlockTemplate();
            var generationTransaction = new GenerationTransaction(blockTemplate, false);

            var hashList = new List<byte[]>();
            
            foreach (var transaction in blockTemplate.Transactions)
            {
                hashList.Add(transaction.Hash.HexToByteArray());
            }            
            
            var merkleTree = new MerkleTree(hashList);

            
            // create the difficulty notification.
            var difficulty = new Difficulty(16);

            // create the job notification.
            var job = new Job(blockTemplate, generationTransaction, merkleTree)
            {
                CleanJobs = true // tell the miners to clean their existing jobs and start working on new one.
            };

            this._jobs.Add(job.Id,job);

            foreach (var pair in this._miners)
            {
                var miner = pair.Value;

                if (!miner.SupportsJobNotifications)
                    continue;

                miner.SendDifficulty(difficulty);
                miner.SendJob(job);
            }
        }

        public Job GetJob(UInt64 id)
        {
            return this._jobs.ContainsKey(id) ? this._jobs[id] : null;
        }

        public bool ProcessShare(IMiner miner, string jobId, string extraNonce2, string nTimeString, string nonceString)
        {
            // check if the job exists
            var id = Convert.ToUInt64(jobId, 16);
            var job = this.GetJob(id);

            if (job == null)
            {
                Log.Warning("Job doesn't exist: {0}", id);
                return false;
            }

            if (nTimeString.Length != 8)
            {
                Log.Warning("Incorrect size of nTime");
                return false;
            }

            if (nonceString.Length != 8)
            {
                Log.Warning("incorrect size of nonce");
                return false;
            }

            var nTime = Convert.ToUInt32(nTimeString, 16);
            var nonce = Convert.ToUInt32(nonceString, 16);

            var coinbase = this.SerializeCoinbase(job, ExtraNonce.Instance.Current, Convert.ToUInt32(extraNonce2, 16));
            var coinbaseHash = this.HashCoinbase(coinbase);

            var merkleRoot = job.MerkleTree.WithFirst(coinbaseHash).ReverseBytes();

            // TODO: The name should be pulled from the config
            var coinProcessor = _coinProcessorFactory.Get(CoinProcessorNames.Scrypt);

            var header = this.SerializeHeader(job, merkleRoot, nTime, nonce);
            var headerHash = coinProcessor.Hash(header);
            var headerValue = new BigInteger(headerHash.ToHexString(), 16);

            var shareDiff = diff1.Divide(headerValue).Multiply(BigInteger.ValueOf(coinProcessor.Multiplier));
            var blockDiffAdjusted = 16 * coinProcessor.Multiplier;

            var target = new BigInteger(job.NetworkDifficulty, 16);
            if (target.Subtract(headerValue).IntValue > 0) // Check if share is a block candidate (matched network difficulty)
            {
                var block = this.SerializeBlock(job, header, coinbase).ToHexString();
            }
            else // invalid share.
            {
               
            }

            return true;
        }

        /// <summary>
        /// Block data structure.
        /// </summary>
        /// <remarks>
        /// https://en.bitcoin.it/wiki/Protocol_specification#block
        /// </remarks>
        /// <param name="header"></param>
        /// <param name="coinbase"></param>
        /// <returns></returns>
        private byte[] SerializeBlock(Job job, byte[] header, byte[] coinbase)
        {
            byte[] result;

            using (var stream = new MemoryStream())
            {
                stream.WriteBytes(header);
                stream.WriteBytes(CoinbaseUtils.VarInt((UInt32) job.BlockTemplate.Transactions.Length));
                stream.WriteBytes(coinbase);

                foreach (var transaction in job.BlockTemplate.Transactions)
                {
                    stream.WriteBytes(transaction.Data.HexToByteArray());
                }

                // need to implement POS support too.

                result = stream.ToArray();
            }

            return result;
        }

        /// <summary>
        /// Block headers are sent in a headers packet in response to a getheaders message.
        /// </summary>
        /// <remarks>
        /// https://en.bitcoin.it/wiki/Protocol_specification#Block_Headers
        /// </remarks>
        /// <example>
        /// nodejs: https://github.com/zone117x/node-stratum-pool/blob/master/lib/blockTemplate.js#L85
        /// </example>
        /// <param name="job"></param>
        /// <param name="merkleRoot"></param>
        /// <param name="nTime"></param>
        /// <param name="nonce"></param>
        /// <returns></returns>
        private byte[] SerializeHeader(Job job, byte[] merkleRoot, UInt32 nTime, UInt32 nonce)
        {
            byte[] result;

            using (var stream = new MemoryStream())
            {
                stream.WriteValueU32(nonce);
                stream.WriteValueU32(Convert.ToUInt32(job.NetworkDifficulty, 16));
                stream.WriteValueU32(nTime);
                stream.WriteBytes(merkleRoot);
                stream.WriteBytes(job.PreviousBlockHash.HexToByteArray());
                stream.WriteValueU32(job.BlockTemplate.Version.BigEndian());

                result = stream.ToArray();
                result = result.ReverseBytes();
            }

            return result;
        }

        private byte[] SerializeCoinbase(Job job, UInt64 extraNonce1, UInt32 extraNonce2)
        {
            var extraNonce1Buffer = BitConverter.GetBytes(extraNonce1);
            var extraNonce2Buffer = BitConverter.GetBytes(extraNonce2);

            byte[] result;

            using (var stream = new MemoryStream())
            {
                stream.WriteBytes(job.CoinbaseInitial.HexToByteArray());
                stream.WriteBytes(extraNonce1Buffer);
                stream.WriteBytes(extraNonce2Buffer);
                stream.WriteBytes(job.CoinbaseFinal.HexToByteArray());

                result = stream.ToArray();
            }

            return result;
        }

        private Hash HashCoinbase(byte[] coinbase)
        {
            return coinbase.DoubleDigest();
        }
    }
}
