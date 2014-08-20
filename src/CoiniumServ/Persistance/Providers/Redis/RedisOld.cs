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
using CoiniumServ.Pools;
using CoiniumServ.Shares;
using CoiniumServ.Utils.Extensions;
using CoiniumServ.Utils.Helpers.Time;
using CSRedis;
using Serilog;

namespace CoiniumServ.Persistance.Providers.Redis
{
    /// <summary>
    /// CSRedis based redis client.
    /// </summary>
    public class RedisOld:IStorageOld, IRedisOld
    {
        public bool IsEnabled { get; private set; }
        public bool IsConnected { get { return _client != null && _client.Connected; } }

        private readonly Version _requiredMinimumVersion = new Version(2, 6);
        private readonly IRedisOldConfig _redisConfig;
        private readonly IPoolConfig _poolConfig;

        private readonly string _coin;

        private RedisClient _client;

        private readonly ILogger _logger;

        public RedisOld(PoolConfig poolConfig)
        {
            _logger = Log.ForContext<RedisOld>().ForContext("Component", poolConfig.Coin.Name);

            _poolConfig = poolConfig; // the pool config.
            _redisConfig = (IRedisOldConfig)poolConfig.StorageOld; // redis config.
            _coin = _poolConfig.Coin.Name.ToLower(); // pool's associated coin name.

            IsEnabled = _redisConfig.Enabled;

            if (IsEnabled)
                Initialize();
        }

        public void AddShare(IShare share)
        {
            try
            {
                if (!IsEnabled || !IsConnected)
                    return;

                //_client.StartPipe(); // batch the commands.

                // add the share to round 
                var currentKey = string.Format("{0}:shares:round:current", _coin);
                _client.HIncrByFloat(currentKey, share.Miner.Username, share.Difficulty);

                // increment shares stats.
                var statsKey = string.Format("{0}:stats", _coin);
                _client.HIncrBy(statsKey, share.IsValid ? "validShares" : "invalidShares", 1);

                // add to hashrate
                if (share.IsValid)
                {
                    var hashrateKey = string.Format("{0}:hashrate", _coin);
                    var entry = string.Format("{0}:{1}", share.Difficulty, share.Miner.Username);
                    _client.ZAdd(hashrateKey, Tuple.Create(TimeHelpers.NowInUnixTime(), entry));
                }

                //_client.EndPipe(); // execute the batch commands.
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while comitting share: {0:l}", e.Message);
            }
        }

        public void AddBlock(IShare share)
        {
            try
            {
                if (!IsEnabled || !IsConnected)
                    return;

                //_client.StartPipe(); // batch the commands.

                if (share.IsBlockAccepted)
                {
                    // rename round.
                    var currentKey = string.Format("{0}:shares:round:current", _coin);
                    var roundKey = string.Format("{0}:shares:round:{1}", _coin, share.Height);
                    _client.Rename(currentKey, roundKey);

                    // add block to pending.
                    var pendingKey = string.Format("{0}:blocks:pending", _coin);
                    var entry = string.Format("{0}:{1}:{2}", share.BlockHash.ToHexString(), share.Block.Tx.First(), share.GenerationTransaction.TotalAmount); // entry format: blockHash:txHash:Amount
                    _client.ZAdd(pendingKey, Tuple.Create(share.Block.Height, entry));
                }

                // increment block stats.
                var statsKey = string.Format("{0}:stats", _coin);
                _client.HIncrBy(statsKey, share.IsBlockAccepted ? "validBlocks" : "invalidBlocks", 1);

                //_client.EndPipe(); // execute the batch commands.
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while adding block: {0:l}", e.Message);
            }
        }

        public void SetRemainingBalances(IList<IWorkerBalance> workerBalances)
        {
            try
            {
                if (!IsEnabled || !IsConnected)
                    return;

                var balancesKey = string.Format("{0}:balances", _coin);

                foreach (var workerBalance in workerBalances)
                {
                    //_client.StartPipeTransaction(); // batch the commands as atomic.

                    _client.HDel(balancesKey, workerBalance.Worker); // first delete the existing key.

                    if (!workerBalance.Paid) // if outstanding balance exists, commit it.
                        _client.HIncrByFloat(balancesKey, workerBalance.Worker, (double) workerBalance.Balance); // increment the value.

                    //_client.EndPipe(); // execute the batch commands.
                }                
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while setting remaining balance: {0:l}", e.Message);
            }
        }

