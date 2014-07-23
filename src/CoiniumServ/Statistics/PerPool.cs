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
using System.Dynamic;
using System.Linq;
using CoiniumServ.Cryptology.Algorithms;
using CoiniumServ.Daemon;
using CoiniumServ.Miners;
using CoiniumServ.Persistance;
using CoiniumServ.Pools.Config;
using CoiniumServ.Utils.Helpers.Time;
using Newtonsoft.Json;

namespace CoiniumServ.Statistics
{
    public class PerPool:IPerPool
    {       
        public ulong Hashrate { get; private set; }
        public ulong NetworkHashrate { get; private set; }
        public int WorkerCount { get; private set; }
        public double Difficulty { get; private set; }
        public int CurrentBlock { get; private set; }
        public IBlocks Blocks { get; private set; }
        public IPoolConfig Config { get; private set; }

        public string Json { get; private set; }

        private readonly IDaemonClient _daemonClient;
        private readonly IStorage _storage;
        private readonly IMinerManager _minerManager;
        private readonly dynamic _response;

        private readonly double _shareMultiplier;
        private const int HashrateWindow = 300; /* How many seconds worth of shares should be gathered to generate hashrate. */

        public PerPool(IPoolConfig poolConfig, IDaemonClient daemonClient,IMinerManager minerManager, IHashAlgorithm hashAlgorithm, IBlocks blockStatistics, IStorage storage)
        {
            Config = poolConfig;
            _daemonClient = daemonClient;
            _minerManager = minerManager;
            Blocks = blockStatistics;
            _storage = storage;

            _response = new ExpandoObject();
            _shareMultiplier = Math.Pow(2, 32) / hashAlgorithm.Multiplier;
        }

        public void Recache(object state)
        {
            // recache data.
            WorkerCount = _minerManager.Miners.Count;

            ReadCoinData();
            ReadHashrate();
            Blocks.Recache(state);

            // recache json response.
            _response.workers = WorkerCount;
            _response.hashrate = Hashrate;

            _response.coin = new ExpandoObject();
            _response.coin.symbol = Config.Coin.Symbol;
            _response.coin.name = Config.Coin.Name;
            _response.coin.algorithm = Config.Coin.Algorithm;

            _response.network = new ExpandoObject();
            _response.network.currentBlock = CurrentBlock;
            _response.network.difficulty = Difficulty;
            _response.network.hashrate = NetworkHashrate;

            Json = JsonConvert.SerializeObject(_response);
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

        public object GetResponseObject()
        {
            return _response;
        }
    }
}
