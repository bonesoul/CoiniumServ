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
using Coinium.Coin.Config;
using Coinium.Crypto.Algorithms;
using Coinium.Mining.Jobs.Manager;
using Coinium.Mining.Jobs.Tracker;
using Coinium.Mining.Miners;
using Coinium.Mining.Pools;
using Coinium.Mining.Pools.Config;
using Coinium.Mining.Pools.Statistics;
using Coinium.Mining.Shares;
using Coinium.Mining.Vardiff;
using Coinium.Payments;
using Coinium.Persistance;
using Coinium.Repository.Context;
using Coinium.Server;
using Coinium.Server.Mining.Service;
using Coinium.Utils.Configuration;

namespace Coinium.Repository.Registries
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
            _applicationContext.Container.Register<IPoolManagerFactory, PoolManagerFactory>().AsSingleton();
            _applicationContext.Container.Register<IHashAlgorithmFactory, HashAlgorithmFactory>().AsSingleton();
            _applicationContext.Container.Register<IPoolFactory, PoolFactory>().AsSingleton();
            _applicationContext.Container.Register<IServerFactory, ServerFactory>().AsSingleton();
            _applicationContext.Container.Register<IServiceFactory, ServiceFactory>().AsSingleton();
            _applicationContext.Container.Register<IJobManagerFactory, JobManagerFactory>().AsSingleton();
            _applicationContext.Container.Register<IJobTrackerFactory, JobTrackerFactory>().AsSingleton();
            _applicationContext.Container.Register<IShareManagerFactory, ShareManagerFactory>().AsSingleton();
            _applicationContext.Container.Register<IMinerManagerFactory, MinerManagerFactory>().AsSingleton();
            _applicationContext.Container.Register<ICoinConfigFactory, CoinConfigFactory>().AsSingleton();
            _applicationContext.Container.Register<IPoolConfigFactory, PoolConfigFactory>().AsSingleton();
            _applicationContext.Container.Register<IGlobalConfigFactory, GlobalConfigFactory>().AsSingleton();
            _applicationContext.Container.Register<IStorageFactory, StorageFactory>().AsSingleton();
            _applicationContext.Container.Register<IPaymentProcessorFactory, PaymentProcessorFactory>().AsSingleton();
            _applicationContext.Container.Register<IStatisticsObjectFactory, StatististicsObjectFactory>().AsSingleton();
            _applicationContext.Container.Register<IVardiffManagerFactory, VardiffManagerFactory>().AsSingleton();
        }
    }
}
