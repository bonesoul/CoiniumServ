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
using CoiniumServ.Accounts;
using CoiniumServ.Miners;
using CoiniumServ.Payments;
using CoiniumServ.Payments.Processor;
using CoiniumServ.Payments.Round;
using CoiniumServ.Persistance.Blocks;
using CoiniumServ.Persistance.Providers;
using CoiniumServ.Persistance.Providers.MySql;
using CoiniumServ.Pools;
using CoiniumServ.Server.Mining.Stratum;
using CoiniumServ.Server.Mining.Stratum.Sockets;
using CoiniumServ.Shares;
using CoiniumServ.Utils.Extensions;
using Dapper;
using MySql.Data.MySqlClient;
using Serilog;

namespace CoiniumServ.Persistance.Layers.Mpos
{
    public class MposStorageLayer : IStorageLayer
    {
        public bool IsEnabled { get; private set; }

        private readonly IMySqlProvider _mySqlProvider;

        private readonly ILogger _logger;

        public MposStorageLayer(IEnumerable<IStorageProvider> providers, PoolConfig poolConfig)
        {
            _logger = Log.ForContext<MposStorageLayer>().ForContext("Component", poolConfig.Coin.Name);

            foreach (var provider in providers)
            {
                if (provider is IMySqlProvider)
                    _mySqlProvider = (IMySqlProvider) provider;
            }

            IsEnabled = _mySqlProvider != null;
        }

        public void AddShare(IShare share)
        {
            try
            {
                if (!IsEnabled)
                    return;

                var ourResult = share.IsValid ? 'Y' : 'N';
                var upstreamResult = share.IsBlockCandidate ? 'Y' : 'N';

                object errorReason;
                if (share.Error != ShareError.None)
                    errorReason = share.Error;
                else
                    errorReason = null;

                using (var connection = new MySqlConnection(_mySqlProvider.ConnectionString))
                {
                    connection.Execute(
                        @"INSERT INTO shares(rem_host, username, our_result, upstream_result, reason, solution, difficulty,time)
                            VALUES (@rem_host, @username, @our_result, @upstream_result, @reason, @solution, @difficulty, @time)",
                        new
                        {
                            rem_host = ((IClient) share.Miner).Connection.RemoteEndPoint.Address.ToString(),
                            username = share.Miner.Username,
                            our_result = ourResult,
                            upstream_result = upstreamResult,
                            reason = errorReason,
                            solution = share.BlockHash.ToHexString(),
                            difficulty = share.Difficulty, // should we consider mpos difficulty multiplier here?
                            time = DateTime.Now
                        });
                }
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while comitting share; {0:l}", e.Message);
            }
        }

        public void RemoveShares(INewPaymentRound round)
        {
            // The function is not supported as it's only required by payments processor. In MPOS mode payments processor should be disabled.
            throw new NotImplementedException(); 
        }

        public void MoveShares(IShare share)
        {
            // The function is not supported as MPOS mode doesn't require the functionality.
        }

        public void MoveSharesToCurrentRound(IPersistedBlock block)
        {
            // The function is not supported as it's only required by payments processor. In MPOS mode payments processor should be disabled.
            throw new NotImplementedException();
        }

