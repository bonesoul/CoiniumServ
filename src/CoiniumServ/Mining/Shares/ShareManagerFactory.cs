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
            Log.Debug("ShareManagerFactory() init..");
            _applicationContext = applicationContext;
        }

        /// <summary>
        /// Gets the specified daemon client.
        /// </summary>
        /// <param name="hashAlgorithm">The hash algorithm.</param>
        /// <param name="jobManager">The job manager.</param>
        /// <param name="daemonClient"></param>
        /// <returns></returns>
        public IShareManager Get(IJobManager jobManager, IDaemonClient daemonClient)
        {
            var @params = new NamedParameterOverloads
            {
                {"jobManager", jobManager},
                {"daemonClient", daemonClient}
            };

            return _applicationContext.Container.Resolve<IShareManager>(@params);
        }
    }
}
