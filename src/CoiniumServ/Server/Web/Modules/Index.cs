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

using System.Collections.Generic;
using CoiniumServ.Pools;
using CoiniumServ.Statistics;
using Nancy;

namespace CoiniumServ.Server.Web.Modules
{
    public class IndexModule : NancyModule
    {
        public IndexModule(IPoolManager poolManager, IStatistics statistics)
        {
            Get["/"] = _ => View["index", new IndexModel
            {
                Pools = poolManager.Pools,
                Statistics = statistics,
            }];
        }
    }

    public class IndexModel
    {
        public IReadOnlyCollection<IPool> Pools { get; set; }

        public IStatistics Statistics { get; set; }
    }
}
