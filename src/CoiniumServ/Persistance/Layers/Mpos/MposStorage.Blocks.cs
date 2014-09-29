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