        public void DeleteShares(IPaymentRound round)
        {
            try
            {
                if (!IsEnabled || !IsConnected)
                    return;

                var roundKey = string.Format("{0}:shares:round:{1}", _coin, round.Block.Height);

                _client.Del(roundKey); // delete the associated shares.            
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while deleting shares: {0:l}", e.Message);
            }
        }

        public void MoveSharesToCurrentRound(IPaymentRound round)
        {
            try
            {
                if (!IsEnabled || !IsConnected)
                    return;

                if (round.Block.Status == BlockStatus.Confirmed || round.Block.Status == BlockStatus.Pending)
                    return;

                var currentRound = string.Format("{0}:shares:round:current", _coin); // current round key.
                var roundShares = string.Format("{0}:shares:round:{1}", _coin, round.Block.Height); // rounds shares key.

                //_client.StartPipeTransaction(); // batch the commands as atomic.

                // add shares to current round again.
                foreach (var pair in round.Shares)
                {
                    _client.HIncrByFloat(currentRound, pair.Key, pair.Value);
                }

                _client.Del(roundShares); // delete the associated shares.

                //_client.EndPipe(); // execute the batch commands.
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while moving shares: {0:l}", e.Message);
            }
        }

        public void MoveBlock(IPaymentRound round)
        {
            try
            {
                if (!IsEnabled || !IsConnected)
                    return;

                if (round.Block.Status == BlockStatus.Pending)
                    return;

                // re-flag the rounds.
                var pendingKey = string.Format("{0}:blocks:pending", _coin); // old key for the round.
                var newKey = string.Empty; // new key for the round.

                switch (round.Block.Status)
                {
                    case BlockStatus.Orphaned:
                        newKey = string.Format("{0}:blocks:orphaned", _coin);
                        break;
                    case BlockStatus.Confirmed:
                        newKey = string.Format("{0}:blocks:confirmed", _coin);
                        break;
                }

                var entry = string.Format("{0}:{1}:{2}", round.Block.BlockHash, round.Block.TransactionHash, round.Block.Amount); // entry format: blockHash:txHash:Amount

                //_client.StartPipeTransaction(); // batch the commands as atomic.
                _client.ZRemRangeByScore(pendingKey, round.Block.Height, round.Block.Height);
                _client.ZAdd(newKey, Tuple.Create(round.Block.Height, entry));
                //_client.EndPipe(); // execute the batch commands.
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while moving block: {0:l}", e.Message);
            }
        }

        public IDictionary<string, int> GetBlockCounts()
        {
            var blocks = new Dictionary<string, int>();

            try
            {
                if (!IsEnabled || !IsConnected)
                    return blocks;

                var pendingKey = string.Format("{0}:blocks:pending", _coin);
                var confirmedKey = string.Format("{0}:blocks:confirmed", _coin);
                var oprhanedKey = string.Format("{0}:blocks:orphaned", _coin);

                //_client.StartPipe(); // batch the commands as atomic.

                blocks["pending"] = (int) _client.ZCard(pendingKey);
                blocks["confirmed"] = (int)_client.ZCard(confirmedKey);
                blocks["orphaned"] = (int)_client.ZCard(oprhanedKey);

                /*var results = _client.EndPipe(); // execute the batch commands.

                blocks["pending"] = Convert.ToInt32(results[0]);
                blocks["confirmed"] = Convert.ToInt32(results[1]);
                blocks["orphaned"] = Convert.ToInt32(results[2]);*/
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while getting block counts: {0:l}", e.Message);
            }

            return blocks;
        }

        public void DeleteExpiredHashrateData(int until)
        {
            try
            {
                if (!IsEnabled || !IsConnected)
                    return;

                var key = string.Format("{0}:hashrate", _coin);

                _client.ZRemRangeByScore(key, double.NegativeInfinity, until);
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while deleting expired hashrate data: {0:l}", e.Message);
            }
        }

        public IDictionary<string, double> GetHashrateData(int since)
        {
            var hashrates = new Dictionary<string, double>();

            try
            {
                if (!IsEnabled || !IsConnected)
                    return hashrates;

                var key = string.Format("{0}:hashrate", _coin);

                var results = _client.ZRangeByScore(key, since, double.PositiveInfinity);

                foreach (var result in results)
                {
                    var data = result.Split(':');
                    var share = double.Parse(data[0].Replace(',', '.'), CultureInfo.InvariantCulture);
                    var worker = data[1];

                    if (!hashrates.ContainsKey(worker))
                        hashrates.Add(worker, 0);

                    hashrates[worker] += share;
                }
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while getting hashrate data: {0:l}", e.Message);
            }

            return hashrates;
        }

