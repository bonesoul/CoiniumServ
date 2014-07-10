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
using Coinium.Persistance.Redis;
using Coinium.Repository.Context;
using Coinium.Server.Web;

namespace Coinium.Utils.Configuration
{
    public class GlobalConfigFactory : IGlobalConfigFactory
    {
        private const string FileName = "config.json";

        private dynamic _data;
        private IRedisConfig _redisConfig;
        private IWebServerConfig _webServerConfig;

        /// <summary>
        /// The _application context
        /// </summary>
        private IApplicationContext _applicationContext;

        public GlobalConfigFactory(IApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;            
        }

        public dynamic Get()
        {
            // return the global config, if we haven't read it yet, do so.
            return _data ?? (_data = JsonConfigReader.Read(FileName));
        }

        public IRedisConfig GetRedisConfig()
        {
            // return the redis config, if we haven't read it yet, do so.
            return _redisConfig ?? (_redisConfig = new RedisConfig(Get().storage.redis));
        }

        public IWebServerConfig GetWebServerConfig()
        {
            return _webServerConfig ?? (_webServerConfig = new WebServerConfig(Get().web));
        }
    }
}
