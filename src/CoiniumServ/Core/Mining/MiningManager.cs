/*
 *   Coinium - Crypto Currency Pool Software - https://github.com/CoiniumServ/CoiniumServ
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
using System.Security.Cryptography;
using System.Threading;
using Coinium.Common.Extensions;
using Coinium.Core.Coin;
using Coinium.Core.Coin.Daemon;
using Coinium.Core.Crypto;
using Coinium.Core.Server.Stratum;
using Coinium.Core.Server.Stratum.Notifications;
using Coinium.Net.Server.Sockets;
using Gibbed.IO;
using Serilog;

namespace Coinium.Core.Mining
{
    /// <summary>
    /// Miner manager that manages all connected miners over different ports.
    /// </summary>
    public class MiningManager
    {
        private int _counter; // counter for assigining unique id's to miners.

        private readonly Dictionary<int, IMiner> _miners = new Dictionary<int, IMiner>(); // Dictionary that holds id <-> miner pairs. 

        private readonly Dictionary<UInt64, Job> _jobs = new Dictionary<UInt64, Job>();

        private Timer _timer;

        private SHA256Managed _sha256Managed = new SHA256Managed();

        public MiningManager()
        {
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

            // create the difficulty notification.
            var difficulty = new Difficulty(16);

            // create the job notification.
            var job = new Job(blockTemplate, generationTransaction)
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

        public bool ProcessShare(StratumMiner miner, string jobId, string extraNonce2, string nTime, string nonce)
        {
            // check if the job exists
            var id = Convert.ToUInt64(jobId, 16);
            var job = MiningManager.Instance.GetJob(id);

            if (job == null)
            {
                Log.Warning("Job doesn't exist: {0}", id);
                return false;
            }

            var coinbaseBuffer = this.SerializeCoinbase(job, ExtraNonce.Instance.Current, Convert.ToUInt32(extraNonce2));
            var coinbaseHash = this.HashCoinbase(coinbaseBuffer);

            return true;
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

        private Sha256Hash HashCoinbase(byte[] coinbase)
        {
            return new Sha256Hash(coinbase.DoubleDigest());
        }


        private static readonly MiningManager _instance = new MiningManager(); // memory instance of the MinerManager.

        /// <summary>
        /// Singleton instance of WalletManager.
        /// </summary>
        public static MiningManager Instance { get { return _instance; } }
    }
}
