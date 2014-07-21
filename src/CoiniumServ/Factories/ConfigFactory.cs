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

using CoiniumServ.Persistance.Redis;
using CoiniumServ.Repository.Context;
using CoiniumServ.Server.Web;
using CoiniumServ.Utils.Configuration;
using CoiniumServ.Utils.Logging;

namespace CoiniumServ.Factories
{
    /// <summary>
    /// Configuration factory that handles configs.
    /// </summary>
    public class ConfigFactory:IConfigFactory
    {
        public bool GlobalConfigExists { get { return _globalConfigData != null; }}
        public dynamic Logging { get { return _globalConfigData.logging; } }

        private const string GlobalConfigFilename = "config/config.json"; // global config filename.
        private readonly dynamic _globalConfigData; // global config data.

        private readonly IRedisConfig _redisConfig;
        private readonly IWebServerConfig _webServerConfig;
        private readonly ILoggingConfig _logConfig;

        /// <summary>
        /// The _application context
        /// </summary>
        private IApplicationContext _applicationContext;

        public ConfigFactory(IApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
            _globalConfigData = JsonConfigReader.Read(GlobalConfigFilename); // read the global config data.
            _redisConfig = new RedisConfig(_globalConfigData.storage.redis);
            _webServerConfig = new WebServerConfig(_globalConfigData.website);
            _logConfig = new LoggingConfig(_globalConfigData.logging);
        }

        // todo: should be per-pool!
        public IRedisConfig GetRedisConfig()
        {
            return _redisConfig;
        }

        public IWebServerConfig GetWebServerConfig()
        {
            return _webServerConfig;
        }

        public ILoggingConfig GetLoggingConfig()
        {
            return _logConfig;
        }
    }
}
 