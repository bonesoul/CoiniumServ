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
using Nancy.Hosting.Self;
using Serilog;

namespace Coinium.Net.Server.Nancy
{
    public class NancyServer : IServer, IDisposable
    {
        /// <summary>
        /// The IP address of the interface the server binded.
        /// </summary>
        public string BindIP { get; protected set; }

        /// <summary>
        /// The listening port for the server.
        /// </summary>
        public int Port { get; protected set; }

        /// <summary>
        /// Is server currently listening for connections?
        /// </summary>
        public bool IsListening { get; protected set; }

        public bool Start()
        {
            var uri = new Uri(string.Format("http://{0}:{1}", BindIP, Port));
            Log.ForContext<NancyServer>().Verbose("Web-server listening on: {0}", uri);

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
                Log.ForContext<NancyServer>().Error("Need elevated privileges to listen on port {0}. [Error: {1}].", Port, e);
                return false;
            }

            return true;
        }

        public bool Stop()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Unhandled exception callback for nancy based web-server.
        /// </summary>
        /// <param name="exception"></param>
        private void UnhandledExceptionHandler(Exception exception)
        {
            Log.ForContext<NancyServer>().Error("Web-server: {0}", exception);
        }

        public void Dispose()
        {
            Stop();
        }
    }    
}
