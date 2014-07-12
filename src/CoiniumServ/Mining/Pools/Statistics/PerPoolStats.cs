using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coinium.Crypto.Algorithms;
using Coinium.Daemon;
using Coinium.Persistance;
using Coinium.Utils.Helpers.Time;
using HashLib;

namespace Coinium.Mining.Pools.Statistics
{
    public class PerPoolStats:IPerPoolStats
    {       
        public ulong Hashrate { get; private set; }
        public ulong NetworkHashrate { get; private set; }
        public double Difficulty { get; private set; }
        public int CurrentBlock { get; private set; }
        public IBlockStats LatestBlocks { get; private set; }

        private readonly IDaemonClient _daemonClient;
        private readonly IStorage _storage;
        private readonly IHashAlgorithm _hashAlgorithm;

        private readonly double _shareMultiplier;
        private const int HashrateWindow = 300; /* How many seconds worth of shares should be gathered to generate hashrate. */

        public PerPoolStats(IDaemonClient daemonClient, IHashAlgorithm hashAlgorithm, IBlockStats blockStatistics, IStorage storage )
        {
            _daemonClient = daemonClient;            
            _hashAlgorithm = hashAlgorithm;
            LatestBlocks = blockStatistics;
            _storage = storage;

            _shareMultiplier = Math.Pow(2, 32) / _hashAlgorithm.Multiplier;
        }

        public void Recache()
        {
            ReadCoinData();
            ReadHashrate();

            LatestBlocks.Recache();
        }

        private void ReadHashrate()
        {
            // read hashrate stats.
            var windowTime = TimeHelpers.NowInUnixTime() - HashrateWindow;
            _storage.DeleteExpiredHashrateData(windowTime);
            var hashrates = _storage.GetHashrateData(windowTime);

            double total = hashrates.Sum(pair => pair.Value);
            Hashrate = Convert.ToUInt64(_shareMultiplier * total / HashrateWindow);            
        }

        private void ReadCoinData()
        {
            var miningInfo = _daemonClient.GetMiningInfo();
            NetworkHashrate = miningInfo.NetworkHashps;
            Difficulty = miningInfo.Difficulty;
            CurrentBlock = miningInfo.Blocks;            
        }
    }
}
