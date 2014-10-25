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
using CoiniumServ.Container.Context;
using CoiniumServ.Pools;
using CoiniumServ.Statistics;
using Metrics;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.CustomErrors;
using Nancy.Diagnostics;
using Nancy.TinyIoc;

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

            #if DEBUG
            // only enable metrics support in debug mode.
            if (_configManager.WebServerConfig.Backend.MetricsEnabled)
                Metric.Config.WithNancy(pipelines);
            #endif

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
