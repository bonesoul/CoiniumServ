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
using System.Collections.Generic;
using System.Threading;
using Coinium.Crypto.Algorithms;
using Coinium.Daemon;
using Coinium.Daemon.Exceptions;
using Coinium.Mining.Jobs.Tracker;
using Coinium.Mining.Miners;
using Coinium.Mining.Shares;
using Coinium.Server.Stratum.Notifications;
using Coinium.Transactions;
using Serilog;

namespace Coinium.Mining.Jobs.Manager
{
    public class JobManager : IJobManager
    {
        private readonly IDaemonClient _daemonClient;

        private readonly IJobTracker _jobTracker;

        private readonly IShareManager _shareManager;

        private readonly IMinerManager _minerManager;

        private readonly IHashAlgorithm _hashAlgorithm;

        private readonly IJobCounter _jobCounter;

        private IExtraNonce _extraNonce;

        public IExtraNonce ExtraNonce { get { return _extraNonce; } }

        /// <summary>
        /// timer for creating new jobs.
        /// </summary>
        private Timer _timer;

        private const int TimerExpiration = 60;

        private readonly TimeSpan _timeSpan = new TimeSpan(0, 0, 0, TimerExpiration);

        public JobManager(IDaemonClient daemonClient, IJobTracker jobTracker, IShareManager shareManager, IMinerManager minerManager, IHashAlgorithm hashAlgorithm)
        {
            _daemonClient = daemonClient;
            _jobTracker = jobTracker;
            _shareManager = shareManager;
            _minerManager = minerManager;
            _hashAlgorithm = hashAlgorithm;
            _jobCounter = new JobCounter();
        }

        public void Initialize(UInt32 instanceId)
        {
            _extraNonce = new ExtraNonce(instanceId);
            _shareManager.BlockFound += OnBlockFound;
            _minerManager.MinerAuthenticated += OnMinerAuthenticated;

            var job = GetNewJob(); // initially create a job.
            _timer = new Timer(NewJobTimer, null, TimeSpan.Zero, _timeSpan); // setup a timer to broadcast jobs.           
        }

        private void OnBlockFound(object sender, EventArgs e)
        {
            var job = GetNewJob(); // create a new job.
            var count = Broadcast(job); // broadcast to miners.  
            Log.ForContext<JobManager>().Information("Broadcasted job 0x{0:x} to {1} subscribers as we have just found a new block.", job.Id, count);
        }

        private void NewJobTimer(object state)
        {
            var job = GetNewJob(); // create a new job.
            var count = Broadcast(job); // broadcast to miners.  
            Log.ForContext<JobManager>().Information("Broadcasted job 0x{0:x} to {1} subscribers as no new blocks found for last {2} seconds.", job.Id, count, TimerExpiration);
        }

        private void OnMinerAuthenticated(object sender, EventArgs e)
        {
            var miner = ((MinerEventArgs) e).Miner;

            if (miner != null)
            {
                var result = SendJobToMiner(miner, _jobTracker.Current);
            }
        }

        private IJob GetNewJob()
        {
            try
            {
                var blockTemplate = _daemonClient.GetBlockTemplate();

                var generationTransaction = new GenerationTransaction(ExtraNonce, _daemonClient, blockTemplate);
                generationTransaction.Create();

                // create the job notification.
                var job = new Job(_jobCounter.Next(), _hashAlgorithm, blockTemplate, generationTransaction)
                {
                    CleanJobs = true // tell the miners to clean their existing jobs and start working on new one.
                };

                _jobTracker.Add(job);

                return job;
            }
            catch (DaemonException daemonException)
            {
                Log.ForContext<JobManager>().Error(daemonException, "Can not read blocktemplate from daemon:");
                return null;
            }
            catch (Exception e)
            {
                Log.ForContext<JobManager>().Error(e, "New job creation failed:");
                return null;
            }
        }

        /// <summary>
        /// Broadcasts to miners.
        /// </summary>
        /// <example>
        /// sample communication: http://bitcoin.stackexchange.com/a/23112/8899
        /// </example>
        private Int32 Broadcast(IJob job)
        {
            try
            {
                var count = 0; // number of subscribers to job is sent.

                foreach (var miner in _minerManager.GetAll())
                {
                    var success = SendJobToMiner(miner, job);

                    if (success)
                        count++;
                }

                _timer.Change(_timeSpan, TimeSpan.Zero); // reset the idle-block timer.

                return count;
            }
            catch (Exception e)
            {
                Log.ForContext<JobManager>().Error(e, "Job broadcast failed:");
                return 0;
            }
        }

        private bool SendJobToMiner(IMiner miner, IJob job)
        {
            if (!miner.Authenticated)
                return false;

            if (!miner.Subscribed)
                return false;

            if (!miner.SupportsJobNotifications)
                return false;

            miner.SendDifficulty();
            miner.SendJob(job);

            return true;
        }
    }
}
