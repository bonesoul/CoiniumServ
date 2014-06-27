#region License
// 
//     CoiniumServ - Crypto Currency Mining Pool Server Software
//     Copyright (C) 2013 - 2014, CoiniumServ Project - http://www.coinium.org
//     https://github.com/CoiniumServ/CoiniumServ
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

using Coinium.Common.Context;
using Nancy.TinyIoc;
using Serilog;

namespace Coinium.Persistance
{
    public class StorageFactory:IStorageFactory
    {

        /// <summary>
        /// The _kernel
        /// </summary>
        private readonly IApplicationContext _applicationContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageFactory" /> class.
        /// </summary>
        /// <param name="applicationContext">The application context.</param>
        public StorageFactory(IApplicationContext applicationContext)
        {
            Log.Debug("StorageManagerFactory() init..");
            _applicationContext = applicationContext;
        }

        public IStorage Get(string storageName)
        {
            // Default to redis
            if (string.IsNullOrWhiteSpace(storageName))
                storageName = Storages.Redis;

            return _applicationContext.Container.Resolve<IStorage>(storageName);
        }
    }
}
