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
using Coinium.Daemon;
using Coinium.Mining.Shares;
using Coinium.Repository.Context;
using Nancy.TinyIoc;

namespace Coinium.Service
{
    public class ServiceFactory : IServiceFactory
    {
        /// <summary>
        /// The _kernel
        /// </summary>
        private readonly IApplicationContext _applicationContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceFactory"/> class.
        /// </summary>
        public ServiceFactory(IApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        /// <summary>
        /// Gets the specified service name.
        /// </summary>
        /// <param name="serviceName">Name of the service.</param>
        /// <param name="shareManager">The share manager.</param>
        /// <param name="daemonClient">The daemon client.</param>
        /// <returns></returns>
        public IRpcService Get(string serviceName, IShareManager shareManager, IDaemonClient daemonClient)
        {
            var @params = new NamedParameterOverloads
            {
                {"shareManager", shareManager}, 
                {"daemonClient", daemonClient}
            };
            return _applicationContext.Container.Resolve<IRpcService>(serviceName, @params);
        }
    }
}
