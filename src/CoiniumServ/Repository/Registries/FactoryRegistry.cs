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
using CoiniumServ.Factories;
using CoiniumServ.Mining.Banning;
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
            _applicationContext.Container.Register<IObjectFactory, ObjectFactory>().AsSingleton();
            _applicationContext.Container.Register<IConfigFactory, ConfigFactory>().AsSingleton();

            _applicationContext.Container.Register<ICoinConfigFactory, CoinConfigFactory>().AsSingleton();
            _applicationContext.Container.Register<IPoolConfigFactory, PoolConfigFactory>().AsSingleton();
        }
    }
}
