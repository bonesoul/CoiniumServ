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
using System.Linq;
using System.Threading;
using CoiniumServ.Pools;
using CoiniumServ.Utils.Helpers;
using Serilog;

namespace CoiniumServ.Jobs.Tracker
{
    public class JobTracker:IJobTracker
    {
        private Dictionary<UInt64, IJob> _jobs;

        private readonly Timer _cleanupTimer; // timer for cleaning old jobs.

        private readonly ILogger _logger;

        public IJob Current { get; private set; }

        private const int MinimumJobBacklog = 3; // number of jobs to leave in. 
        private readonly int _cleanupFrequency; // frequency to cleanup jobs in seconds.

        public JobTracker(IPoolConfig poolConfig)
        {
            _jobs = new Dictionary<UInt64, IJob>();
            _logger = Log.ForContext<JobTracker>().ForContext("Component", poolConfig.Coin.Name);

            _cleanupFrequency = MinimumJobBacklog*poolConfig.Job.RebroadcastTimeout; // calculate the cleanup frequency = number of jobs in backlog * rebroad-timeout
            _cleanupTimer = new Timer(CleanUp, null, Timeout.Infinite, Timeout.Infinite); // create the timer as disabled.
            _cleanupTimer.Change(_cleanupFrequency * 1000, Timeout.Infinite); // adjust the timer's next run.
        }

        public IJob Get(UInt64 id)
        {
            return _jobs.ContainsKey(id) ? _jobs[id] : null;
        }

        public void Add(IJob job)
        {
            _jobs.Add(job.Id, job);
            Current = job;
        }

        private void CleanUp(object state)
        {
            var startingCount = _jobs.Count;

            // calculate the cleanup delta time - jobs created before this will be cleaned up.
            var delta = TimeHelpers.NowInUnixTimestamp() - _cleanupFrequency;

            // find expired jobs that were created before our calcualted delta time.
            _jobs = _jobs.Where(j => j.Value.CreationTime >= delta || j.Value == Current)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            var cleanedCount = startingCount - _jobs.Count;

            if(cleanedCount > 0)
                _logger.Debug("Cleaned-up {0} expired jobs", cleanedCount);

            _cleanupTimer.Change(_cleanupFrequency * 1000, Timeout.Infinite); // reset the cleanup timer.
        }
    }
}
