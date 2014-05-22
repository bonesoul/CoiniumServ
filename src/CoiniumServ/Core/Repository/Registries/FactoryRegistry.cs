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

using Coinium.Core.Coin.Algorithms;
using Coinium.Core.Coin.Configs;
using Coinium.Core.Context;
using Coinium.Core.Mining.Jobs;
using Coinium.Core.Mining.Pool;
using Coinium.Core.Mining.Share;
using Coinium.Core.RPC.Service;
using Coinium.Core.Server;

namespace Coinium.Core.Repository.Registries
{
    public class FactoryRegistry : IRegistry
    {
        private readonly IApplicationContext _applicationContext;

        public FactoryRegistry(IApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public void RegisterInstances()
        {
            _applicationContext.Container.Register<IHashAlgorithmFactory, HashAlgorithmFactory>().AsSingleton();
            _applicationContext.Container.Register<IPoolFactory, PoolFactory>().AsSingleton();
            _applicationContext.Container.Register<IServerFactory, ServerFactory>().AsSingleton();
            _applicationContext.Container.Register<IServiceFactory, ServiceFactory>().AsSingleton();
            _applicationContext.Container.Register<IJobManagerFactory, JobManagerFactory>().AsSingleton();
            _applicationContext.Container.Register<IShareManagerFactory, ShareManagerFactory>().AsSingleton();
            _applicationContext.Container.Register<ICoinConfigFactory, CoinConfigFactory>().AsSingleton();
        }
    }
}
