using System;
using System.Diagnostics;
using System.Threading;
using CoiniumServ.Configuration;
using CoiniumServ.Cryptology.Algorithms;
using CoiniumServ.Pools;
using Newtonsoft.Json;
using Serilog;

namespace CoiniumServ.Statistics.New
{
    public class StatisticsManager:IStatisticsManager
    {
        public ulong Hashrate { get; private set; }
        public int MinerCount { get; private set; }
        public DateTime LastUpdate { get; private set; }

        private readonly Timer _recacheTimer; // timer for recaching stastics.

        private readonly Stopwatch _stopWatch = new Stopwatch();

        private readonly IPoolManager _poolManager;

        private readonly IAlgorithmManager _algorithmManager;

        private readonly IStatisticsConfig _config;

        private readonly ILogger _logger;

        public StatisticsManager(IConfigManager configManager, IPoolManager poolManager, IAlgorithmManager algorithmManager)
        {
            _config = configManager.WebServerConfig.Statistics;
            _poolManager = poolManager;
            _algorithmManager = algorithmManager;

            _logger = Log.ForContext<StatisticsManager>();

            _recacheTimer = new Timer(Recache, null, Timeout.Infinite, Timeout.Infinite); // create the timer as disabled.
            Recache(null); // recache data initially.
        }

        public string ServiceResponse { get; private set; }

        public void Recache()
        {
            _stopWatch.Start();

            // recache data.
            _poolManager.Recache();
            _algorithmManager.Recache();
            RecacheGlobal();            

            LastUpdate = DateTime.Now;

            // cache the json-service response
            ServiceResponse = JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            _logger.Debug("Recached statistics - took {0:0.000} seconds", (float)_stopWatch.ElapsedMilliseconds / 1000);
            _stopWatch.Reset();

            _recacheTimer.Change(_config.UpdateInterval * 1000, Timeout.Infinite); // reset the recache timer.
        }
        
        private void Recache(object state)
        {
            Recache();
        }

        private void RecacheGlobal()
        {
            Hashrate = 0;
            MinerCount = 0;

            foreach (var pool in _poolManager)
            {
                Hashrate += pool.Hashrate;
                MinerCount += pool.MinerManager.Count;
            }
        }
    }
}
