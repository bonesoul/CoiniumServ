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
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Coinium.Mining.Pools.Config;
using Coinium.Mining.Shares;
using Coinium.Utils.Configuration;
using Coinium.Utils.Extensions;
using Coinium.Utils.Helpers.Time;
using Serilog;
using StackExchange.Redis;

namespace Coinium.Persistance.Redis
{
    public class Redis:IStorage, IRedis
    {
        public bool IsEnabled { get; private set; }
        public bool IsConnected { get { return _connectionMultiplexer.IsConnected; } }

        private readonly Version _requiredMinimumVersion = new Version(2, 6);
        private readonly IGlobalConfigFactory _globalConfigFactory;
        private readonly IRedisConfig _config;
        private readonly IPoolConfig _poolConfig;

        private ConnectionMultiplexer _connectionMultiplexer;
        private IDatabase _database;
        private IServer _server;

        public Redis(IGlobalConfigFactory globalConfigFactory, IPoolConfig poolConfig)
        {
            _globalConfigFactory = globalConfigFactory;

            _poolConfig = poolConfig;
            _config = _globalConfigFactory.GetRedisConfig(); // read the config.
            IsEnabled = _config.IsEnabled;

            if (IsEnabled)
                Initialize();
        }

        public void CommitShare(IShare share)
        {
            if (!IsEnabled || !IsConnected)
                return;

            var coin = _poolConfig.Coin.Name.ToLower(); // the coin we are working on.
            var batch = _database.CreateBatch(); // batch the commands.

            // add the share to round 
            // key: coin:shares:round:current
            // field: username, value: difficulty.
            var roundKey = string.Format("{0}:shares:round:current", coin);
            batch.HashIncrementAsync(roundKey, share.Miner.Username, share.Difficulty, CommandFlags.FireAndForget);

            // increment shares stats.
            // key: coin:stats
            // fields: validShares, invalidShares.
            var statsKey = string.Format("{0}:stats", coin);
            batch.HashIncrementAsync(statsKey, share.IsValid ? "validShares" : "invalidShares", 1, CommandFlags.FireAndForget);

            // add to hashrate 
            // key: coin:shares:hashrate
            // score: unix-time
            // value: difficulty:username
            if (share.IsValid)
            {
                var hashrateKey = string.Format("{0}:shares:hashrate", coin);
                var entry = string.Format("{0}:{1}", share.Difficulty, share.Miner.Username);
                batch.SortedSetAddAsync(hashrateKey, entry, TimeHelpers.NowInUnixTime(), CommandFlags.FireAndForget);
            }

            batch.Execute(); // execute the batch commands.
        }

        public void CommitBlock(IShare share)
        {
            var coin = _poolConfig.Coin.Name.ToLower(); // the coin we are working on.
            var batch = _database.CreateBatch(); // batch the commands.

            if (share.IsBlockAccepted)
            {
                // rename round [coin:round:current -> coin:round:heigh]
                var currentKey = string.Format("{0}:shares:round:current", coin);
                var newKey = string.Format("{0}:shares:round:{1}", coin, share.Height);
                batch.KeyRenameAsync(currentKey, newKey, When.Always, CommandFlags.HighPriority);

                // add block to pending 
                // key: coin:blocks:pending
                // score: block height:
                // value: blockHash:generation-transaction-hash
                var pendingKey = string.Format("{0}:blocks:pending", coin);
                var entry = string.Format("{0}:{1}", share.BlockHash.ToHexString(), share.Block.Tx.First());
                batch.SortedSetAddAsync(pendingKey, entry, share.Block.Height, CommandFlags.FireAndForget);
            }

            // increment block stats.
            // key: coin:stats
            // fields: validBlocks, invalidBlocks
            var statsKey = string.Format("{0}:stats", coin);
            batch.HashIncrementAsync(statsKey, share.IsBlockAccepted ? "validBlocks" : "invalidBlocks", 1, CommandFlags.FireAndForget);

            batch.Execute(); // execute the batch commands.
        }

        public string[] GetPendingBlocks()
        {
            var coin = _poolConfig.Coin.Name.ToLower(); // the coin we are working on.

            //var test1=_database.SortedSetRangeByRank()
            //var test2=_database.SortedSetRangeByRankWithScores()

            return null;
        }

        private void Initialize()
        {
            var options = new ConfigurationOptions();
            var endpoint = new DnsEndPoint(_config.Host, _config.Port, AddressFamily.InterNetwork);
            options.EndPoints.Add(endpoint);
            options.AllowAdmin = true;
            if (!string.IsNullOrEmpty(_config.Password))
                options.Password = _config.Password;

            try
            {
                // create the connection
                _connectionMultiplexer = ConnectionMultiplexer.ConnectAsync(options).Result;

                // access to database.
                _database = _connectionMultiplexer.GetDatabase(_config.DatabaseId);

                // get the configured server.
                _server = _connectionMultiplexer.GetServer(endpoint);

                // check the version
                var info = _server.Info();
                Version version = null;
                foreach (var pair in info[0])
                {
                    if (pair.Key == "redis_version")
                    {
                        version = new Version(pair.Value);
                        if (version < _requiredMinimumVersion)
                            throw new Exception(string.Format("You are using redis version {0}, minimum required version is 2.6", version));

                        break;
                    }
                }
                
                Log.ForContext<Redis>().Information("Storage initialized: {0}, v{1}.", endpoint, version);
            }
            catch (Exception e)
            {
                IsEnabled = false;
                Log.ForContext<Redis>().Error(e, string.Format("Storage initialization failed: {0}", endpoint));
            }
        }
    }
}
