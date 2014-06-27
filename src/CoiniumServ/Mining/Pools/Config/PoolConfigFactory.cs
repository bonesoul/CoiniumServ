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
using Coinium.Coin.Configs;
using Coinium.Common.Context;
using Nancy.TinyIoc;
using Serilog;

namespace Coinium.Mining.Pools.Config
{
    public class PoolConfigFactory : IPoolConfigFactory
    {
        /// <summary>
        /// The _application context
        /// </summary>
        private readonly IApplicationContext _applicationContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="PoolConfigFactory"/> class.
        /// </summary>
        /// <param name="applicationContext">The application context.</param>
        public PoolConfigFactory(IApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        /// <summary>
        /// Gets the specified read configuration.
        /// </summary>
        /// <param name="readConfig">The read configuration.</param>
        /// <returns></returns>
        public IPoolConfig Get(dynamic readConfig)
        {
            var @params = new NamedParameterOverloads { { "coinConfigFactory", _applicationContext.Container.Resolve<ICoinConfigFactory>() }, { "config", readConfig } };
            return _applicationContext.Container.Resolve<IPoolConfig>(@params);
        }
    }
}
