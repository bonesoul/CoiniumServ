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
using System.Globalization;
using System.Linq;
using CoiniumServ.Payments;
using CoiniumServ.Persistance.Blocks;
using CoiniumServ.Pools.Config;
using CoiniumServ.Shares;
using CoiniumServ.Utils.Helpers.Time;
using CoiniumServ.Utils.Extensions;
using CSRedis;
using Serilog;

namespace CoiniumServ.Persistance.Redis
{
    public class Redis3:IStorage, IRedis
    {
        public bool IsEnabled { get; private set; }
        public bool IsConnected { get { return _client != null && _client.Connected; } }

        private readonly Version _requiredMinimumVersion = new Version(2, 6);
        private readonly IRedisConfig _redisConfig;
        private readonly IPoolConfig _poolConfig;

        private RedisClient _client;

        private readonly ILogger _logger;

        public Redis3(PoolConfig poolConfig)
        {
            _logger = Log.ForContext<Redis3>().ForContext("Component", poolConfig.Coin.Name);

            _poolConfig = poolConfig; // the pool config.
            _redisConfig = (IRedisConfig)poolConfig.Storage;

            IsEnabled = _redisConfig.Enabled;

            if (IsEnabled)
                Initialize();
        }


        public void AddShare(IShare share)
        {
            if (!IsEnabled || !IsConnected)
                return;

            var coin = _poolConfig.Coin.Name.ToLower(); // the coin we are working on.

            // add the share to round 
            var currentKey = string.Format("{0}:shares:round:current", coin);
            _client.HIncrByFloatAsync(currentKey, share.Miner.Username, share.Difficulty);

            // increment shares stats.
            var statsKey = string.Format("{0}:stats", coin);
            _client.HIncrByAsync(statsKey, share.IsValid ? "validShares" : "invalidShares", 1);
            
            if (!share.IsValid) 
                return;

            // add to hashrate
            var hashrateKey = string.Format("{0}:hashrate", coin);
            var entry = string.Format("{0}:{1}", share.Difficulty, share.Miner.Username);
            _client.ZAddAsync(hashrateKey, Tuple.Create(TimeHelpers.NowInUnixTime(), entry));
        }

        public void AddBlock(IShare share)
        {
            if (!IsEnabled || !IsConnected)
                return;

            var coin = _poolConfig.Coin.Name.ToLower(); // the coin we are working on.

            if (share.IsBlockAccepted)
            {
                // rename round.
                var currentKey = string.Format("{0}:shares:round:current", coin);
                var roundKey = string.Format("{0}:shares:round:{1}", coin, share.Height);
                _client.RenameNxAsync(currentKey, roundKey);

                // add block to pending.
                var pendingKey = string.Format("{0}:blocks:pending", coin);
                var entry = string.Format("{0}:{1}", share.BlockHash.ToHexString(), share.Block.Tx.First());
                _client.ZAddAsync(pendingKey, Tuple.Create(share.Block.Height, entry));
            }

            // increment block stats.
            var statsKey = string.Format("{0}:stats", coin);
            _client.HIncrByAsync(statsKey, share.IsBlockAccepted ? "validBlocks" : "invalidBlocks", 1);
        }

        public void SetRemainingBalances(IList<IWorkerBalance> workerBalances)
        {
            throw new NotImplementedException();
        }

        public void DeleteShares(IPaymentRound round)
        {
            throw new NotImplementedException();
        }

        public void MoveSharesToCurrentRound(IPaymentRound round)
        {
            throw new NotImplementedException();
        }

        public void MoveBlock(IPaymentRound round)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, int> GetBlockCounts()
        {
            var blocks = new Dictionary<string, int>();

            if (!IsEnabled || !IsConnected)
                return blocks;

            var coin = _poolConfig.Coin.Name.ToLower(); // the coin we are working on.

            var pendingKey = string.Format("{0}:blocks:pending", coin);
            var confirmedKey = string.Format("{0}:blocks:confirmed", coin);
            var oprhanedKey = string.Format("{0}:blocks:orphaned", coin);

            _client.ZCard(pendingKey);
            _client.ZCard(confirmedKey);
            _client.ZCard(oprhanedKey);

            blocks["pending"] = (int)_client.ZCardAsync(pendingKey).Result;
            blocks["confirmed"] = (int)_client.ZCardAsync(confirmedKey).Result;
            blocks["orphaned"] = (int)_client.ZCardAsync(oprhanedKey).Result;

            return blocks;
        }

        public void DeleteExpiredHashrateData(int until)
        {
            if (!IsEnabled || !IsConnected)
                return;

            var key = string.Format("{0}:hashrate", _poolConfig.Coin.Name.ToLower());

            _client.ZRemRangeByScoreAsync(key, double.NegativeInfinity, until);
        }

        public IDictionary<string, double> GetHashrateData(int since)
        {
            var hashrates = new Dictionary<string, double>();

            if (!IsEnabled || !IsConnected)
                return hashrates;

            var key = string.Format("{0}:hashrate", _poolConfig.Coin.Name.ToLower());

            var results = _client.ZRangeByScoreAsync(key, since, double.PositiveInfinity).Result;

            foreach (var result in results)
            {
                var data = result.Split(':');
                var share = double.Parse(data[0].Replace(',', '.'), CultureInfo.InvariantCulture);
                var worker = data[1];

                if (!hashrates.ContainsKey(worker))
                    hashrates.Add(worker, 0);

                hashrates[worker] += share;
            }

            return hashrates;
        }

