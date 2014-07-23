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
using System.Net;
using System.Net.Sockets;
using CoiniumServ.Factories;
using CoiniumServ.Payments;
using CoiniumServ.Persistance.Blocks;
using CoiniumServ.Pools.Config;
using CoiniumServ.Shares;
using CoiniumServ.Utils.Extensions;
using CoiniumServ.Utils.Helpers.Time;
using Serilog;
using StackExchange.Redis;

namespace CoiniumServ.Persistance.Redis
{
    public class Redis:IStorage, IRedis
    {
        public bool IsEnabled { get; private set; }
        public bool IsConnected { get { return _connectionMultiplexer.IsConnected; } }

        private readonly Version _requiredMinimumVersion = new Version(2, 6);
        private readonly IRedisConfig _redisConfig;
        private readonly IPoolConfig _poolConfig;

        private ConnectionMultiplexer _connectionMultiplexer;
        private IDatabase _database;
        private IServer _server;

        private readonly ILogger _logger;

        public Redis(PoolConfig poolConfig)
        {
            _logger = Log.ForContext<Redis>().ForContext("Component", poolConfig.Coin.Name);

            _poolConfig = poolConfig; // the pool config.
            _redisConfig = (IRedisConfig) poolConfig.Storage;

            IsEnabled = _redisConfig.Enabled;

            if (IsEnabled)
                Initialize();
        }

        public void AddShare(IShare share)
        {
            if (!IsEnabled || !IsConnected)
                return;

            var coin = _poolConfig.Coin.Name.ToLower(); // the coin we are working on.
            var batch = _database.CreateBatch(); // batch the commands.

            // add the share to round 
            var currentKey = string.Format("{0}:shares:round:current", coin);
            batch.HashIncrementAsync(currentKey, share.Miner.Username, share.Difficulty, CommandFlags.FireAndForget);

            // increment shares stats.
            var statsKey = string.Format("{0}:stats", coin);
            batch.HashIncrementAsync(statsKey, share.IsValid ? "validShares" : "invalidShares", 1, CommandFlags.FireAndForget);

            // add to hashrate
            if (share.IsValid)
            {
                var hashrateKey = string.Format("{0}:hashrate", coin);
                var entry = string.Format("{0}:{1}", share.Difficulty, share.Miner.Username);
                batch.SortedSetAddAsync(hashrateKey, entry, TimeHelpers.NowInUnixTime(), CommandFlags.FireAndForget);
            }

            batch.Execute(); // execute the batch commands.
        }

        public void AddBlock(IShare share)
        {
            if (!IsEnabled || !IsConnected)
                return;

            var coin = _poolConfig.Coin.Name.ToLower(); // the coin we are working on.
            var batch = _database.CreateBatch(); // batch the commands.

            if (share.IsBlockAccepted)
            {
                // rename round.
                var currentKey = string.Format("{0}:shares:round:current", coin);
                var roundKey = string.Format("{0}:shares:round:{1}", coin, share.Height);
                batch.KeyRenameAsync(currentKey, roundKey, When.Always, CommandFlags.HighPriority);

                // add block to pending.
                var pendingKey = string.Format("{0}:blocks:pending", coin);
                var entry = string.Format("{0}:{1}", share.BlockHash.ToHexString(), share.Block.Tx.First());
                batch.SortedSetAddAsync(pendingKey, entry, share.Block.Height, CommandFlags.FireAndForget);
            }

            // increment block stats.
            var statsKey = string.Format("{0}:stats", coin);
            batch.HashIncrementAsync(statsKey, share.IsBlockAccepted ? "validBlocks" : "invalidBlocks", 1, CommandFlags.FireAndForget);

            batch.Execute(); // execute the batch commands.
        }

        public void SetRemainingBalances(IList<IWorkerBalance> workerBalances)
        {
            if (!IsEnabled || !IsConnected)
                return;

            var balancesKey = string.Format("{0}:balances", _poolConfig.Coin.Name.ToLower());
            var batch = _database.CreateBatch(); // batch the commands.

            foreach (var workerBalance in workerBalances)
            {
                batch.HashDeleteAsync(balancesKey, workerBalance.Worker, CommandFlags.FireAndForget); // first delete the existing key.

                if(!workerBalance.Paid) // if outstanding balance exists, commit it.
                    batch.HashIncrementAsync(balancesKey, workerBalance.Worker, (double)workerBalance.Balance, CommandFlags.FireAndForget); // increment the value.
            }            

            batch.Execute(); // execute the batch commands.
        }

