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
using Coinium.Core.RPC.Service;
using Coinium.Core.Server.Stratum;
using Coinium.Core.Server.Vanilla;

namespace Coinium.Core.Repository.Registries
{
    public class ServiceRegistry:IRegistry
    {
        private readonly IApplicationContext _applicationContext;

        public ServiceRegistry(IApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public void RegisterInstances()
        {
            _applicationContext.Kernel.Bind<IRpcService>().To<VanillaService>().Named(RpcServiceNames.Vanilla);
            _applicationContext.Kernel.Bind<IRpcService>().To<StratumService>().Named(RpcServiceNames.Stratum);
        }
    }
}
