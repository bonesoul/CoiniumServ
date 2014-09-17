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
using CoiniumServ.Blocks;
using CoiniumServ.Coin.Config;
using CoiniumServ.Configuration;
using CoiniumServ.Container.Context;
using CoiniumServ.Daemon;
using CoiniumServ.Jobs.Tracker;
using CoiniumServ.Markets;
using CoiniumServ.Payments;
using CoiniumServ.Payments.Accounter;
using CoiniumServ.Payments.Processor;
using CoiniumServ.Payments.Round;
using CoiniumServ.Pools;
using CoiniumServ.Server.Web;
using Nancy.Bootstrapper;

namespace CoiniumServ.Container.Registries
{
    public class ClassRegistry : IRegistry
    {
        private readonly IApplicationContext _applicationContext;

        public ClassRegistry(IApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public void RegisterInstances()
        {
            // per-pool objects
            _applicationContext.Container.Register<IPool, Pool>().AsMultiInstance();
            _applicationContext.Container.Register<IDaemonClient, DaemonClient>().AsMultiInstance();
            _applicationContext.Container.Register<IJobTracker, JobTracker>().AsMultiInstance();
            _applicationContext.Container.Register<IBlockProcessor, BlockProcessor>().AsMultiInstance();
            _applicationContext.Container.Register<INetworkInfo, NetworkInfo>().AsMultiInstance();
            _applicationContext.Container.Register<IBlocksCache, BlocksCache>().AsMultiInstance();

            // payment objects
            _applicationContext.Container.Register<IBlockAccounter, BlockAccounter>().AsMultiInstance();
            _applicationContext.Container.Register<INewPaymentRound, NewPaymentRound>().AsMultiInstance();
            _applicationContext.Container.Register<INewPaymentProcessor, NewPaymentProcessor>().AsMultiInstance();

            // config
            _applicationContext.Container.Register<IPoolConfig, PoolConfig>().AsMultiInstance();
            _applicationContext.Container.Register<ICoinConfig, CoinConfig>().AsMultiInstance();
            _applicationContext.Container.Register<IDaemonManagerConfig, DaemonManagerConfig>().AsMultiInstance();
            _applicationContext.Container.Register<IStandaloneDaemonConfig, StandaloneDaemonConfig>().AsMultiInstance();
            _applicationContext.Container.Register<IJsonConfigReader, JsonConfigReader>().AsSingleton();

            // web
            _applicationContext.Container.Register<INancyBootstrapper, WebBootstrapper>().AsSingleton();

            // markets
            _applicationContext.Container.Register<IMarketManager, MarketManager>().AsSingleton();
        }
    }
}