        public IList<IPendingBlock> GetPendingBlocks()
        {
            var blocks = new Dictionary<UInt32, IPendingBlock>();

            if (!IsEnabled || !IsConnected)
                return blocks.Values.ToList();

            var coin = _poolConfig.Coin.Name.ToLower(); // the coin we are working on.
            var pendingKey = string.Format("{0}:blocks:pending", coin);

            var results = _client.ZRevRangeByScoreWithScoresAsync(pendingKey, -1, 0, true).Result;

            foreach (var result in results)
            {
                var item = result.Item1;
                var score = result.Item2;

                var data = item.Split(':');
                var blockHash = data[0];
                var transactionHash = data[1];
                var hashCandidate = new HashCandidate(blockHash, transactionHash);

                if (!blocks.ContainsKey((UInt32)score))
                    blocks.Add((UInt32)score, new PendingBlock((UInt32)score));

                var persistedBlock = blocks[(UInt32)score];
                persistedBlock.AddHashCandidate(hashCandidate);
            }

            return blocks.Values.ToList();
        }

        private IEnumerable<IFinalizedBlock> GetFinalizedBlocks(BlockStatus status)
        {
            var blocks = new Dictionary<UInt32, IFinalizedBlock>();

            if (!IsEnabled || !IsConnected)
                return blocks.Values.ToList();

            if (status == BlockStatus.Pending)
                throw new Exception("Pending is not a valid finalized block status");

            var coin = _poolConfig.Coin.Name.ToLower(); // the coin we are working on.
            string key = string.Empty;

            switch (status)
            {
                case BlockStatus.Kicked:
                    key = string.Format("{0}:blocks:kicked", coin);
                    break;
                case BlockStatus.Orphaned:
                    key = string.Format("{0}:blocks:orphaned", coin);
                    break;
                case BlockStatus.Confirmed:
                    key = string.Format("{0}:blocks:confirmed", coin);
                    break;
            }



            var results = _client.ZRevRangeByScoreWithScoresAsync(key, -1, 0, true).Result;

            foreach (var result in results)
            {
                var item = result.Item1;
                var score = result.Item2;

                var data = item.Split(':');
                var blockHash = data[0];
                var transactionHash = data[1];

                switch (status)
                {
                    case BlockStatus.Kicked:
                        blocks.Add((UInt32)score, new KickedBlock((UInt32)score, blockHash, transactionHash, 0, 0));
                        break;
                    case BlockStatus.Orphaned:
                        blocks.Add((UInt32)score, new OrphanedBlock((UInt32)score, blockHash, transactionHash, 0, 0));
                        break;
                    case BlockStatus.Confirmed:
                        blocks.Add((UInt32)score, new ConfirmedBlock((UInt32)score, blockHash, transactionHash, 0, 0));
                        break;
                }
            }

            return blocks.Values.ToList();
        }

        public IDictionary<uint, IPersistedBlock> GetAllBlocks()
        {
            var blocks = new Dictionary<uint, IPersistedBlock>();

            if (!IsEnabled || !IsConnected)
                return blocks;

            foreach (var block in GetFinalizedBlocks(BlockStatus.Confirmed))
            {
                blocks.Add(block.Height, block);
            }

            foreach (var block in GetFinalizedBlocks(BlockStatus.Orphaned))
            {
                blocks.Add(block.Height, block);
            }

            foreach (var block in GetFinalizedBlocks(BlockStatus.Kicked))
            {
                blocks.Add(block.Height, block);
            }

            foreach (var block in GetPendingBlocks())
            {
                blocks.Add(block.Height, block);
            }

            return blocks;
        }       

        public Dictionary<uint, Dictionary<string, double>> GetSharesForRounds(IList<IPaymentRound> rounds)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, double> GetPreviousBalances()
        {
            throw new NotImplementedException();
        }

        private void Initialize()
        {
            try
            {
                // create the connection
                _client = new RedisClient(_redisConfig.Host, _redisConfig.Port)
                {
                    ReconnectAttempts = 3,
                    ReconnectTimeout = 200
                };

                // select the database
                _client.SelectAsync(_redisConfig.DatabaseId);

                // authenticate if needed.
                if (!string.IsNullOrEmpty(_redisConfig.Password))
                    _client.AuthAsync(_redisConfig.Password);

                // check the version
                var version = GetVersion();
                if (version < _requiredMinimumVersion)
                    throw new Exception(string.Format("You are using redis version {0}, minimum required version is 2.6", version));

                _logger.Information("Storage initialized: {0:l}:{1}, v{2:l}.", _redisConfig.Host, _redisConfig.Port, version);
            }
            catch (Exception e)
            {
                _logger.Error("Storage initialization failed: {0:l}:{1} - {2:l}", _redisConfig.Host, _redisConfig.Port, e.Message);
            }
        }

        private Version GetVersion()
        {
            Version version = null;
            var info = _client.InfoAsync("server").Result;

            var parts = info.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            foreach (var part in parts)
            {
                var data = part.Split(':');

                if (data[0] != "redis_version")
                    continue;

                version = new Version(data[1]);
            }

            return version;
        }
    }
}
