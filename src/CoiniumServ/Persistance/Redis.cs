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
using System.Net;
using Coinium.Common.Configuration;
using Coinium.Mining.Shares;
using StackExchange.Redis;

namespace Coinium.Persistance
{
    public class Redis:IStorage, IRedis
    {
        public bool Enabled { get; private set; }

        public string Host { get; private set; }
        public Int32 Port { get; private set; }
        public int DatabaseId { get; private set; }

        private readonly IGlobalConfigFactory _globalConfigFactory;
        private ConnectionMultiplexer _connectionMultiplexer;
        private IDatabase _database;

        public Redis(IGlobalConfigFactory globalConfigFactory)
        {
            _globalConfigFactory = globalConfigFactory;

            ReadConfig();

            if(Enabled)
                Initialize();
        }

        public void CommitShare(IShare share)
        {
            throw new NotImplementedException();
        }

        private void Initialize()
        {
            var options = new ConfigurationOptions();
            options.EndPoints.Add(new DnsEndPoint(Host, Port));

            // create the connection
            _connectionMultiplexer = ConnectionMultiplexer.ConnectAsync(options).Result;

            // access to database.
            _database = _connectionMultiplexer.GetDatabase(DatabaseId);
        }

        private void ReadConfig()
        {
            var globalConfig = _globalConfigFactory.Get();
            var redisConfig = globalConfig.database.redis;
            Enabled = redisConfig.enabled;
            Host = redisConfig.host;
            Port = redisConfig.port;
            DatabaseId = redisConfig.databaseId;
        }
    }
}
