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
using CoiniumServ.Accounts;
using CoiniumServ.Daemon;
using CoiniumServ.Daemon.Exceptions;
using CoiniumServ.Miners;
using CoiniumServ.Payments.Processor;
using CoiniumServ.Payments.Round;
using CoiniumServ.Persistance.Blocks;
using CoiniumServ.Persistance.Providers;
using CoiniumServ.Persistance.Providers.MySql;
using CoiniumServ.Persistance.Providers.Redis;
using CoiniumServ.Pools;
using CoiniumServ.Server.Mining.Stratum;
using CoiniumServ.Shares;
using CoiniumServ.Utils.Extensions;
using CoiniumServ.Utils.Helpers.Time;
using Dapper;
using MySql.Data.MySqlClient;
using Serilog;

namespace CoiniumServ.Persistance.Layers.Hybrid
{
    public class HybridStorageLayer : IStorageLayer
    {
        public bool IsEnabled { get; private set; }

        private readonly IDaemonClient _daemonClient;

        private readonly ILogger _logger;

        private readonly IRedisProvider _redisProvider;

        private readonly IMySqlProvider _mySqlProvider;

        private readonly string _coin;

        public HybridStorageLayer(IEnumerable<IStorageProvider> providers, IDaemonClient daemonClient, IPoolConfig poolConfig)
        {
            _daemonClient = daemonClient;
            _logger = Log.ForContext<HybridStorageLayer>().ForContext("Component", poolConfig.Coin.Name);
            _coin = poolConfig.Coin.Name.ToLower(); // pool's associated coin name.

            // try loading providers.
            try
            {
                foreach (var provider in providers)
                {
                    if (provider is IRedisProvider)
                        _redisProvider = (IRedisProvider) provider;
                    else if (provider is IMySqlProvider)
                        _mySqlProvider = (IMySqlProvider) provider;
                }

                IsEnabled = (_redisProvider != null && _mySqlProvider != null);
            }
            catch (Exception e)
            {
                _logger.Error("Error initializing hybrid storage layer; {0:l}", e.Message);
            }
        }

        public void AddShare(IShare share)
        {
            try
            {
                if (!IsEnabled || !_redisProvider.IsConnected)
                    return;

                //_client.StartPipe(); // batch the commands.

                // add the share to round 
                var currentKey = string.Format("{0}:shares:round:current", _coin);
                _redisProvider.Client.HIncrByFloat(currentKey, share.Miner.Username, share.Difficulty);

                // increment shares stats.
                var statsKey = string.Format("{0}:stats", _coin);
                _redisProvider.Client.HIncrBy(statsKey, share.IsValid ? "validShares" : "invalidShares", 1);

                // add to hashrate
                if (share.IsValid)
                {
                    var hashrateKey = string.Format("{0}:hashrate", _coin);
                    var entry = string.Format("{0}:{1}", share.Difficulty, share.Miner.Username);
                    _redisProvider.Client.ZAdd(hashrateKey, Tuple.Create(TimeHelpers.NowInUnixTime(), entry));
                }

                //_client.EndPipe(); // execute the batch commands.
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while comitting share: {0:l}", e.Message);
            }
        }

        public void RemoveShares(INewPaymentRound round)
        {
            try
            {
                if (!IsEnabled || !_redisProvider.IsConnected)
                    return;

                var roundKey = string.Format("{0}:shares:round:{1}", _coin, round.Block.Height);

                _redisProvider.Client.Del(roundKey); // delete the associated shares.            
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while deleting shares: {0:l}", e.Message);
            }
        }

        public void MoveShares(IShare share)
        {
            try
            {
                if (!IsEnabled || !_redisProvider.IsConnected)
                    return;

                if (!share.IsBlockAccepted)
                    return;

                // rename round.
                var currentKey = string.Format("{0}:shares:round:current", _coin);
                var roundKey = string.Format("{0}:shares:round:{1}", _coin, share.Height);
                _redisProvider.Client.Rename(currentKey, roundKey);
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while moving shares for new block: {0:l}", e.Message);
            }
        }

