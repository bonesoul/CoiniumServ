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
using Coinium.Crypto.Algorithms;
using Coinium.Daemon;
using Coinium.Mining.Miners;
using Coinium.Persistance;
using Coinium.Utils.Helpers.Time;

namespace Coinium.Mining.Pools.Statistics
{
    public class PoolStatistics : IPoolStatistics
    {
        public IBlockStatistics Blocks { get; private set; }

        public UInt64 Hashrate { get; private set; }
        public UInt64 NetworkHashrate { get; private set; }
        public double Difficulty { get; private set; }
        public int CurrentBlockHeight { get; private set; }

        public IList<IMiner> Miners
        {
            get { return _minerManager.Miners; }
        }

        private readonly IDaemonClient _daemonClient;
        private readonly IMinerManager _minerManager;
        private readonly IStorage _storage;
        private readonly IHashAlgorithm _hashAlgorithm;

        private readonly Timer _timer;
        private const int TimerExpiration = 10;
        private const int HashrateWindow = 300; /* How many seconds worth of shares should be gathered to generate hashrate. */

        private readonly double _shareMultiplier;

        public PoolStatistics(IDaemonClient daemonClient, IBlockStatistics blockStatistics, IMinerManager minerManager, IStorage storage, IHashAlgorithm hashAlgorithm)
        {
            _daemonClient = daemonClient;
            _minerManager = minerManager;
            _storage = storage;
            _hashAlgorithm = hashAlgorithm;

            _shareMultiplier = Math.Pow(2, 32)/_hashAlgorithm.Multiplier;

            Blocks = blockStatistics;

            _timer = new Timer(Recache, null, Timeout.Infinite, Timeout.Infinite); // create the timer as disabled.
            Recache(null); // recache data initially.
        }

        private void Recache(object state)
        {
            // read hashrate stats.
            var windowTime = TimeHelpers.NowInUnixTime() - HashrateWindow;
            _storage.DeleteExpiredHashrateData(windowTime);
            var hashrates = _storage.GetHashrateData(windowTime);

            double total = hashrates.Sum(pair => pair.Value);
            Hashrate = Convert.ToUInt64(_shareMultiplier*total/HashrateWindow);

            // read data from daemon
            var miningInfo = _daemonClient.GetMiningInfo();
            NetworkHashrate = miningInfo.NetworkHashps;
            Difficulty = miningInfo.Difficulty;
            CurrentBlockHeight = miningInfo.Blocks;

            // reset the recache timer.
            _timer.Change(TimerExpiration * 1000, Timeout.Infinite);
        }
    }
}
