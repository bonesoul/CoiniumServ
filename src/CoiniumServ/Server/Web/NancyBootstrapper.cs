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
using Metrics;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.CustomErrors;
using Nancy.TinyIoc;

namespace CoiniumServ.Server.Web
{
    public class NancyBootstrapper : DefaultNancyBootstrapper
    {
        private readonly IApplicationContext _applicationContext;

        private readonly IConfigManager _configManager;

        public NancyBootstrapper(IApplicationContext applicationContext, IConfigManager configManager)
        {
            _applicationContext = applicationContext;
            _configManager = configManager;
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            // todo: enable authentication support
            if (_configManager.WebServerConfig.Backend.MetricsEnabled)
                Metric.Config.WithNancy(config => config.WithMetricsModule("/admin/metrics"));

            CustomErrors.Enable(pipelines, new ErrorConfiguration());
        }

        protected override IRootPathProvider RootPathProvider
        {
            get { return new RootPathProvider(_configManager.WebServerConfig.Template); }
        }

        protected override TinyIoCContainer GetApplicationContainer()
        {
            return _applicationContext.Container;
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            // prevents nancy from autoregistering it's own set of resolvers.
        }
    }
}
