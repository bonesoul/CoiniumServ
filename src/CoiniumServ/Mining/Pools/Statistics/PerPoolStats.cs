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
using System.Linq;
using Coinium.Crypto.Algorithms;
using Coinium.Daemon;
using Coinium.Mining.Miners;
using Coinium.Mining.Pools.Config;
using Coinium.Persistance;
using Coinium.Utils.Helpers.Time;

namespace Coinium.Mining.Pools.Statistics
{
    public class PerPoolStats:IPerPoolStats
    {       
        public ulong Hashrate { get; private set; }
        public ulong NetworkHashrate { get; private set; }
        public int WorkerCount { get; private set; }
        public double Difficulty { get; private set; }
        public int CurrentBlock { get; private set; }
        public IBlockStats LatestBlocks { get; private set; }
        public string Algorithm { get; private set; }

        private readonly IDaemonClient _daemonClient;
        private readonly IStorage _storage;
        private readonly IHashAlgorithm _hashAlgorithm;
        private readonly IMinerManager _minerManager;
        private readonly IPoolConfig _poolConfig;

        private readonly double _shareMultiplier;
        private const int HashrateWindow = 300; /* How many seconds worth of shares should be gathered to generate hashrate. */

        public PerPoolStats(IPoolConfig poolConfig, IDaemonClient daemonClient,IMinerManager minerManager, IHashAlgorithm hashAlgorithm, IBlockStats blockStatistics, IStorage storage)
        {
            _poolConfig = poolConfig;
            _daemonClient = daemonClient;            
            _hashAlgorithm = hashAlgorithm;
            _minerManager = minerManager;
            LatestBlocks = blockStatistics;
            _storage = storage;

            _shareMultiplier = Math.Pow(2, 32) / _hashAlgorithm.Multiplier;
        }

        public void Recache(object state)
        {
            ReadCoinData();
            ReadHashrate();

            WorkerCount = _minerManager.Miners.Count;
            Algorithm = _poolConfig.Coin.Algorithm;

            LatestBlocks.Recache(state);
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
