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
using System.Threading;
using CoiniumServ.Cryptology.Algorithms;
using CoiniumServ.Daemon;
using CoiniumServ.Daemon.Exceptions;
using CoiniumServ.Jobs.Tracker;
using CoiniumServ.Miners;
using CoiniumServ.Payments;
using CoiniumServ.Server.Mining.Stratum;
using CoiniumServ.Server.Mining.Stratum.Notifications;
using CoiniumServ.Server.Mining.Vanilla;
using CoiniumServ.Shares;
using CoiniumServ.Transactions;
using Serilog;

namespace CoiniumServ.Jobs.Manager
{
    public class JobManager : IJobManager
    {
        private readonly IDaemonClient _daemonClient;

        private readonly IJobTracker _jobTracker;

        private readonly IShareManager _shareManager;

        private readonly IMinerManager _minerManager;

        private readonly IHashAlgorithm _hashAlgorithm;

        private readonly IJobCounter _jobCounter;

        private readonly IWalletConfig _walletConfig;

        private readonly IRewardsConfig _rewardsConfig;

        private IExtraNonce _extraNonce; // todo: check this.

        private readonly ILogger _logger;

        public IExtraNonce ExtraNonce { get { return _extraNonce; } }


        private Timer _timer;
        private const int TimerExpiration = 60;

        public JobManager(string pool, IDaemonClient daemonClient, IJobTracker jobTracker, IShareManager shareManager,
            IMinerManager minerManager, IHashAlgorithm hashAlgorithm, IWalletConfig walletConfig,
            IRewardsConfig rewardsConfig)
        {
            _daemonClient = daemonClient;
            _jobTracker = jobTracker;
            _shareManager = shareManager;
            _minerManager = minerManager;
            _hashAlgorithm = hashAlgorithm;
            _walletConfig = walletConfig;
            _rewardsConfig = rewardsConfig;
            _jobCounter = new JobCounter(); // todo make this ioc based too.

            _logger = Log.ForContext<JobManager>().ForContext("Component", pool);
        }

        public void Initialize(UInt32 instanceId)
        {
            _extraNonce = new ExtraNonce(instanceId);
            _shareManager.BlockFound += OnBlockFound;
            _minerManager.MinerAuthenticated += OnMinerAuthenticated;

            _timer = new Timer(IdleJobTimer, null,Timeout.Infinite, Timeout.Infinite); // create the timer as disabled.
            BroadcastNewJob(true); // broadcast a new job initially - which will also setup the timer.
        }

        private void OnBlockFound(object sender, EventArgs e)
        {
            BroadcastNewJob(false);
        }

        private void IdleJobTimer(object state)
        {
            BroadcastNewJob(true);
        }

        private void BroadcastNewJob(bool initiatedByTimer)
        {
            var job = GetNewJob(); // create a new job.
            var count = Broadcast(job); // broadcast to miners.  

            if (job == null)
                return;

            if (initiatedByTimer)
                _logger.Information("Broadcasted job 0x{0:x} to {1} subscribers as no new blocks found for last {2} seconds.", job.Id, count, TimerExpiration);
            else
                _logger.Information("Broadcasted job 0x{0:x} to {1} subscribers as we have just found a new block.", job.Id, count);

            _timer.Change(TimerExpiration * 1000, Timeout.Infinite); // reset the idle-block timer.
        }

        private void OnMinerAuthenticated(object sender, EventArgs e)
        {
            var miner = ((MinerEventArgs) e).Miner;

            if (miner == null) 
                return;

            var result = SendJobToMiner(miner, _jobTracker.Current);
        }

        private IJob GetNewJob()
        {
            try
            {
                // TODO: fix the error for [Error] [JobManager] [n/a] New job creation failed:
                // Coinium.Daemon.Exceptions.RpcException: Dogecoin is downloading blocks...

                var blockTemplate = _daemonClient.GetBlockTemplate();

                // TODO: convert generation transaction to ioc based.
                var generationTransaction = new GenerationTransaction(ExtraNonce, _daemonClient, blockTemplate,_walletConfig, _rewardsConfig);
                generationTransaction.Create();

                // create the job notification.
                var job = new Job(_jobCounter.Next(), _hashAlgorithm, blockTemplate, generationTransaction)
                {
                    CleanJobs = true // tell the miners to clean their existing jobs and start working on new one.
                };

                _jobTracker.Add(job);

                return job;
            }
            catch (RpcException rpcException)
            {
                _logger.Error(rpcException, "New job creation failed:");
                return null;
            }
            catch (Exception e)
            {
                _logger.Error(e, "New job creation failed:");
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

                foreach (var miner in _minerManager.Miners)
                {
                    var success = SendJobToMiner(miner, job);

                    if (success)
                        count++;
                }

                return count;
            }
            catch (Exception e)
            {
                _logger.Error(e, "Job broadcast failed:");
                return 0;
            }
        }

        private bool SendJobToMiner(IMiner miner, IJob job)
        {
            if (miner is IVanillaMiner) // only stratum miners needs to be submitted new jobs.
                return false;

            var stratumMiner = (IStratumMiner) miner;

            if (!stratumMiner.Authenticated)
                return false;

            if (!stratumMiner.Subscribed)
                return false;

            stratumMiner.SendJob(job);

            return true;
        }
    }
}
