#region License
// 
//     MIT License
//
//     CoiniumServ - Crypto Currency Mining Pool Server Software
//     Copyright (C) 2013 - 2017, CoiniumServ Project
//     Hüseyin Uslu, shalafiraistlin at gmail dot com
//     https://github.com/bonesoul/CoiniumServ
// 
//     Permission is hereby granted, free of charge, to any person obtaining a copy
//     of this software and associated documentation files (the "Software"), to deal
//     in the Software without restriction, including without limitation the rights
//     to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//     copies of the Software, and to permit persons to whom the Software is
//     furnished to do so, subject to the following conditions:
//     
//     The above copyright notice and this permission notice shall be included in all
//     copies or substantial portions of the Software.
//     
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//     FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//     LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//     OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//     SOFTWARE.
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
