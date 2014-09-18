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
using CoiniumServ.Pools;
using CoiniumServ.Server.Web.Models;
using Nancy;
using Nancy.CustomErrors;

namespace CoiniumServ.Server.Web.Modules
{
    public class PoolModule : NancyModule
    {
        public PoolModule(IPoolManager poolManager)
            :base("/pool")
        {
            Get["/{slug}"] = _ =>
            {
                var pool = poolManager.Get(_.slug); // find the requested pool. TODO: use IStatistics instead

                if (pool == null) // make sure queried pool exists.
                {
                    ViewBag.Title = "Error";
                    ViewBag.Heading = "An error occured!";

                    return View["error", new ErrorViewModel
                    {
                        Summary = "Pool not found",
                        Details = string.Format("The requested pool does not exist: {0}", _.slug)
                    }];
                }
             
                ViewBag.Title = string.Format("{0} Pool", pool.Config.Coin.Name);
                ViewBag.Heading = string.Format("{0} Pool", pool.Config.Coin.Name);

                // return our view
                return View["pool", new PoolModel
                {
                    Pool = pool
                }];
            };

            Get["/{slug}/workers"] = _ =>
            {
                var pool = poolManager.Get(_.slug); // find the requested pool.                

                if (pool == null) // make sure queried pool exists.
                {
                    ViewBag.Title = "Error";
                    ViewBag.Heading = "An error occured!";

                    return View["error", new ErrorViewModel
                    {
                        Summary = "Pool not found",
                        Details = string.Format("The requested pool does not exist: {0}", _.slug)
                    }];
                }

                ViewBag.Title = string.Format("{0} Workers", pool.Config.Coin.Name);
                ViewBag.Heading = string.Format("{0} Workers", pool.Config.Coin.Name);

                // return our view
                return View["workers", new WorkersModel
                {
                    Workers = pool.MinerManager.Miners
                }];
            };

            Get["/{slug}/round"] = _ =>
            {
                var pool = poolManager.Get(_.slug); // find the requested pool.                

                if (pool == null) // make sure queried pool exists.
                {
                    ViewBag.Title = "Error";
                    ViewBag.Heading = "An error occured!";

                    return View["error", new ErrorViewModel
                    {
                        Summary = "Pool not found",
                        Details = string.Format("The requested pool does not exist: {0}", _.slug)
                    }];
                }

                ViewBag.Title = string.Format("{0} Current Round", pool.Config.Coin.Name);
                ViewBag.Heading = string.Format("{0} Current Round", pool.Config.Coin.Name);

                // return our view
                return View["round", new RoundModel
                {
                    Round = pool.NetworkInfo.Round,
                    Shares = pool.RoundShares
                }];
            };
        }
    }
}
