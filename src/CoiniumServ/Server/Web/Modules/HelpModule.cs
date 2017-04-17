#region License
// 
//     MIT License
//
//     CoiniumServ - Crypto Currency Mining Pool Server Software
//     Copyright (C) 2013 - 2017, CoiniumServ Project
//     Hüseyin Uslu, shalafiraistlin at gmail dot com
//     https://github.com/bonesoul/CoiniumServ
// 
//     Permission is hereby granted, free of charge, to any person obtaining a copy
//     of this software and associated documentation files (the "Software"), to deal
//     in the Software without restriction, including without limitation the rights
//     to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//     copies of the Software, and to permit persons to whom the Software is
//     furnished to do so, subject to the following conditions:
//     
//     The above copyright notice and this permission notice shall be included in all
//     copies or substantial portions of the Software.
//     
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//     FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//     LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//     OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//     SOFTWARE.
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
