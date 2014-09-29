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

using CoiniumServ.Container.Context;
using CoiniumServ.Persistance.Layers;
using CoiniumServ.Persistance.Layers.Hybrid;
using CoiniumServ.Persistance.Layers.Mpos;
using CoiniumServ.Persistance.Layers.Null;
using CoiniumServ.Persistance.Providers;
using CoiniumServ.Persistance.Providers.MySql;
using CoiniumServ.Persistance.Providers.Redis;

namespace CoiniumServ.Container.Registries
{
    public class StorageRegistry: IRegistry
    {
        private readonly IApplicationContext _applicationContext;

        public StorageRegistry(IApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public void RegisterInstances()
        {
            // providers
            _applicationContext.Container.Register<IStorageProvider, MySqlProvider>(StorageProviders.MySql).AsMultiInstance();
            _applicationContext.Container.Register<IStorageProvider, RedisProvider>(StorageProviders.Redis).AsMultiInstance();

            // layers
            _applicationContext.Container.Register<IStorageLayer, HybridStorage>(StorageLayers.Hybrid).AsMultiInstance();
            _applicationContext.Container.Register<IStorageLayer, MposStorage>(StorageLayers.Mpos).AsMultiInstance();
            _applicationContext.Container.Register<IStorageLayer, NullStorage>(StorageLayers.Empty).AsSingleton();

            // migrators
            _applicationContext.Container.Register<IMigrationManager, MigrationManager>().AsMultiInstance();
        }
    }
}
