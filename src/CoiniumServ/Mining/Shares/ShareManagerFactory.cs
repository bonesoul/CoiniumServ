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

using Coinium.Daemon;
using Coinium.Mining.Jobs;
using Coinium.Persistance;
using Coinium.Repository.Context;
using Nancy.TinyIoc;
using Serilog;

namespace Coinium.Mining.Shares
{
    public class ShareManagerFactory : IShareManagerFactory
    {
        /// <summary>
        /// The _kernel
        /// </summary>
        private readonly IApplicationContext _applicationContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShareManagerFactory" /> class.
        /// </summary>
        /// <param name="applicationContext">The application context.</param>
        public ShareManagerFactory(IApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        /// <summary>
        /// Gets the specified daemon client.
        /// </summary>
        /// <param name="jobManager">The job manager.</param>
        /// <param name="daemonClient"></param>
        /// <param name="storage"></param>
        /// <returns></returns>
        public IShareManager Get(IDaemonClient daemonClient, IJobManager jobManager, IStorage storage)
        {
            var @params = new NamedParameterOverloads
            {
                {"daemonClient", daemonClient},
                {"jobManager", jobManager},
                {"storage", storage}
            };

            return _applicationContext.Container.Resolve<IShareManager>(@params);
        }
    }
}
