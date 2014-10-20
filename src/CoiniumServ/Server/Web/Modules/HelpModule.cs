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

using CoiniumServ.Configuration;
using CoiniumServ.Mining.Software;
using CoiniumServ.Pools;
using CoiniumServ.Server.Web.Models.GettingStarted;
using Nancy;
using Nancy.CustomErrors;
using Nancy.Helpers;

namespace CoiniumServ.Server.Web.Modules
{
    public class HelpModule:NancyModule
    {
        public HelpModule(IPoolManager poolManager, IConfigManager configManager, ISoftwareRepository softwareRepository)
            :base("/help")
        {
            Get["/faq"] = _ =>
            {
                ViewBag.Header = "Frequently Asked Questions";

                return View["faq"];
            };

            Get["/gettingstarted/"] = _ =>
            {
                var model = new GettingStartedModel
                {
                    Stack = configManager.StackConfig,
                    Pools = poolManager.GetAllAsReadOnly()
                };

                return View["gettingstarted/index", model];
            };

            Get["/gettingstarted/pool/{slug}"] = _ =>
            {
                var pool = poolManager.Get(HttpUtility.HtmlEncode(_.slug)); // find the requested pool.

                if (pool == null)
                {
                    return View["error", new ErrorViewModel
                    {
                        Details = string.Format("The requested pool does not exist: {0}", _.slug)
                    }];
                }

                var model = new GettingStartedPoolModel
                {
                    Stack = configManager.StackConfig,
                    Pool = pool
                };

                return View["gettingstarted/pool", model];
            };

            Get["/miningsoftware/"] = _ =>
            {
                return View["miningsoftware", softwareRepository];
            };
        }
    }
}