        public void MoveSharesToCurrentRound(IPersistedBlock block)
        {
            try
            {
                if (!IsEnabled || !_redisProvider.IsConnected)
                    return;

                if (block.Status == BlockStatus.Confirmed || block.Status == BlockStatus.Pending)
                    return;

                var round = string.Format("{0}:shares:round:{1}", _coin, block.Height); // rounds shares key.
                var current = string.Format("{0}:shares:round:current", _coin); // current round key.

                //_client.StartPipeTransaction(); // batch the commands as atomic.

                // add shares to current round again.
                foreach (var entry in _redisProvider.Client.HGetAll(round))
                {
                    _redisProvider.Client.HIncrByFloat(current, entry.Key, double.Parse(entry.Value, CultureInfo.InvariantCulture));
                }

                _redisProvider.Client.Del(round); // delete the round shares.

                //_client.EndPipe(); // execute the batch commands.
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while moving shares: {0:l}", e.Message);
            }
        }

        public Dictionary<string, double> GetCurrentShares()
        {
            var shares = new Dictionary<string, double>();

            try
            {
                if (!IsEnabled || !_redisProvider.IsConnected)
                    return shares;

                var key = string.Format("{0}:shares:round:{1}", _coin, "current");
                var hashes = _redisProvider.Client.HGetAll(key);

                foreach (var hash in hashes)
                {
                    shares.Add(hash.Key, double.Parse(hash.Value, CultureInfo.InvariantCulture));
                }
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while getting shares for current round: {0:l}", e.Message);
            }

            return shares;
        }

        public Dictionary<string, double> GetShares(IPersistedBlock block)
        {
            var shares = new Dictionary<string, double>();

            try
            {
                if (!IsEnabled || !_redisProvider.IsConnected)
                    return shares;

                var key = string.Format("{0}:shares:round:{1}", _coin, block.Height);
                var hashes = _redisProvider.Client.HGetAll(key);

                foreach (var hash in hashes)
                {
                    shares.Add(hash.Key, double.Parse(hash.Value, CultureInfo.InvariantCulture));
                }
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while getting shares for round; {0:l}", e.Message);
            }

            return shares;
        }

