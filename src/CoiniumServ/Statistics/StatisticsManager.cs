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
        public double Hashrate { get; private set; }

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
