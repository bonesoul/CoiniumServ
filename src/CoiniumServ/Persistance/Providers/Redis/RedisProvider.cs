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
using CoiniumServ.Pools;
using CSRedis;
using Serilog;

namespace CoiniumServ.Persistance.Providers.Redis
{
    public class RedisProvider : IRedisProvider
    {
        public bool IsConnected { get { return Client.Connected; } }

        public RedisClient Client { get; private set; }
        private readonly Version _requiredMinimumVersion = new Version(2, 6);

        private readonly IRedisProviderConfig _config;        

        private readonly ILogger _logger;

        public RedisProvider(IPoolConfig poolConfig, IRedisProviderConfig config)
        {
            _logger = Log.ForContext<RedisProvider>().ForContext("Component", poolConfig.Coin.Name);

            _config = config;

            Initialize();
        }

        private void Initialize()
        {
            try
            {
                // create the connection
                Client = new RedisClient(_config.Host, _config.Port)
                {
                    ReconnectAttempts = 3,
                    ReconnectTimeout = 200
                };

                // select the database
                Client.Select(_config.DatabaseId);

                // authenticate if needed.
                if (!string.IsNullOrEmpty(_config.Password))
                    Client.Auth(_config.Password);

                // check the version
                var version = GetVersion();
                if (version < _requiredMinimumVersion)
                    throw new Exception(string.Format("You are using redis version {0}, minimum required version is 2.6", version));

                _logger.Information("Redis storage initialized: {0:l}:{1}, v{2:l}.", _config.Host, _config.Port, version);
            }
            catch (Exception e)
            {
                _logger.Error("Redis storage initialization failed: {0:l}:{1} - {2:l}", _config.Host, _config.Port, e.Message);
            }
        }

        private Version GetVersion()
        {
            Version version = null;

            try
            {
                var info = Client.Info("server");

                var parts = info.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

                foreach (var part in parts)
                {
                    var data = part.Split(':');

                    if (data[0] != "redis_version")
                        continue;

                    version = new Version(data[1]);
                }
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while getting version info: {0:l}", e.Message);
            }

            return version;
        }
    }
}
