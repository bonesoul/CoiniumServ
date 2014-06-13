/*
 *   CoiniumServ - crypto currency pool software - https://github.com/CoiniumServ/CoiniumServ
 *   Copyright (C) 2013 - 2014, Coinium Project - http://www.coinium.org
 *
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

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
            Log.Verbose("Web-server listening on: {0}", uri);

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
                Log.Error("Need elevated privileges to listen on port {0}. [Error: {1}].", Port, e);
                Environment.Exit(-1);
            }
        }

        /// <summary>
        /// Unhandled exception callback for nancy based web-server.
        /// </summary>
        /// <param name="exception"></param>
        private void UnhandledExceptionHandler(Exception exception)
        {
            Log.Error("Web-server: {0}",exception);
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
