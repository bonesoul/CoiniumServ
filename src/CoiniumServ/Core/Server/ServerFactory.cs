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

using Coinium.Core.Context;
using Coinium.Core.Mining.Miner;
using Ninject;

namespace Coinium.Core.Server
{
    public class ServerFactory : IServerFactory
    {
        private readonly IApplicationContext _applicationContext;

        public ServerFactory(IApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        /// <summary>
        /// Gets the specified service name.
        /// </summary>
        /// <param name="serviceName">Name of the service.</param>
        /// <param name="minerManager">The miner manager.</param>
        /// <returns></returns>
        public IMiningServer Get(string serviceName, IMinerManager minerManager)
        {
            var minerManagerParam = new Ninject.Parameters.ConstructorArgument("minerManager", minerManager);
            return _applicationContext.Kernel.Get<IMiningServer>(serviceName, minerManagerParam);
        }
    }
}
