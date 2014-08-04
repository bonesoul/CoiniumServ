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
using CoiniumServ.Pools.Config;
using CoiniumServ.Shares;
using Serilog;
using ServiceStack.Redis;

namespace CoiniumServ.Persistance.Redis
{
    public class Redis2 : IStorage, IRedis
    {
        public bool IsConnected { get; private set; }
        public bool IsEnabled { get; private set; }

        private readonly Version _requiredMinimumVersion = new Version(2, 6);

        private RedisClient _client;

        private readonly IRedisConfig _redisConfig;
        private readonly IPoolConfig _poolConfig;

        private readonly ILogger _logger;

        public Redis2(PoolConfig poolConfig)
        {
            _logger = Log.ForContext<Redis2>().ForContext("Component", poolConfig.Coin.Name);

            _poolConfig = poolConfig; // the pool config.
            _redisConfig = (IRedisConfig)poolConfig.Storage;

            IsEnabled = _redisConfig.Enabled;

            if (IsEnabled)
                Initialize();
        }

        public void AddShare(IShare share)
        {
            throw new System.NotImplementedException();
        }

        public void AddBlock(IShare share)
        {
            throw new System.NotImplementedException();
        }

        public void SetRemainingBalances(IList<IWorkerBalance> workerBalances)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteShares(IPaymentRound round)
        {
            throw new System.NotImplementedException();
        }

        public void MoveSharesToCurrentRound(IPaymentRound round)
        {
            throw new System.NotImplementedException();
        }

        public void MoveBlock(IPaymentRound round)
        {
            throw new System.NotImplementedException();
        }

        public IDictionary<string, int> GetBlockCounts()
        {
            throw new System.NotImplementedException();
        }

        public void DeleteExpiredHashrateData(int until)
        {
            if (!IsEnabled || !IsConnected)
                return;
        }

        public IDictionary<string, double> GetHashrateData(int since)
        {
            throw new System.NotImplementedException();
        }

        public IList<IPendingBlock> GetPendingBlocks()
        {
            throw new System.NotImplementedException();
        }

        public IDictionary<uint, IPersistedBlock> GetAllBlocks()
        {
            throw new System.NotImplementedException();
        }

        public Dictionary<uint, Dictionary<string, double>> GetSharesForRounds(IList<IPaymentRound> rounds)
        {
            throw new System.NotImplementedException();
        }

        public Dictionary<string, double> GetPreviousBalances()
        {
            throw new System.NotImplementedException();
        }

        private void Initialize()
        {
            try
            {
                // create the connection
                _client = !string.IsNullOrEmpty(_redisConfig.Password)
                    ? new RedisClient(_redisConfig.Host, _redisConfig.Port, _redisConfig.Password)
                    {
                        Db = _redisConfig.DatabaseId,
                        ConnectTimeout = 5000
                    }
                    : new RedisClient(_redisConfig.Host, _redisConfig.Port)
                    {
                        Db = _redisConfig.DatabaseId,
                        ConnectTimeout = 5000
                    };

                IsConnected = true;

                // check the version
                var version = GetVersion();
                if (version < _requiredMinimumVersion)
                    throw new Exception(string.Format("You are using redis version {0}, minimum required version is 2.6", version));

                _logger.Information("Storage initialized: {0:l}:{1}, v{2:l}.", _redisConfig.Host, _redisConfig.Port, version);
            }
            catch (Exception e)
            {
                _logger.Error("Storage initialization failed: {0:l}:{1} - {2:l}\n\r {3:l}", _redisConfig.Host, _redisConfig.Port, e.Message);
            }
        }

        private Version GetVersion()
        {
            Version version = null;
            var info = _client.Info;

            foreach (var pair in info)
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
