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
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.CustomErrors;
using Nancy.Diagnostics;
using Nancy.TinyIoc;

namespace CoiniumServ.Net.Server.Http.Web
{
    public class WebBootstrapper : DefaultNancyBootstrapper
    {
        /// <summary>
        /// The _application context
        /// </summary>
        private readonly IApplicationContext _applicationContext;

        public WebBootstrapper(IApplicationContext applicationContext)
        {            
            _applicationContext = applicationContext;            
        }

        protected override DiagnosticsConfiguration DiagnosticsConfiguration
        {
            get { return new DiagnosticsConfiguration { Password = @"coinium" }; }
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            StaticConfiguration.EnableRequestTracing = true;
            CustomErrors.Enable(pipelines, new ErrorConfiguration());
        }

        protected override IRootPathProvider RootPathProvider
        {
            get { return new CustomRootPathProvider(); }
        }

        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            // static content
            nancyConventions.StaticContentsConventions.Clear();
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("css", "/css"));
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("js", "/js"));
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("font-awesome", "/font-awesome"));

            // view location
            nancyConventions.ViewLocationConventions.Clear();
            nancyConventions.ViewLocationConventions.Add((viewName, model, context) => string.Concat("/", viewName));
            nancyConventions.ViewLocationConventions.Add((viewName, model, context) => string.Concat("/", context.ModuleName, "/", viewName));
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
