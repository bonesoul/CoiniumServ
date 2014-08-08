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
using CoiniumServ.Configuration;
using CoiniumServ.Factories;
using Serilog;

namespace CoiniumServ.Statistics
{
    public class Statistics:IStatistics, IStatisticsProvider
    {
        public IGlobal Global { get; private set; }
        public IAlgorithms Algorithms { get; private set; }
        public IPools Pools { get; private set; }
        public DateTime LastUpdate { get; private set; }

        private readonly Timer _recacheTimer; // timer for recaching stastics.

        private readonly Stopwatch _stopWatch = new Stopwatch();

        private readonly IStatisticsConfig _config;

        private readonly ILogger _logger;

        public Statistics(IConfigManager configManager, IObjectFactory statisticsObjectFactory)
        {
            _config = configManager.WebServerConfig.Statistics;

            Pools = statisticsObjectFactory.GetPoolStats();
            Global = statisticsObjectFactory.GetGlobalStatistics();
            Algorithms = statisticsObjectFactory.GetAlgorithmStatistics();

            _logger = Log.ForContext<Statistics>();

            _recacheTimer = new Timer(Recache, null, Timeout.Infinite, Timeout.Infinite); // create the timer as disabled.
            Recache(null); // recache data initially.
        }

        public void Recache(object state)
        {
            _stopWatch.Start();

            // recache data.
            Pools.Recache(state);
            Algorithms.Recache(state);
            Global.Recache(state);

            LastUpdate = DateTime.Now;

            _logger.Debug("Recached statistics - took {0:0.000} seconds", (float)_stopWatch.ElapsedMilliseconds / 1000);
            _stopWatch.Reset();

            _recacheTimer.Change(_config.UpdateInterval * 1000, Timeout.Infinite); // reset the recache timer.
        }
    }
}