        public IEnumerable<IPersistedBlock> GetBlocks(BlockStatus status)
        {
            var blocks = new Dictionary<UInt32, IPersistedBlock>();

            try
            {
                if (!IsEnabled || !IsConnected)
                    return blocks.Values.ToList();

                var key = string.Empty;

                switch (status)
                {
                    case BlockStatus.Pending:
                        key = string.Format("{0}:blocks:pending", _coin);
                        break;
                    case BlockStatus.Orphaned:
                        key = string.Format("{0}:blocks:orphaned", _coin);
                        break;
                    case BlockStatus.Confirmed:
                        key = string.Format("{0}:blocks:confirmed", _coin);
                        break;
                }

                var results = _client.ZRevRangeByScoreWithScores(key, double.PositiveInfinity, 0, true);

                foreach (var result in results)
                {
                    var item = result.Item1;
                    var score = result.Item2;

                    var data = item.Split(':');
                    var blockHash = data[0];
                    var transactionHash = data[1];
                    var amount = decimal.Parse(data[2]);

                    blocks.Add((UInt32)score, new PersistedBlock(status, (UInt32)score, blockHash, transactionHash, amount));                    
                }
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while getting {0:l} blocks: {1:l}", status.ToString().ToLower(), e.Message);
            }

            return blocks.Values.ToList();
        }

        public IDictionary<uint, IPersistedBlock> GetAllBlocks()
        {
            var blocks = new Dictionary<uint, IPersistedBlock>();

            if (!IsEnabled || !IsConnected)
                return blocks;

            foreach (var block in GetBlocks(BlockStatus.Confirmed))
            {
                blocks.Add(block.Height, block);
            }

            foreach (var block in GetBlocks(BlockStatus.Orphaned))
            {
                blocks.Add(block.Height, block);
            }

            foreach (var block in GetBlocks(BlockStatus.Pending))
            {
                blocks.Add(block.Height, block);
            }

            return blocks;
        }

        public Dictionary<string, double> GetSharesForCurrentRound()
        {
            var shares = new Dictionary<string, double>();

            try
            {
                if (!IsEnabled || !IsConnected)
                    return shares;

                var key = string.Format("{0}:shares:round:{1}", _coin, "current");
                var hashes = _client.HGetAll(key);

                foreach (var hash in hashes)
                {
                    shares.Add(hash.Key, double.Parse(hash.Value, CultureInfo.InvariantCulture));
                }                
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while getting shares for current round", e.Message);
            }

            return shares;
        }

        public Dictionary<uint, Dictionary<string, double>> GetSharesForRounds(IList<IPaymentRound> rounds)
        {
            var sharesForRounds = new Dictionary<UInt32, Dictionary<string, double>>(); // dictionary of block-height <-> shares.

            try
            {
                if (!IsEnabled || !IsConnected)
                    return sharesForRounds;

                foreach (var round in rounds)
                {
                    var roundKey = string.Format("{0}:shares:round:{1}", _coin, round.Block.Height);
                    var hashes = _client.HGetAll(roundKey);

                    var shares = hashes.ToDictionary(x => x.Key, x => double.Parse(x.Value, CultureInfo.InvariantCulture));
                    sharesForRounds.Add(round.Block.Height, shares);
                }
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while getting shares for round: {0:l}", e.Message);
            }

            return sharesForRounds;
        }

        public Dictionary<string, double> GetPreviousBalances()
        {
            var previousBalances = new Dictionary<string, double>();

            try
            {
                if (!IsEnabled || !IsConnected)
                    return previousBalances;

                var key = string.Format("{0}:balances", _coin);
                var hashes = _client.HGetAll(key);
                previousBalances = hashes.ToDictionary(x => x.Key, x => double.Parse(x.Value, CultureInfo.InvariantCulture));
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while getting previous balances: {0:l}", e.Message);
            }

            return previousBalances;
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
                _client.Select(_redisConfig.DatabaseId);

                // authenticate if needed.
                if (!string.IsNullOrEmpty(_redisConfig.Password))
                    _client.Auth(_redisConfig.Password);

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

            try
            {
                var info = _client.Info("server");

                var parts = info.Split(new[] {Environment.NewLine}, StringSplitOptions.None);

                foreach (var part in parts)
                {
                    var data = part.Split(':');

                    if (data[0] != "redis_version")
                        continue;

                    version = new Version(data[1]);
                }
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while getting version info: {0:l}", e.Message);
            }

            return version;
        }
    }
}
