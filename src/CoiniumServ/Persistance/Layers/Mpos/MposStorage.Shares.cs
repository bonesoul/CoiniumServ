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