        public void DeleteShares(IPaymentRound round)
        {
            if (!IsEnabled || !IsConnected)
                return;

            var coin = _poolConfig.Coin.Name.ToLower(); // the coin we are working on.
            var roundKey = string.Format("{0}:shares:round:{1}", coin, round.Block.Height);

            _database.KeyDeleteAsync(roundKey, CommandFlags.FireAndForget); // delete the associated shares.
        }

        public void MoveSharesToCurrentRound(IPaymentRound round)
        {
            if (!IsEnabled || !IsConnected)
                return;

            if (round.Block.Status == BlockStatus.Confirmed || round.Block.Status == BlockStatus.Pending)
                return;

            var coin = _poolConfig.Coin.Name.ToLower(); // the coin we are working on.
            var currentRound = string.Format("{0}:shares:round:current", coin); // current round key.
            var roundShares = string.Format("{0}:shares:round:{1}", coin, round.Block.Height); // rounds shares key.

            // add shares to current round again.
            foreach (var pair in round.Shares)
            {
                _database.HashIncrementAsync(currentRound, pair.Key, pair.Value, CommandFlags.FireAndForget);
            }

            // delete the associated shares.
            _database.KeyDeleteAsync(roundShares, CommandFlags.FireAndForget); // delete the associated shares.
        }

        public void MoveBlock(IPaymentRound round)
        {
            if (!IsEnabled || !IsConnected)
                return;

            if (round.Block.Status == BlockStatus.Pending)
                return;

            var coin = _poolConfig.Coin.Name.ToLower(); // the coin we are working on.

            // re-flag the rounds.
            var pendingKey = string.Format("{0}:blocks:pending", coin); // old key for the round.
            var newKey = string.Empty; // new key for the round.

            switch (round.Block.Status)
            {
                case BlockStatus.Kicked:
                    newKey = string.Format("{0}:blocks:kicked", coin);
                    break;
                case BlockStatus.Orphaned:
                    newKey = string.Format("{0}:blocks:orphaned", coin);
                    break;
                case BlockStatus.Confirmed:
                    newKey = string.Format("{0}:blocks:confirmed", coin);
                    break;
            }
            
            var entry = string.Format("{0}:{1}", round.Block.BlockHash, round.Block.TransactionHash);   
                 
            _database.SortedSetRemoveRangeByScoreAsync(pendingKey, round.Block.Height, round.Block.Height, Exclude.None, CommandFlags.FireAndForget);
            _database.SortedSetAddAsync(newKey, entry, round.Block.Height, CommandFlags.FireAndForget);
        }

        public IDictionary<string, int> GetBlockCounts()
        {
            var blocks = new Dictionary<string, int>();

            if (!IsEnabled || !IsConnected)
                return blocks;

            var batch = _database.CreateBatch(); // batch the commands.

            var coin = _poolConfig.Coin.Name.ToLower(); // the coin we are working on.

            var pendingKey = string.Format("{0}:blocks:pending", coin);
            var confirmedKey = string.Format("{0}:blocks:confirmed", coin);
            var oprhanedKey = string.Format("{0}:blocks:orphaned", coin);

            var pendingTask = batch.SortedSetLengthAsync(pendingKey);
            var confirmedTask = batch.SortedSetLengthAsync(confirmedKey);
            var orphanedTask = batch.SortedSetLengthAsync(oprhanedKey);

            batch.Execute(); // execute the batch commands.

            blocks["pending"] = (int)pendingTask.Result;
            blocks["confirmed"] = (int)confirmedTask.Result;
            blocks["orphaned"] = (int)orphanedTask.Result;

            return blocks;
        }

        public void DeleteExpiredHashrateData(int until)
        {
            if (!IsEnabled || !IsConnected)
                return;

            var key = string.Format("{0}:hashrate", _poolConfig.Coin.Name.ToLower());

            _database.SortedSetRemoveRangeByScore(key, double.NegativeInfinity, until, Exclude.Stop);
        }

