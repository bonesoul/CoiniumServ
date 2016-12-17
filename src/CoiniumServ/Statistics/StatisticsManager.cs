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
using System.Diagnostics;
using System.Threading;
using CoiniumServ.Algorithms;
using CoiniumServ.Configuration;
using CoiniumServ.Pools;
using Newtonsoft.Json;
using Serilog;

namespace CoiniumServ.Statistics
{
    public class StatisticsManager:IStatisticsManager
    {
        public ulong Hashrate { get; private set; }

        public int MinerCount { get; private set; }
        public DateTime LastUpdate { get; private set; }

        public IAlgorithmManager Algorithms { get; private set; }

        public IPoolManager Pools { get; private set; }

        private readonly IStatisticsConfig _config;

        private readonly Timer _recacheTimer; // timer for recaching stastics.

        private readonly Stopwatch _stopWatch = new Stopwatch();

        private readonly ILogger _logger;

        public StatisticsManager(IConfigManager configManager, IPoolManager poolManager, IAlgorithmManager algorithmManager)
        {
            Pools = poolManager;
            Algorithms = algorithmManager;

            _config = configManager.StatisticsConfig;
            _logger = Log.ForContext<StatisticsManager>();

            _recacheTimer = new Timer(Recache, null, Timeout.Infinite, Timeout.Infinite); // create the timer as disabled.
            Recache(null); // recache data initially.
        }

        public string ServiceResponse { get; private set; }

        public void Recache()
        {
            _stopWatch.Start();

            // recache data.
            Pools.Recache();
            Algorithms.Recache();
            RecacheGlobal();            

            LastUpdate = DateTime.Now;

            // cache the json-service response
            ServiceResponse = JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            _logger.Debug("Recached statistics - took {0:0.000} seconds", (float)_stopWatch.ElapsedMilliseconds / 1000);
            _stopWatch.Reset();

            _recacheTimer.Change(_config.UpdateInterval * 1000, Timeout.Infinite); // reset the recache timer.
        }

        private void RecacheGlobal()
        {
            Hashrate = 0;
            MinerCount = 0;

            foreach (var pool in Pools)
            {
                Hashrate += pool.Hashrate;
                if(pool.MinerManager != null)
                    MinerCount += pool.MinerManager.Count;
            }
        }
        
        private void Recache(object state)
        {
            Recache();
        }
    }
}
