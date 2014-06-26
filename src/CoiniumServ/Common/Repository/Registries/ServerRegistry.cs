#region License
// 
//     CoiniumServ - crypto currency pool software - https://github.com/CoiniumServ/CoiniumServ
//     Copyright (C) 2013 - 2014, Coinium Project - http://www.coinium.org
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
using Coinium.Rpc.Service;
using Coinium.Server;
using Coinium.Server.Stratum;
using Coinium.Server.Vanilla;

namespace Coinium.Common.Repository.Registries
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
            _applicationContext.Container.Register<IMiningServer, VanillaServer>(RpcServiceNames.Vanilla).AsMultiInstance();
            _applicationContext.Container.Register<IMiningServer, StratumServer>(RpcServiceNames.Stratum).AsMultiInstance();
        }
    }
}
