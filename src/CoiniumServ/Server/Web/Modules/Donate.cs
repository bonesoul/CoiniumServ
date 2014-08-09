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

using System.Linq;
using CoiniumServ.Statistics;
using Nancy;

namespace CoiniumServ.Server.Web.Modules
{
    public class DonateModule : NancyModule
    {
        public DonateModule(IStatistics statistics)
        {
            Get["/donate/"] = _ =>
            {
                // include common data required by layout.
                ViewBag.Title = "Donation";
                ViewBag.Heading = "Donation";
                ViewBag.Pools = statistics.Pools.ToDictionary(x => x.Value.Config.Coin.Symbol, x => x.Value.Config.Coin.Name);
                ViewBag.LastUpdate = statistics.LastUpdate.ToString("HH:mm:ss tt zz"); // last statistics update.

                // return our view
                return View["donate"];
            };
        }
    }
}
