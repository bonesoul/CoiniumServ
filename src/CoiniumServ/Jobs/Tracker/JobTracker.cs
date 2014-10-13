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
