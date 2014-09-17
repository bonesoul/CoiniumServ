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
using System.IO;
using System.Net;
using System.Net.Sockets;
using Nancy.Bootstrapper;
using Nancy.Hosting.Self;
using Serilog;

namespace CoiniumServ.Server.Web
{
    public class HttpServer : IServer, IDisposable
    {
        /// <summary>
        /// The IP address of the interface the server binded.
        /// </summary>
        public string BindInterface { get; protected set; }

        /// <summary>
        /// The listening port for the server.
        /// </summary>
        public int Port { get; protected set; }

        /// <summary>
        /// Is server currently listening for connections?
        /// </summary>
        public bool IsListening { get; protected set; }

        private readonly INancyBootstrapper _webBootstrapper;

        private readonly ILogger _logger;

        public HttpServer(INancyBootstrapper webBootstrapper)
        {
            _webBootstrapper = webBootstrapper;
            _logger = Log.ForContext<HttpServer>();
        }

        public bool Start()
        {
            // if BindInterface is configured as empty, nacy to listen on all available interfaces thanks to RewriteLocalhost
            var uri = new Uri(string.Format("http://{0}:{1}", BindInterface, Port));            

            var hostConfiguration = new HostConfiguration
            {
                UrlReservations = {CreateAutomatically = true},
                RewriteLocalhost = true
            };

            hostConfiguration.UnhandledExceptionCallback += UnhandledExceptionHandler;

            try
            {
                var host = new NancyHost(_webBootstrapper, hostConfiguration, uri); // create nancy host.
                host.Start();
                IsListening = true;
            }
            catch (InvalidOperationException e)
            {
                // nancy requires elevated privileges to run on port well known ports - thrown when we are on Windows.
                _logger.Error("Need elevated privileges to listen on port {0}, try running as administrator - {1:l}",
                    Port, e.Message);
                IsListening = false;
                return false;
            }
            catch (SocketException e)
            {
                // nancy requires elevated privileges to run on port well known ports - thrown when we are on mono.
                _logger.Error("Need elevated privileges to listen on port {0}, try running as root - {1:l}", Port,
                    e.Message);
                IsListening = false;
                return false;
            }
            catch (HttpListenerException e)
            {
                _logger.Error("Can not listen on requested interface: {0:l} - {1:l}", BindInterface, e.Message);
                IsListening = false;
                return false;
            }
            catch (Exception e)
            {
                var baseException = e.GetBaseException();

                if (baseException is DirectoryNotFoundException)
                    _logger.Error("Invalid template path for web-server given; {0:l}", baseException.Message);
                else
                    _logger.Error("An error occured while starting web-server: {0:l}", e);
                IsListening = false;
                return false;
            }

            _logger.Information("Web-server listening on: {0:l}", uri);
            return true;
        }

        public bool Stop()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Unhandled exception callback for nancy based web-server.
        /// </summary>
        /// <param name="e"></param>
        private void UnhandledExceptionHandler(Exception e)
        {
            #if DEBUG
                _logger.Error("Unhandled web-server exception: {0:l}", e);
            #else
                _logger.Error("Unhandled web-server exception: {0:l}", e.Message);
            #endif
        }

        public void Dispose()
        {
            Stop();
        }
    }    
}
