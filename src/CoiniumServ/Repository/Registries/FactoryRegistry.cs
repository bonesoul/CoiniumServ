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

using CoiniumServ.Coin.Config;
using CoiniumServ.Crypto.Algorithms;
using CoiniumServ.Mining.Jobs.Manager;
using CoiniumServ.Mining.Jobs.Tracker;
using CoiniumServ.Mining.Miners;
using CoiniumServ.Mining.Pools;
using CoiniumServ.Mining.Pools.Config;
using CoiniumServ.Mining.Pools.Statistics;
using CoiniumServ.Mining.Shares;
using CoiniumServ.Mining.Vardiff;
using CoiniumServ.Payments;
using CoiniumServ.Persistance;
using CoiniumServ.Repository.Context;
using CoiniumServ.Server;
using CoiniumServ.Server.Mining.Service;
using CoiniumServ.Utils.Configuration;

namespace CoiniumServ.Repository.Registries
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
