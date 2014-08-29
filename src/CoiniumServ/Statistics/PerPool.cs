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
using System.Dynamic;
using System.Linq;
using CoiniumServ.Configuration;
using CoiniumServ.Cryptology.Algorithms;
using CoiniumServ.Daemon;
using CoiniumServ.Daemon.Exceptions;
using CoiniumServ.Miners;
using CoiniumServ.Persistance;
using CoiniumServ.Pools;
using CoiniumServ.Statistics.New;
using CoiniumServ.Utils.Helpers.Time;
using Newtonsoft.Json;

namespace CoiniumServ.Statistics
{
    public class PerPool:IPerPool
    {       
        public ulong Hashrate { get; private set; }
        public ulong NetworkHashrate { get; private set; }
        public int WorkerCount { get; private set; }
        public Dictionary<string, double> Workers { get; private set; } // todo: convert to enumurable object.
        public Dictionary<string, double> CurrentRoundShares { get; private set; } // todo: convert to enumurable object.
        public double Difficulty { get; private set; }
        public int Round { get; private set; }
        public IBlocksCount Blocks { get; private set; }
        public IPoolConfig Config { get; private set; }

        public string Json { get; private set; }

        public string WorkersJson { get; private set; }

        public string CurrentRoundJson { get; private set; }

        private readonly IDaemonClient _daemonClient;
        private readonly IStorageOld _storage;
        private readonly IMinerManager _minerManager;
        private readonly IStatisticsConfig _statisticsConfig;

        private readonly dynamic _response;

        private readonly double _shareMultiplier;

        public PerPool(IPoolConfig poolConfig, IConfigManager configManager, IDaemonClient daemonClient, IMinerManager minerManager, IHashAlgorithm hashAlgorithm, IBlocksCount blockStatistics, IStorageOld storage)
        {
            Config = poolConfig;
            _statisticsConfig = configManager.WebServerConfig.Statistics;
            _daemonClient = daemonClient;
            _minerManager = minerManager;
            _storage = storage;

            Blocks = blockStatistics;
            Workers = new Dictionary<string, double>();              
           
            _response = new ExpandoObject();
            _shareMultiplier = Math.Pow(2, 32) / hashAlgorithm.Multiplier;
        }

        public void Recache(object state)
        {
            // recache data.
            WorkerCount = _minerManager.Miners.Count;

            ReadCoinData(); // read coin data.
            ReadHashrate(); // read hashrate data.
            RecacheWorkers(); // recache the worker data.
            RecacheRound(); // recache the current round shares data.

            Blocks.Recache(state); // recache the blocks.

            RecacheJson();
        }

        private void ReadCoinData()
        {
            try
            {
                var miningInfo = _daemonClient.GetMiningInfo();
                NetworkHashrate = miningInfo.NetworkHashps;
                Difficulty = miningInfo.Difficulty;
                Round = miningInfo.Blocks + 1;
            }
            catch (RpcException)
            {
                NetworkHashrate = 0;
                Difficulty = 0;
                Round = -1;
            }
        }

        private void ReadHashrate()
        {
            // read hashrate stats.
            var windowTime = TimeHelpers.NowInUnixTime() - _statisticsConfig.HashrateWindow;
            _storage.DeleteExpiredHashrateData(windowTime);
            var hashrates = _storage.GetHashrateData(windowTime);

            double total = hashrates.Sum(pair => pair.Value);
            Hashrate = Convert.ToUInt64(_shareMultiplier * total / _statisticsConfig.HashrateWindow);
        }

        private void RecacheWorkers()
        {
            var shares = _storage.GetSharesForCurrentRound(); // get shares for current round

            Workers.Clear();
            foreach (var miner in _minerManager.Miners)
            {
                // skip un-authenticated miners
                if (!miner.Authenticated)
                    continue;

                // miners with same username may get connected multi-times, so make sure we only count them in collection once.
                if (Workers.ContainsKey(miner.Username))                    
                    continue;

                var amount = shares.ContainsKey(miner.Username) ? shares[miner.Username] : 0; // determine amount of shares that belongs to miner.
                Workers.Add(miner.Username, amount); // add miners the shares.
            }

            WorkersJson = JsonConvert.SerializeObject(Workers);
        }

        private void RecacheRound()
        {
            CurrentRoundShares = _storage.GetSharesForCurrentRound();

            CurrentRoundJson = JsonConvert.SerializeObject(CurrentRoundShares);
        }

        private void RecacheJson()
        {
            // recache json response.
            _response.workers = WorkerCount;
            _response.hashrate = Hashrate;

            _response.coin = new ExpandoObject();
            _response.coin.symbol = Config.Coin.Symbol;
            _response.coin.name = Config.Coin.Name;
            _response.coin.algorithm = Config.Coin.Algorithm;

            _response.network = new ExpandoObject();
            _response.network.currentBlock = Round;
            _response.network.difficulty = Difficulty;
            _response.network.hashrate = NetworkHashrate;

            Json = JsonConvert.SerializeObject(_response);
        }

        public object GetResponseObject()
        {
            return _response;
        }
    }
}
