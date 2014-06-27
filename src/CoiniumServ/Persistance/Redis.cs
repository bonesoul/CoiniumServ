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
using System.Net.Sockets;
using Coinium.Common.Configuration;
using Coinium.Mining.Shares;
using Serilog;
using StackExchange.Redis;

namespace Coinium.Persistance
{
    public class Redis:IStorage, IRedis
    {
        public bool IsEnabled { get; private set; }
        public bool IsConnected { get { return _connectionMultiplexer.IsConnected; } }
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

            if(IsEnabled)
                Initialize();
        }

        public void CommitShare(IShare share)
        {
            if (!IsConnected)
                return;
        }

        private void Initialize()
        {
            var options = new ConfigurationOptions();
            var endpoint = new DnsEndPoint(Host, Port, AddressFamily.InterNetwork);
            options.EndPoints.Add(endpoint);

            try
            {

                // create the connection
                _connectionMultiplexer = ConnectionMultiplexer.ConnectAsync(options).Result;

                // access to database.
                _database = _connectionMultiplexer.GetDatabase(DatabaseId);

                Log.ForContext<Redis>().Information("Storage initialized: {0}", endpoint);
            }
            catch (Exception e)
            {
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
            DatabaseId = redisConfig.databaseId;
        }
    }
}
