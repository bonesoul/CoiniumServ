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
using CoiniumServ.Persistance.Blocks;
using CoiniumServ.Persistance.Query;
using CoiniumServ.Shares;
using CoiniumServ.Utils.Extensions;
using CoiniumServ.Utils.Helpers;
using Dapper;
using MySql.Data.MySqlClient;

namespace CoiniumServ.Persistance.Layers.Hybrid
{
    public partial class HybridStorage
    {
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
                            createdAt = share.Block.Time.UnixTimestampToDateTime()
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

        public IPersistedBlock GetBlock(uint height)
        {
            try
            {
                if (!IsEnabled)
                    return null;

                using (var connection = new MySqlConnection(_mySqlProvider.ConnectionString))
                {
                    return connection.Query<PersistedBlock>(
                        @"SELECT Height, Orphaned, Confirmed, Accounted, BlockHash, TxHash, Amount, Reward, CreatedAt From Block WHERE Height = @height",
                        new { height }).Single();
                }
            }
            catch (InvalidOperationException) // fires when no result is found.
            {
                return null;
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while getting block; {0:l}", e.Message);
                return null;
            }
        }

        public IList<IPersistedBlock> GetBlocks(IPaginationQuery paginationQuery)
        {
            var blocks = new List<IPersistedBlock>();

            try
            {
                if (!IsEnabled)
                    return null;

                using (var connection = new MySqlConnection(_mySqlProvider.ConnectionString))
                {
                    var results = connection.Query<PersistedBlock>(
                        @"SELECT Height, Orphaned, Confirmed, Accounted, BlockHash, TxHash, Amount, Reward, CreatedAt From Block 
                            ORDER BY Height DESC LIMIT @offset, @count",
                        new
                        {
                            offset = paginationQuery.Offset,
                            count = paginationQuery.Count
                        });

                    blocks.AddRange(results);
                }
            }
            catch (InvalidOperationException) // fires when no result is found.
            {
                return null;
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while getting block; {0:l}", e.Message);
            }

            return blocks;
        }

        public IList<IPersistedBlock> GetPaidBlocks(IPaginationQuery paginationQuery)
        {
            var blocks = new List<IPersistedBlock>();

            try
            {
                if (!IsEnabled)
                    return null;

                using (var connection = new MySqlConnection(_mySqlProvider.ConnectionString))
                {
                    var results = connection.Query<PersistedBlock>(
                        @"SELECT b.Height, b.Orphaned, b.Confirmed, b.Accounted, b.BlockHash, b.TxHash, b.Amount, b.Reward, b.CreatedAt 
                            From Block b
	                            INNER JOIN Payment as p ON p.Block = b.Height
	                            WHERE b.Accounted = true and p.Completed = true
                                GROUP BY b.Height
	                            ORDER BY b.Height DESC LIMIT @offset, @count",
                        new
                        {
                            offset = paginationQuery.Offset,
                            count = paginationQuery.Count
                        });

                    blocks.AddRange(results);
                }
            }
            catch (InvalidOperationException) // fires when no result is found.
            {
                return null;
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while getting block; {0:l}", e.Message);
            }

            return blocks;
        }

        public IEnumerable<IPersistedBlock> GetUnpaidBlocks()
        {
            var blocks = new List<IPersistedBlock>();

            try
            {
                using (var connection = new MySqlConnection(_mySqlProvider.ConnectionString))
                {
                    // we need to find the blocks that were confirmed by the coin network but still not accounted.
                    var results = connection.Query<PersistedBlock>(
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

        public IEnumerable<IPersistedBlock> GetPendingBlocks()
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
                    blocks["pending"] = (int)data.pending;
                    blocks["orphaned"] = (int)data.orphaned;
                    blocks["confirmed"] = (int)data.confirmed;
                }
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while getting block totals: {0:l}", e.Message);
            }

            return blocks;
        }
    }
}
