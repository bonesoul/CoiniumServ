#region License
// 
//     CoiniumServ - Crypto Currency Mining Pool Server Software
//     Copyright (C) 2013 - 2014, CoiniumServ Project - http://www.coinium.org
//     https://github.com/CoiniumServ/CoiniumServ
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
using Coinium.Mining.Shares;
using Coinium.Utils.Configuration;
using Serilog;
using StackExchange.Redis;

namespace Coinium.Persistance.Redis
{
    public class Redis:IStorage, IRedis
    {
        public bool IsEnabled { get; private set; }
        public bool IsConnected { get { return _connectionMultiplexer.IsConnected; } }
        public string Host { get; private set; }
        public Int32 Port { get; private set; }
        public string Password { get; private set; }
        public int DatabaseId { get; private set; }

        private readonly Version _requiredMinimumVersion = new Version(2, 6);
        private readonly IGlobalConfigFactory _globalConfigFactory;
        private ConnectionMultiplexer _connectionMultiplexer;
        private IDatabase _database;
        private IServer _server;

        public Redis(IGlobalConfigFactory globalConfigFactory)
        {
            _globalConfigFactory = globalConfigFactory;

            ReadConfig();

            if (IsEnabled)
                Initialize();
        }

        public void CommitShare(IShare share)
        {
            if (!IsEnabled || !IsConnected)
                return;

            var roundKey = string.Format("{0}:shares:round:current", share.Miner.Pool.Config.Coin.Name.ToLower());
            var statsKey = string.Format("{0}:stats", share.Miner.Pool.Config.Coin.Name.ToLower());

            _database.HashIncrement(roundKey, share.Miner.Username, share.Difficulty ,CommandFlags.FireAndForget); // add the share to round.
            _database.HashIncrement(statsKey, share.Valid ? "validShares" : "invalidShares", 1 , CommandFlags.FireAndForget); // increment the valid shares.
        }

        private void Initialize()
        {
            var options = new ConfigurationOptions();
            var endpoint = new DnsEndPoint(Host, Port, AddressFamily.InterNetwork);
            options.EndPoints.Add(endpoint);
            options.AllowAdmin = true;
            if (!string.IsNullOrEmpty(Password))
                options.Password = Password;

            try
            {
                // create the connection
                _connectionMultiplexer = ConnectionMultiplexer.ConnectAsync(options).Result;

                // access to database.
                _database = _connectionMultiplexer.GetDatabase(DatabaseId);

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

        private void ReadConfig()
        {
            var globalConfig = _globalConfigFactory.Get();
            var redisConfig = globalConfig.database.redis;
            IsEnabled = redisConfig.enabled;
            Host = redisConfig.host;
            Port = redisConfig.port;
            Password = redisConfig.password;
            DatabaseId = redisConfig.databaseId;
        }
    }
}