        public Dictionary<string, double> GetCurrentShares()
        {
            var shares = new Dictionary<string, double>();

            try
            {
                if (!IsEnabled)
                    return shares;

                using (var connection = new MySqlConnection(_mySqlProvider.ConnectionString))
                {
                    var results = connection.Query(
                        @"SELECT username, sum(difficulty) AS diff FROM shares GROUP BY username");

                    foreach (var row in results)
                    {
                        shares.Add(row.username, row.diff);
                    }
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
            // The function is not supported as it's only required by payments processor. In MPOS mode payments processor should be disabled.
            throw new NotImplementedException();
        }

        public void DeleteExpiredHashrateData(int until)
        {
            // MPOS doesn't use another hashrate data structure except the shares, which are already handled by MPOS.
        }

        public IDictionary<string, double> GetHashrateData(int since)
        {
            var hashrateData = new Dictionary<string, double>();

            try
            {
                if (!IsEnabled)
                    return hashrateData;

                using (var connection = new MySqlConnection(_mySqlProvider.ConnectionString))
                {
                    var results = connection.Query(@"SELECT username, sum(difficulty) AS shares FROM shares WHERE our_result='Y' GROUP BY username");

                    foreach (var row in results)
                    {
                        hashrateData.Add(row.username, row.shares);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while getting share data: {0:l}", e.Message);
            }

            return hashrateData;
        }

        public void AddBlock(IShare share)
        {
            // Blocks are handled by MPOS by itself, we don't need to persist found block data.
        }

        public void UpdateBlock(IPersistedBlock block)
        {
            // The function is not supported as it's only required by payments processor. In MPOS mode payments processor should be disabled.
            throw new NotImplementedException();
        }

        public IDictionary<string, int> GetTotalBlocks()
        {
            var blocks = new Dictionary<string, int> {{"total", 0}, {"pending", 0}, {"orphaned", 0}, {"confirmed", 0}};

            try
            {
                if (!IsEnabled)
                    return blocks;

                using (var connection = new MySqlConnection(_mySqlProvider.ConnectionString))
                {
                    var result = connection.Query(@"SELECT COUNT(*),
                        (SELECT COUNT(*) FROM blocks WHERE confirmations >= 0 AND confirmations < 120) AS pending,
                        (SELECT COUNT(*) FROM blocks WHERE confirmations < 0) AS orphaned,
                        (SELECT COUNT(*) FROM blocks WHERE confirmations >= 120) AS confirmed
                        from blocks");

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
            // The function is not supported as it's only required by payments processor. In MPOS mode payments processor should be disabled.
            throw new NotImplementedException();
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
                    var results = connection.Query<PersistedBlock>(@"SELECT height, blockhash, amount, confirmations, time 
                    FROM blocks WHERE confirmations >= 0 and confirmations < 120 ORDER BY height ASC");

                    blocks.AddRange(results);
                }
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while getting pending blocks; {0:l}", e.Message);
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
                        string.Format(@"SELECT height, blockhash, amount, confirmations, time 
                            FROM blocks ORDER BY height DESC LIMIT {0}",
                            count));

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
                        filter = "confirmations >= 0 AND confirmations < 120";
                        break;
                    case BlockStatus.Orphaned:
                        filter = "confirmations = -1";
                        break;
                    case BlockStatus.Confirmed:
                        filter = "confirmations >= 120";
                        break;
                }

                using (var connection = new MySqlConnection(_mySqlProvider.ConnectionString))
                {
                    var results = connection.Query<PersistedBlock>(string.Format(@"SELECT height, blockhash, amount, confirmations, time 
                        FROM blocks WHERE {0} ORDER BY height DESC LIMIT {1}",
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
            // The function is not supported as user managment is handled by MPOS itself.
            throw new NotImplementedException();
        }

        public IAccount GetAccount(string username)
        {
            // TODO: implement me!
            throw new NotImplementedException();
        }

        public IAccount GetAccountById(int id)
        {
            // TODO: implement me!
            throw new NotImplementedException();
        }

        public Dictionary<string, double> GetPreviousBalances()
        {
            // The function is not supported as it's only required by payments processor. In MPOS mode payments processor should be disabled.
            throw new NotImplementedException();
        }

        public void CommitPayoutsForRound(INewPaymentRound round)
        {
            // The function is not supported as it's only required by payments processor. In MPOS mode payments processor should be disabled.
            throw new NotImplementedException();
        }

        public IList<IPayout> GetPendingPayouts()
        {
            // TODO: implement me!
            throw new NotImplementedException();
        }

        public void UpdatePayout(IPayout payout)
        {
            // The function is not supported as it's only required by payments processor. In MPOS mode payments processor should be disabled.
            throw new NotImplementedException();
        }

        public void AddTransaction(IPaymentTransaction transaction)
        {
            // The function is not supported as it's only required by payments processor. In MPOS mode payments processor should be disabled.
            throw new NotImplementedException();
        }

        public bool Authenticate(IMiner miner)
        {
            try
            {
                if (!IsEnabled)
                    return false;

                using (var connection = new MySqlConnection(_mySqlProvider.ConnectionString))
                {
                    // query the username against mpos pool_worker table.
                    var result = connection.Query<string>(
                        "SELECT password FROM pool_worker WHERE username = @username",
                        new {username = miner.Username}).FirstOrDefault();

                    // if matching record exists for given miner username, then authenticate the miner.
                    // note: we don't check for password on purpose.
                    return result != null;
                }
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while reading pool_worker table; {0:l}", e.Message);
                return false;
            }
        }

        public void UpdateDifficulty(IStratumMiner miner)
        {
            try
            {
                if (!IsEnabled)
                    return;

                using (var connection = new MySqlConnection(_mySqlProvider.ConnectionString))
                {
                    connection.Execute(
                        "UPDATE pool_worker SET difficulty = @difficulty WHERE username = @username", new
                        {
                            difficulty = miner.Difficulty,
                            username = miner.Username
                        });
                }
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while updating difficulty for miner; {0:l}", e.Message);
            }
        }
    }
}