        public void DeleteExpiredHashrateData(int until)
        {
            try
            {
                if (!IsEnabled || !_redisProvider.IsConnected)
                    return;

                var key = string.Format("{0}:hashrate", _coin); 
                _redisProvider.Client.ZRemRangeByScore(key, double.NegativeInfinity, until);
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
                if (!IsEnabled || !_redisProvider.IsConnected)
                    return hashrates;

                var key = string.Format("{0}:hashrate", _coin);

                var results = _redisProvider.Client.ZRangeByScore(key, since, double.PositiveInfinity);

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

        public void AddBlock(IShare share)
        {
            try
            {
                if (!IsEnabled)
                    return;

                using (var connection = new MySqlConnection(_mySqlProvider.ConnectionString))
                {
                    connection.Execute(
                        @"INSERT INTO Block(Height, BlockHash, TxHash, Amount, CreatedAt) VALUES (@height, @blockHash, @txHash, @amount, @createdAt)",
                        new
                        {
                            height = share.Block.Height,
                            blockHash = share.BlockHash.ToHexString(),
                            txHash = share.Block.Tx.First(),
                            amount = (decimal)share.GenerationTransaction.TotalAmount,
                            createdAt = share.Block.Time.UnixTimeToDateTime()
                        });
                }
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while adding block; {0:l}", e.Message);
            }            
        }

        public void UpdateBlock(IPersistedBlock block)
        {
            try
            {
                if (!IsEnabled)
                    return;

                using (var connection = new MySqlConnection(_mySqlProvider.ConnectionString))
                {
                    connection.Execute(
                        @"UPDATE Block SET Orphaned = @orphaned, Confirmed = @confirmed, Accounted = @accounted, Reward = @reward WHERE Height = @height",
                        new
                        {
                            orphaned = block.Status == BlockStatus.Orphaned,
                            confirmed = block.Status == BlockStatus.Confirmed,
                            accounted = block.Accounted,
                            reward = block.Reward,
                            height = block.Height
                        });
                }
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while updating block; {0:l}", e.Message);
            }
        }

        public IDictionary<string, int> GetTotalBlocks()
        {
            var blocks = new Dictionary<string, int> { { "total", 0 }, { "pending", 0 }, { "orphaned", 0 }, { "confirmed", 0 } };

            try
            {
                if (!IsEnabled)
                    return blocks;

                using (var connection = new MySqlConnection(_mySqlProvider.ConnectionString))
                {
                    var result = connection.Query(@"SELECT COUNT(*),
                (SELECT COUNT(*) FROM Block WHERE Orphaned = false AND Confirmed = false) AS pending,
                (SELECT COUNT(*) FROM Block WHERE Orphaned = true) AS orphaned,
                (SELECT COUNT(*) FROM Block WHERE Confirmed = true) AS confirmed
                from Block");

                    var data = result.First();
                    blocks["pending"] = (int) data.pending;
                    blocks["orphaned"] = (int) data.orphaned;
                    blocks["confirmed"] = (int) data.confirmed;
                }
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while getting block totals: {0:l}", e.Message);
            }

            return blocks;
        }

        public IEnumerable<IPersistedBlock> GetAllUnpaidBlocks()
        {
            var blocks = new List<IPersistedBlock>();

            try
            {
                using (var connection = new MySqlConnection(_mySqlProvider.ConnectionString))
                {
                    // we need to find the blocks that were confirmed by the coin network but still not accounted.
                    var results =
                        connection.Query<PersistedBlock>(
                            @"SELECT Height, Orphaned, Confirmed, Accounted, BlockHash, TxHash, Amount, Reward, CreatedAt 
                                FROM Block WHERE Accounted = false AND Confirmed = true ORDER BY Height DESC");

                    blocks.AddRange(results);
                }
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while getting un-paid blocks: {0:l}", e.Message);
            }

            return blocks;
        }

        public IEnumerable<IPersistedBlock> GetAllPendingBlocks()
        {
            var blocks = new List<IPersistedBlock>();

            try
            {
                if (!IsEnabled)
                    return blocks;

                using (var connection = new MySqlConnection(_mySqlProvider.ConnectionString))
                {
                    var results = connection.Query<PersistedBlock>(
                        @"SELECT Height, Orphaned, Confirmed, Accounted, BlockHash, TxHash, Amount, Reward, CreatedAt 
                            FROM Block WHERE Orphaned = false AND Confirmed = false ORDER BY Height ASC"
                 );

                    blocks.AddRange(results);
                }
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while getting pending blocks: {0:l}", e.Message);
            }

            return blocks;
        }

        public IEnumerable<IPersistedBlock> GetLastBlocks(int count = 20)
        {
            var blocks = new List<IPersistedBlock>();

            try
            {
                if (!IsEnabled)
                    return blocks;

                using (var connection = new MySqlConnection(_mySqlProvider.ConnectionString))
                {
                    var results = connection.Query<PersistedBlock>(
                        string.Format(
                            @"SELECT Height, Orphaned, Confirmed, Accounted, BlockHash, TxHash, Amount, Reward, CreatedAt 
                                FROM Block ORDER BY Height DESC LIMIT {0}",
                            count)
                        );

                    blocks.AddRange(results);
                }
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while getting last blocks: {0:l}", e.Message);
            }

            return blocks;
        }

        public IEnumerable<IPersistedBlock> GetLastBlocks(BlockStatus status, int count = 20)
        {
            var blocks = new List<IPersistedBlock>();

            try
            {
                if (!IsEnabled)
                    return blocks;

                string filter = string.Empty;

                switch (status)
                {
                    case BlockStatus.Pending:
                        filter = "Orphaned = false AND Confirmed = false";
                        break;
                    case BlockStatus.Orphaned:
                        filter = "Orphaned = true";
                        break;
                    case BlockStatus.Confirmed:
                        filter = "Confirmed = true";
                        break;
                }

                using (var connection = new MySqlConnection(_mySqlProvider.ConnectionString))
                {
                    var results = connection.Query<PersistedBlock>(string.Format(
                        @"SELECT Height, Orphaned, Confirmed, Accounted, BlockHash, TxHash, Amount, Reward, CreatedAt 
                            FROM Block WHERE {0} ORDER BY Height DESC LIMIT {1}",
                        filter, count));

                    blocks.AddRange(results);
                }
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while getting last {0:l} blocks: {1:l}", status.ToString().ToLower(), e.Message);
            }

            return blocks;
        }

        public void AddAccount(IAccount user)
        {
            try
            {
                if (!IsEnabled)
                    return;

                using (var connection = new MySqlConnection(_mySqlProvider.ConnectionString))
                {
                    connection.Execute(
                        @"INSERT INTO Account(Username, Address, CreatedAt) VALUES (@username, @address, @createdAt)",
                        new
                        {
                            username = user.Username,
                            address = user.Address,
                            createdAt = DateTime.Now
                        });
                }
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while creating user; {0:l}", e.Message);
            }
        }

        public IAccount GetAccount(string username)
        {
            try
            {
                if (!IsEnabled)
                    return null;

                using (var connection = new MySqlConnection(_mySqlProvider.ConnectionString))
                {
                    return connection.Query<Account>("SELECT Id, Username FROM Account WHERE Username = @username",
                        new {username}).Single();
                }
            }
            catch (InvalidOperationException) // fires when no result is found.
            {
                return null;
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while getting user; {0:l}", e.Message);
                return null;
            }
        }

        public IAccount GetAccountById(int id)
        {
            try
            {
                if (!IsEnabled)
                    return null;

                using (var connection = new MySqlConnection(_mySqlProvider.ConnectionString))
                {
                    return connection.Query<Account>("SELECT Id, Username FROM Account WHERE Id = @id",
                        new { id }).Single();
                }
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while getting user; {0:l}", e.Message);
                return null;
            }
        }

        public Dictionary<string, double> GetPreviousBalances()
        {
            var previousBalances = new Dictionary<string, double>();

            try
            {
                if (!IsEnabled || !_redisProvider.IsConnected)
                    return previousBalances;

                var key = string.Format("{0}:balances", _coin);
                var hashes = _redisProvider.Client.HGetAll(key);
                previousBalances = hashes.ToDictionary(x => x.Key, x => double.Parse(x.Value, CultureInfo.InvariantCulture));
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while getting previous balances: {0:l}", e.Message);
            }

            return previousBalances;
        }

        public void CommitPayoutsForRound(INewPaymentRound round)
        {
            try
            {
                if (!IsEnabled)
                    return;

                using (var connection = new MySqlConnection(_mySqlProvider.ConnectionString))
                {
                    foreach (var entry in round.Payouts)
                    {
                        connection.Execute(
                            @"INSERT INTO Payout(Block,User, Amount, CreatedAt) VALUES(@blockId, @userId, @amount, @createdAt)",
                            new
                            {
                                blockId = entry.BlockId,
                                userId = entry.UserId,
                                amount = entry.Amount,
                                createdAt = DateTime.Now
                            });
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while committing payouts for round; {0:l}", e.Message);
            }
        }

        public IList<IPayout> GetPendingPayouts()
        {
            var payouts = new List<IPayout>();

            try
            {
                if (!IsEnabled)
                    return payouts;

                using (var connection = new MySqlConnection(_mySqlProvider.ConnectionString))
                {
                    var results = connection.Query<Payout>(
                            @"SELECT Id, Block, User, Amount, Completed FROM Payout Where Completed = false ORDER BY Id ASC");

                    payouts.AddRange(results);
                }
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while getting pending payments: {0:l}", e.Message);
            }

            return payouts;
        }

        public void UpdatePayout(IPayout payout)
        {
            try
            {
                if (!IsEnabled)
                    return;

                using (var connection = new MySqlConnection(_mySqlProvider.ConnectionString))
                {
                    connection.Execute(
                        @"UPDATE Payout SET Completed = @completed WHERE Id = @id",
                        new
                        {
                            completed = payout.Completed,
                            id = payout.Id
                        });
                }
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while updating payment; {0:l}", e.Message);
            }
        }

        public void AddTransaction(IPaymentTransaction transaction)
        {
            try
            {
                if (!IsEnabled)
                    return;

                using (var connection = new MySqlConnection(_mySqlProvider.ConnectionString))
                {
                    connection.Execute(
                        @"INSERT INTO Transaction(User, Payment, Amount, Currency, TxId, CreatedAt) VALUES(@userId, @paymentId, @amount, @currency, @txId, @createdAt)",
                        new
                        {
                            userId = transaction.User.Id,
                            paymentId = transaction.Payment.Id,
                            amount = transaction.Payment.Amount,
                            currency = transaction.Currency,
                            txId = transaction.TxId,
                            createdAt = transaction.CreatedAt
                        });
                }
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while committing transaction; {0:l}", e.Message);
            }
        }

        public bool Authenticate(IMiner miner)
        {
            // within current implementation of hybrid storage layer, we don't have users registered to a pool but they
            // just mine with supplying a valid coin wallet address as username. So we just need to make sure the username
            // is valid address against the coin network.
            try
            {
                return _daemonClient.ValidateAddress(miner.Username).IsValid; // if so validate it against coin daemon as an address.
            }
            catch (RpcException)
            {
                return false;
            }
        }

        public void UpdateDifficulty(IStratumMiner miner)
        {
            // with-in our current hybrid-storage-layer, we don't need to write difficulty to persistance layer.
            return;
        }
    }
}
