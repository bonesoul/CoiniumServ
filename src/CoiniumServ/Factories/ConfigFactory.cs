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

using CoiniumServ.Coin.Config;
using CoiniumServ.Configuration;
using CoiniumServ.Pools.Config;
using CoiniumServ.Repository.Context;
using Nancy.TinyIoc;

namespace CoiniumServ.Factories
{
    /// <summary>
    /// Configuration factory that handles configs.
    /// </summary>
    public class ConfigFactory:IConfigFactory
    {
        /// <summary>
        /// The _application context
        /// </summary>
        private readonly IApplicationContext _applicationContext;

        public ConfigFactory(IApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public IConfigManager GetConfigManager()
        {
            return _applicationContext.Container.Resolve<IConfigManager>();
        }

        public IPoolConfig GetPoolConfig(dynamic config, ICoinConfig coinConfig)
        {
            var @params = new NamedParameterOverloads
            {
                {"config", config},
                {"coinConfig", coinConfig}
            };

            return _applicationContext.Container.Resolve<IPoolConfig>(@params);
        }

        public ICoinConfig GetCoinConfig(dynamic config)
        {
            var @params = new NamedParameterOverloads
            {
                {"config", config},
            };

            return _applicationContext.Container.Resolve<ICoinConfig>(@params);
        }
    }
}
 