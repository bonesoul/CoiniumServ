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
using CoiniumServ.Persistance.Providers;
using CoiniumServ.Persistance.Providers.MySql;
using CoiniumServ.Pools;
using CoiniumServ.Server.Mining.Stratum.Sockets;
using CoiniumServ.Shares;
using CoiniumServ.Utils.Extensions;
using Dapper;
using Serilog;

namespace CoiniumServ.Persistance.Layers.Mpos
{
    public class MposStorage : IStorageLayer
    {
        private readonly IMySqlProvider _mySqlProvider;

        private readonly ILogger _logger;

        public MposStorage(IEnumerable<IStorageProvider> providers, PoolConfig poolConfig)
        {
            _logger = Log.ForContext<MposStorage>().ForContext("Component", poolConfig.Coin.Name);

            foreach (var provider in providers)
            {
                if (provider is IMySqlProvider)
                    _mySqlProvider = (IMySqlProvider) provider;
            }
        }

//            connection.Execute(
//                @"insert shares(rem_host, username, our_result, upstream_result, reason, solution, difficulty,time) 
//                    values (@rem_host, @username, @our_result, @upstream_result, @reason, @solution, @difficulty, @time)",
//                new
//                {
//                    rem_host = "127.0.0.1",
//                    username = "test",
//                    our_result = 'Y',
//                    upstream_result = 'Y',
//                    reason = "test",
//                    solution = "test",
//                    difficulty = 5,
//                    time = DateTime.Now
//                });

            //var db = Database.OpenConnection("Server=localhost;Port=3306;Database=mpos;Uid=root;provider=MySql.Data");
            //var test =db.Settings.FindByName("DB_VERSION");


        public bool SupportsShareStorage { get { return true; } }

        public void AddShare(IShare share)
        {
            try
            {
                var ourResult = share.IsValid ? 'Y' : 'N';
                var upstreamResult = share.IsBlockCandidate ? 'Y' : 'N';

                object errorReason;
                if (share.Error != ShareError.None)
                    errorReason = share.Error;
                else
                    errorReason = null;

                _mySqlProvider.Connection.Execute(
                    @"insert shares(rem_host, username, our_result, upstream_result, reason, solution, difficulty,time)
                values (@rem_host, @username, @our_result, @upstream_result, @reason, @solution, @difficulty, @time)",
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
            catch (Exception e)
            {
                _logger.Error("An exception occured while comitting share: {0:l}", e.Message);
            }
        }

        public void RemoveShares(IPaymentRound round)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, double> GetCurrentShares()
        {
            throw new NotImplementedException();
        }

        public Dictionary<uint, Dictionary<string, double>> GetShares(IList<IPaymentRound> rounds)
        {
            throw new NotImplementedException();
        }

        public bool SupportsBlockStorage { get { return false; } }

        public void AddBlock(IShare share)
        {
            throw new NotImplementedException();
        }

        public void UpdateBlock(IPaymentRound round)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, int> GetTotalBlocks()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IPersistedBlock> GetBlocks(BlockStatus status)
        {
            throw new NotImplementedException();
        }

        public bool SupportsWorkerStorage { get { return true; } }

        public void GetWorker(string username)
        {
            throw new NotImplementedException();
        }

        public bool SupoortsPaymentsStorage { get { return false; } }
    }
}
