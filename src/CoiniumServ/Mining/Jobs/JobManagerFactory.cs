#region License
// 
//     CoiniumServ - crypto currency pool software - https://github.com/CoiniumServ/CoiniumServ
//     Copyright (C) 2013 - 2014, Coinium Project - http://www.coinium.org
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
using Coinium.Coin.Algorithms;
using Coinium.Coin.Daemon;
using Coinium.Common.Context;
using Coinium.Mining.Miners;
using Nancy.TinyIoc;
using Serilog;

namespace Coinium.Mining.Jobs
{
    public class JobManagerFactory : IJobManagerFactory
    {
        /// <summary>
        /// The _kernel
        /// </summary>
        private readonly IApplicationContext _applicationContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobManagerFactory"/> class.
        /// </summary>
        /// <param name="applicationContext">The application context.</param>
        public JobManagerFactory(IApplicationContext applicationContext)
        {
            Log.Debug("JobManagerFactory() init..");
            _applicationContext = applicationContext;
        }

        /// <summary>
        /// Gets the specified daemon client.
        /// </summary>
        /// <param name="daemonClient">The daemon client.</param>
        /// <param name="minerManager">The miner manager.</param>
        /// <param name="hashAlgorithm"></param>
        /// <returns></returns>
        public IJobManager Get(IDaemonClient daemonClient, IMinerManager minerManager, IHashAlgorithm hashAlgorithm)
        {
            var @params = new NamedParameterOverloads
            {
                {"daemonClient", daemonClient}, 
                {"minerManager", minerManager},
                {"hashAlgorithm", hashAlgorithm}
            };

            return _applicationContext.Container.Resolve<IJobManager>(@params);
        }
    }
}
