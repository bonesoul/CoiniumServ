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
using CoiniumServ.Pools.Config;
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

        private readonly IJobConfig _jobConfig;

        private readonly IWalletConfig _walletConfig;

        private readonly IRewardsConfig _rewardsConfig;

        private IExtraNonce _extraNonce; // todo: check this.

        private readonly ILogger _logger;

        public IExtraNonce ExtraNonce { get { return _extraNonce; } }

        private Timer _reBroadcastTimer; // timer for rebroadcasting jobs after an pre-configured idle perioud.

        private Timer _blockPollerTimer; // timer for polling new blocks.

        public JobManager(IPoolConfig poolConfig, IDaemonClient daemonClient, IJobTracker jobTracker, IShareManager shareManager,
            IMinerManager minerManager, IHashAlgorithm hashAlgorithm)
        {
            _daemonClient = daemonClient;
            _jobTracker = jobTracker;
            _shareManager = shareManager;
            _minerManager = minerManager;
            _hashAlgorithm = hashAlgorithm;

            _jobConfig = poolConfig.Job;
            _walletConfig = poolConfig.Wallet;
            _rewardsConfig = poolConfig.Rewards;
            
            _jobCounter = new JobCounter(); // todo make this ioc based too.

            _logger = Log.ForContext<JobManager>().ForContext("Component", poolConfig.Coin.Name);
        }

        public void Initialize(UInt32 instanceId)
        {
            _extraNonce = new ExtraNonce(instanceId);
            _shareManager.BlockFound += OnBlockFound;
            _minerManager.MinerAuthenticated += OnMinerAuthenticated;

            // create the timers as disabled.
            _reBroadcastTimer = new Timer(IdleJobTimer, null,Timeout.Infinite, Timeout.Infinite);
            _blockPollerTimer = new Timer(BlockPoller, null, Timeout.Infinite, Timeout.Infinite);

            CreateAndBroadcastNewJob(true); // broadcast a new job initially - which will also setup the timers.
        }

        private void OnBlockFound(object sender, EventArgs e)
        {
            CreateAndBroadcastNewJob(false);
        }

        private void IdleJobTimer(object state)
        {
            CreateAndBroadcastNewJob(true);
        }

        private void BlockPoller(object stats)
        {
            if (_jobTracker.Current == null) // make sure we already have succesfully created a previous job.
                return; // else just skip block-polling until we do so.

            try
            {
                var blockTemplate = _daemonClient.GetBlockTemplate();

                if (blockTemplate.Height != _jobTracker.Current.Height) // if network reports a new block-height
                {
                    _logger.Verbose("A new block {0} emerged in network, rebroadcasting new work", blockTemplate.Height);
                    CreateAndBroadcastNewJob(false); // broadcast a new job.
                }
                else
                    _logger.Verbose("No new blocks found in network, current job: 0x{0:x} height: {1}, network height: {2}", _jobTracker.Current.Id, _jobTracker.Current.Height, blockTemplate.Height);
            }
            catch (RpcException) { } // just skip any exceptions caused by the block-pooler queries.

            _blockPollerTimer.Change(_jobConfig.BlockRefreshInterval, Timeout.Infinite); // reset the block-poller timer so we can keep polling.
        }

        private void OnMinerAuthenticated(object sender, EventArgs e)
        {
            var miner = ((MinerEventArgs)e).Miner;

            if (miner == null)
                return;

            if(_jobTracker.Current != null) // if we have a valid job,
                SendJobToMiner(miner, _jobTracker.Current); // send it to newly connected miner.
        }

        private void CreateAndBroadcastNewJob(bool initiatedByTimer)
        {            
            var job = GetNewJob(); // create a new job.

            if (job != null) // if we were able to create a new job
            {
                var count = BroadcastJob(job); // broadcast to miners.  

                _blockPollerTimer.Change(_jobConfig.BlockRefreshInterval, Timeout.Infinite); // reset the block-poller timer so we can start or keep polling for a new block in the network.

                if (initiatedByTimer)
                    _logger.Information("Broadcasted new job 0x{0:x} to {1} subscribers as no new blocks found for last {2} seconds.", job.Id, count, _jobConfig.RebroadcastTimeout);
                else
                    _logger.Information("Broadcasted new job 0x{0:x} to {1} subscribers as network found a new block.", job.Id, count);
            }

            // no matter we created a job successfully or not, reset the rebroadcast timer, so we can keep trying. 
            _reBroadcastTimer.Change(_jobConfig.RebroadcastTimeout * 1000, Timeout.Infinite);
        }

        private IJob GetNewJob()
        {
            try
            {
                var blockTemplate = _daemonClient.GetBlockTemplate();

                // TODO: convert generation transaction to ioc & DI based.
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
                _logger.Error("New job creation failed: {0:l}", rpcException.Message);
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
        private Int32 BroadcastJob(IJob job)
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

            // todo: this message should be configurable and optional
            stratumMiner.SendMessage(string.Format("New job 0x{0:x} for next block {1}.", job.Id, job.Height));

            return true;
        }
    }
}
