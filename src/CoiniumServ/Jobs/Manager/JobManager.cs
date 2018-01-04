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
using System.Threading;
using CoiniumServ.Algorithms;
using CoiniumServ.Daemon;
using CoiniumServ.Daemon.Exceptions;
using CoiniumServ.Jobs.Tracker;
using CoiniumServ.Mining;
using CoiniumServ.Pools;
using CoiniumServ.Server.Mining.Getwork;
using CoiniumServ.Server.Mining.Stratum;
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

        private readonly IPoolConfig _poolConfig;

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
            _poolConfig = poolConfig;
            
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
            _logger.Verbose("As we have just found a new block, rebroadcasting new work");
            CreateAndBroadcastNewJob(false);
        }

        private void IdleJobTimer(object state)
        {
            _logger.Verbose("As idle job timer expired, rebroadcasting new work");
            CreateAndBroadcastNewJob(true);
        }

        private void BlockPoller(object stats)
        {
            if (_jobTracker.Current == null) // make sure we already have succesfully created a previous job.
                return; // else just skip block-polling until we do so.

            try
            {
                var blockTemplate = _daemonClient.GetBlockTemplate(_poolConfig.Coin.Options.BlockTemplateModeRequired);

                if (blockTemplate.Height == _jobTracker.Current.Height) // if network reports the same block-height with our current job.
                    return; // just return.
                
                _logger.Verbose("A new block {0} emerged in network, rebroadcasting new work", blockTemplate.Height);
                CreateAndBroadcastNewJob(false); // broadcast a new job.
            }
            catch (RpcException) { } // just skip any exceptions caused by the block-pooler queries.

            _blockPollerTimer.Change(_poolConfig.Job.BlockRefreshInterval, Timeout.Infinite); // reset the block-poller timer so we can keep polling.
        }

        private void CreateAndBroadcastNewJob(bool initiatedByTimer)
        {
            IJob job = null;

            for (var i = 0; i < 3; i++) // try creating a new job 5 times at least.
            {
                job = GetNewJob(); // create a new job.

                if (job != null)
                    break;
            }

            if (job != null) // if we were able to create a new job
            {
                var count = BroadcastJob(job); // broadcast to miners.  

                _blockPollerTimer.Change(_poolConfig.Job.BlockRefreshInterval, Timeout.Infinite); // reset the block-poller timer so we can start or keep polling for a new block in the network.

                if (initiatedByTimer)
                    _logger.Information("Broadcasted new job 0x{0:x} to {1} subscribers as no new blocks found for last {2} seconds", job.Id, count, _poolConfig.Job.RebroadcastTimeout);
                else
                    _logger.Information("Broadcasted new job 0x{0:x} to {1} subscribers as network found a new block", job.Id, count);
            }

            // no matter we created a job successfully or not, reset the rebroadcast timer, so we can keep trying. 
            _reBroadcastTimer.Change(_poolConfig.Job.RebroadcastTimeout * 1000, Timeout.Infinite);
        }

        private IJob GetNewJob()
        {
            try
            {
                var blockTemplate = _daemonClient.GetBlockTemplate(_poolConfig.Coin.Options.BlockTemplateModeRequired);

                // TODO: convert generation transaction to ioc & DI based.
                var generationTransaction = new GenerationTransaction(ExtraNonce, _daemonClient, blockTemplate, _poolConfig);
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
            if (miner is IGetworkMiner) // only stratum miners needs to be submitted new jobs.
                return false;

            var stratumMiner = (IStratumMiner) miner;

            if (!stratumMiner.Authenticated)
                return false;

            if (!stratumMiner.Subscribed)
                return false;

            stratumMiner.SendJob(job);

            return true;
        }

        private void OnMinerAuthenticated(object sender, EventArgs e)
        {
            var miner = ((MinerEventArgs)e).Miner;

            if (miner == null)
                return;

            if (_jobTracker.Current != null) // if we have a valid job,
                SendJobToMiner(miner, _jobTracker.Current); // send it to newly connected miner.
        }
    }
}
