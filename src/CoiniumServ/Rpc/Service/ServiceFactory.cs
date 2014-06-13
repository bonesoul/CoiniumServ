/*
 *   CoiniumServ - crypto currency pool software - https://github.com/CoiniumServ/CoiniumServ
 *   Copyright (C) 2013 - 2014, Coinium Project - http://www.coinium.org
 *
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using Coinium.Coin.Daemon;
using Coinium.Common.Context;
using Coinium.Mining.Jobs;
using Coinium.Mining.Share;
using Nancy.TinyIoc;

namespace Coinium.Rpc.Service
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
        /// <param name="jobManager">The job manager.</param>
        /// <param name="shareManager">The share manager.</param>
        /// <param name="daemonClient">The daemon client.</param>
        /// <returns></returns>
        public IRpcService Get(string serviceName, IJobManager jobManager, IShareManager shareManager, IDaemonClient daemonClient)
        {
            var @params = new NamedParameterOverloads { { "jobManager", jobManager }, { "shareManager", shareManager }, { "daemonClient", daemonClient } };
            return _applicationContext.Container.Resolve<IRpcService>(serviceName, @params);
        }
    }
}
