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
using Serilog;

namespace CoiniumServ.Persistance.Providers.Redis
{
    public class RedisProviderConfig:IRedisProviderConfig
    {
        public bool Valid { get; private set; }
        public string Host { get; private set; }
        public int Port { get; private set; }
        public string Password { get; private set; }
        public int DatabaseId { get; private set; }

        public RedisProviderConfig(dynamic config)
        {
            try
            {
                // load the config data.
                Host = string.IsNullOrEmpty(config.host) ? "127.0.0.1" : config.host;
                Port = config.port == 0 ? 6379 : config.port;
                Password = config.password;
                DatabaseId = config.databaseId;
                Valid = true;
            }
            catch (Exception e)
            {
                Valid = false;
                Log.Logger.ForContext<RedisProviderConfig>().Error(e, "Error loading redis configuration");
            }
        }
    }
}
