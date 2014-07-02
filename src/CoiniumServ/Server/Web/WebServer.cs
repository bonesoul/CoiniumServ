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
using System;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Diagnostics;
using Nancy.Hosting.Self;
using Nancy.TinyIoc;
using Serilog;

namespace Coinium.Server.Web
{
    public class WebServer
    {
        public int Port { get; private set; }

        public string Interface { get; private set; }

        /// <summary>
        /// Inits a new instance of embedded web-server.
        /// </summary>
        public WebServer()
        {
            //this.Interface = Config.Instance.Interface;
            //this.Port = Config.Instance.Port;
        }

        public void Start()
        {
            var uri = new Uri(string.Format("http://{0}:{1}", Interface, Port));
            Log.ForContext<WebServer>().Verbose("Web-server listening on: {0}", uri);

            var hostConfiguration = new HostConfiguration();
            hostConfiguration.UnhandledExceptionCallback += UnhandledExceptionHandler;
            hostConfiguration.UrlReservations.CreateAutomatically = true;

            var host = new NancyHost(hostConfiguration, uri);

            try
            {
                host.Start();
            }
            catch (InvalidOperationException e) // nancy requires elevated privileges to run on port 80.
            {
                Log.ForContext<WebServer>().Error("Need elevated privileges to listen on port {0}. [Error: {1}].", Port, e);
                Environment.Exit(-1);
            }
        }

        /// <summary>
        /// Unhandled exception callback for nancy based web-server.
        /// </summary>
        /// <param name="exception"></param>
        private void UnhandledExceptionHandler(Exception exception)
        {
            Log.ForContext<WebServer>().Error("Web-server: {0}", exception);
        }
    }

    public class CustomBootstrapper : DefaultNancyBootstrapper
    {
        protected override DiagnosticsConfiguration DiagnosticsConfiguration
        {
            get { return new DiagnosticsConfiguration { Password = @"coinium" }; }
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            StaticConfiguration.EnableRequestTracing = true;
        }
    }
}
