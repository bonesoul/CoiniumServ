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
using CoiniumServ.Payments;
using CoiniumServ.Persistance.Blocks;
using CoiniumServ.Server.Mining.Stratum.Sockets;
using CoiniumServ.Shares;
using CoiniumServ.Utils.Extensions;
using Dapper;
using MySql.Data.MySqlClient;

namespace CoiniumServ.Persistance.Layers.Mpos
{
    public partial class MposStorage
    {
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

        public void MoveCurrentShares(int height)
        {
            // this function is not supported as this functionality is handled by mpos itself.
        }

        public void MoveOrphanedShares(IPersistedBlock block)
        {
            // this function is not supported as this functionality is handled by mpos itself.
            throw new NotSupportedException();
        }

        public void RemoveShares(IPaymentRound round)
        {
            // this function is not supported as this functionality is handled by mpos itself.
            throw new NotSupportedException();
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
            // this function is not supported as this functionality is only required by payment processors which mpos itself is already one so and handles itself.
            throw new NotSupportedException();
        }
    }
}