        public IDictionary<string, double> GetHashrateData(int since)
        {
            var hashrates = new Dictionary<string, double>();

            if (!IsEnabled || !IsConnected)
                return hashrates;

            var key = string.Format("{0}:hashrate", _poolConfig.Coin.Name.ToLower());

            var results = _database.SortedSetRangeByScore(key, since);

            foreach (var result in results)
            {
                var data = result.ToString().Split(':');
                var share = double.Parse(data[0].Replace(',','.'), CultureInfo.InvariantCulture);
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

            var task = _database.SortedSetRangeByRankWithScoresAsync(pendingKey, 0, -1, Order.Ascending, CommandFlags.HighPriority);
            var results = task.Result;

            foreach (var result in results)
            {
                var data = result.Element.ToString().Split(':');
                var blockHash = data[0];
                var transactionHash = data[1];
                var hashCandidate = new HashCandidate(blockHash, transactionHash);

                if (!blocks.ContainsKey((UInt32)result.Score))
                    blocks.Add((UInt32)result.Score, new PendingBlock((UInt32)result.Score));

                var persistedBlock = blocks[(UInt32)result.Score];
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

            var task = _database.SortedSetRangeByRankWithScoresAsync(key, 0, -1, Order.Ascending, CommandFlags.HighPriority);
            var results = task.Result;

            foreach (var result in results)
            {
                var data = result.Element.ToString().Split(':');
                var blockHash = data[0];
                var transactionHash = data[1];

                switch (status)
                {
                    case BlockStatus.Kicked:
                        blocks.Add((UInt32) result.Score, new KickedBlock((UInt32) result.Score, blockHash, transactionHash, 0, 0));
                        break;
                    case BlockStatus.Orphaned:
                        blocks.Add((UInt32)result.Score, new OrphanedBlock((UInt32)result.Score, blockHash, transactionHash, 0, 0));
                        break;
                    case BlockStatus.Confirmed:
                        blocks.Add((UInt32)result.Score, new ConfirmedBlock((UInt32)result.Score, blockHash, transactionHash, 0, 0));
                        break;
                }
            }

            return blocks.Values.ToList();
        }

        public IDictionary<UInt32, IPersistedBlock> GetAllBlocks()
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

        public Dictionary<UInt32, Dictionary<string, double>> GetSharesForRounds(IList<IPaymentRound> rounds)
        {
            var sharesForRounds = new Dictionary<UInt32, Dictionary<string, double>>(); // dictionary of block-height <-> shares.

            if (!IsEnabled || !IsConnected)
                return sharesForRounds;

            var coin = _poolConfig.Coin.Name.ToLower(); // the coin we are working on.

            foreach (var round in rounds)
            {
                var roundKey = string.Format("{0}:shares:round:{1}", coin, round.Block.Height);
                var hashes = _database.HashGetAllAsync(roundKey, CommandFlags.HighPriority).Result;

                var shares = hashes.ToDictionary<HashEntry, string, double>(pair => pair.Name, pair => (double)pair.Value);
                sharesForRounds.Add(round.Block.Height, shares);
            }

            return sharesForRounds;
        }

        public Dictionary<string, double> GetPreviousBalances()
        {
            var previousBalances = new Dictionary<string, double>();

            if (!IsEnabled || !IsConnected)
                return previousBalances;

            var key = string.Format("{0}:balances", _poolConfig.Coin.Name.ToLower());
            var hashes = _database.HashGetAllAsync(key, CommandFlags.HighPriority).Result;

            previousBalances = hashes.ToDictionary<HashEntry, string, double>(pair => pair.Name, pair => (double)pair.Value);

            return previousBalances;
        }

        private void Initialize()
        {
            var options = new ConfigurationOptions()
            {
                AllowAdmin = true,
                ConnectTimeout = 1000,
            };

            var endpoint = new DnsEndPoint(_redisConfig.Host, _redisConfig.Port, AddressFamily.InterNetwork);
            options.EndPoints.Add(endpoint);

            if (!string.IsNullOrEmpty(_redisConfig.Password))
                options.Password = _redisConfig.Password;

            try
            {
                // create the connection
                _connectionMultiplexer = ConnectionMultiplexer.ConnectAsync(options).Result;

                // access to database.
                _database = _connectionMultiplexer.GetDatabase(_redisConfig.DatabaseId);

                // get the configured server.
                _server = _connectionMultiplexer.GetServer(endpoint);

                // check the version
                var version = GetVersion();
                if (version < _requiredMinimumVersion)
                    throw new Exception(string.Format("You are using redis version {0}, minimum required version is 2.6", version));

                _logger.Information("Storage initialized: {0:l}:{1}, v{2:l}.", endpoint.Host, endpoint.Port, version);
            }
            catch (Exception e)
            {
                IsEnabled = false;
                _logger.Error(string.Format("Storage initialization failed: {0:l}:{1}.", endpoint.Host, endpoint.Port));
            }
        }

        private Version GetVersion()
        {
            Version version = null;
            var info = _server.Info();

            foreach (var pair in info[0])
            {
                if (pair.Key != "redis_version")
                    continue;

                version = new Version(pair.Value);
                break;
            }

            return version;
        }
    }
}
