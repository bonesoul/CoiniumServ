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

using CoiniumServ.Repository.Context;
using CoiniumServ.Server.Mining;
using CoiniumServ.Server.Mining.Service;
using CoiniumServ.Server.Mining.Stratum;
using CoiniumServ.Server.Mining.Stratum.Service;
using CoiniumServ.Server.Mining.Vanilla;
using CoiniumServ.Server.Mining.Vanilla.Service;
using CoiniumServ.Server.Web;

namespace CoiniumServ.Repository.Registries
{
    public class ServerRegistry : IRegistry
    {
        private readonly IApplicationContext _applicationContext;

        public ServerRegistry(IApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public void RegisterInstances()
        {
            // servers.
            _applicationContext.Container.Register<IMiningServer, VanillaServer>(Services.Vanilla).AsMultiInstance();
            _applicationContext.Container.Register<IMiningServer, StratumServer>(Services.Stratum).AsMultiInstance();
            _applicationContext.Container.Register<IWebServer, WebServer>().AsSingleton();

            // services.
            _applicationContext.Container.Register<IRpcService, VanillaService>(Services.Vanilla).AsMultiInstance();
            _applicationContext.Container.Register<IRpcService, StratumService>(Services.Stratum).AsMultiInstance();
        }
    }
}
