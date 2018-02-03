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
using Dapper;
using MySql.Data.MySqlClient;

namespace CoiniumServ.Persistance.Layers.Mpos
{
    public partial class MposStorage
    {
        public void AddBlock(IShare share)
        {
            // this function is not supported as this functionality is handled by mpos itself.
        }

        public void UpdateBlock(IPersistedBlock block)
        {
            // this function is not supported as this functionality is handled by mpos itself.
        }

        public IPersistedBlock GetBlock(uint height)
        {
            throw new NotImplementedException();
        }

        public IList<IPersistedBlock> GetBlocks(IPaginationQuery paginationQuery)
        {
            throw new NotImplementedException();
        }

        public IList<IPersistedBlock> GetPaidBlocks(IPaginationQuery paginationQuery)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPersistedBlock> GetUnpaidBlocks()
        {
            // this function is not supported as this functionality is only required by payment processors which mpos itself is already one so and handles itself.
            throw new NotSupportedException();
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
                        (SELECT COUNT(*) FROM blocks WHERE confirmations >= 0 AND confirmations < 120) AS pending,
                        (SELECT COUNT(*) FROM blocks WHERE confirmations < 0) AS orphaned,
                        (SELECT COUNT(*) FROM blocks WHERE confirmations >= 120) AS confirmed
                        from blocks");

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
