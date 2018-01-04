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
using CoiniumServ.Container.Context;
using CoiniumServ.Pools;
using CoiniumServ.Statistics;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.CustomErrors;
using Nancy.TinyIoc;
using Nancy.Diagnostics;

namespace CoiniumServ.Server.Web
{
    public class NancyBootstrapper : DefaultNancyBootstrapper
    {
        private readonly IApplicationContext _applicationContext;

        private readonly IStatisticsManager _statisticsManager;
        private readonly IPoolManager _poolManager;
        private readonly IConfigManager _configManager;

        public NancyBootstrapper(IApplicationContext applicationContext, IStatisticsManager statisticsManager, IPoolManager poolManager, IConfigManager configManager)
        {
            _applicationContext = applicationContext;
            _statisticsManager = statisticsManager;
            _poolManager = poolManager;
            _configManager = configManager;
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            // intercept the request and fill common ViewBag data.
            pipelines.AfterRequest += (ctx) =>
            {
                ctx.ViewBag.StackName = _configManager.StackConfig.Name;

                if (string.IsNullOrEmpty(ctx.ViewBag.Header))
                    ctx.ViewBag.Header = "";

                ctx.ViewBag.Title = string.IsNullOrEmpty(ctx.ViewBag.Header)
                    ? _configManager.StackConfig.Name
                    : string.Format("{0} - {1}", _configManager.StackConfig.Name, ctx.ViewBag.Header);

                if(string.IsNullOrEmpty(ctx.ViewBag.SubHeading))
                    ctx.ViewBag.SubHeading = "";

                ctx.ViewBag.Pools = _poolManager;
                ctx.ViewBag.Feed = _configManager.WebServerConfig.Feed;
                ctx.ViewBag.LastUpdate = _statisticsManager.LastUpdate.ToString("HH:mm:ss tt zz"); // last statistics update.
            };

            CustomErrors.Enable(pipelines, new ErrorConfiguration());
        }

        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            base.ConfigureConventions(nancyConventions);

            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddFile("/favicon.ico", "/Content/favicon.ico"));
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddFile("/robots.txt","/Content/robots.txt"));
        }

        protected override IRootPathProvider RootPathProvider
        {
            get { return new RootPathProvider(_configManager.WebServerConfig.Template); }
        }

        protected override TinyIoCContainer GetApplicationContainer()
        {
            return _applicationContext.Container;
        }

        #if DEBUG // on debug mode, enable http://website/_Nancy/
        protected override DiagnosticsConfiguration DiagnosticsConfiguration
        {
            get { return new DiagnosticsConfiguration { Password = @"debug" }; }
        }
        #endif

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            // prevents nancy from autoregistering it's own set of resolvers.
        }
    }
}
