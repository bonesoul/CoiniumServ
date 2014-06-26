#region License
// 
//     CoiniumServ - crypto currency pool software - https://github.com/CoiniumServ/CoiniumServ
//     Copyright (C) 2013 - 2014, Coinium Project - http://www.coinium.org
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
using System.Collections.Generic;
using Coinium.Coin.Algorithms;
using Coinium.Coin.Daemon;
using Coinium.Mining.Miners;
using Coinium.Server.Stratum.Notifications;
using Coinium.Transactions;

namespace Coinium.Mining.Jobs
{
    public class JobManager : IJobManager
    {
        public Dictionary<UInt64, IJob> Jobs { get; private set; }

        public IJobCounter JobCounter { get; private set; }

        private readonly IDaemonClient _daemonClient;

        private readonly IMinerManager _minerManager;

        private readonly IHashAlgorithm _hashAlgorithm;

        private IExtraNonce _extraNonce;

        public IExtraNonce ExtraNonce { get { return _extraNonce; } }

        public IJob LastJob { get; private set; }

        public JobManager(IDaemonClient daemonClient, IMinerManager minerManager, IHashAlgorithm hashAlgorithm)
        {
            _daemonClient = daemonClient;
            _minerManager = minerManager;
            _hashAlgorithm = hashAlgorithm;
            JobCounter = new JobCounter();
            Jobs = new Dictionary<UInt64, IJob>();
        }

        public void Initialize(UInt32 instanceId)
        {
            _extraNonce = new ExtraNonce(instanceId);
        }

        public IJob GetJob(UInt64 id)
        {
            return Jobs.ContainsKey(id) ? Jobs[id] : null;
        }

        public void AddJob(IJob job)
        {
            Jobs.Add(job.Id, job);
        }

        /// <summary>
        /// Broadcasts to miners.
        /// </summary>
        /// <example>
        /// sample communication: http://bitcoin.stackexchange.com/a/23112/8899
        /// </example>
        public void Broadcast()
        {
            var blockTemplate = _daemonClient.GetBlockTemplate();
            var generationTransaction = new GenerationTransaction(ExtraNonce, _daemonClient, blockTemplate);
            generationTransaction.Create();

            // create the difficulty notification.
            var difficulty = new Difficulty(16);

            // create the job notification.
            var job = new Job(JobCounter.Next(), _hashAlgorithm, blockTemplate, generationTransaction)
            {
                CleanJobs = true // tell the miners to clean their existing jobs and start working on new one.
            };

            Jobs.Add(job.Id, job);
            LastJob = job;

            foreach (var miner in _minerManager.GetAll())
            {
                if (!miner.Authenticated)
                    continue;

                if (!miner.Subscribed)
                    continue;

                if (!miner.SupportsJobNotifications)
                    continue;

                miner.SendDifficulty(difficulty);
                miner.SendJob(job);
            }
        }
    }
}
