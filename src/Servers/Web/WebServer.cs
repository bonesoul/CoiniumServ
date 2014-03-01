using System;
using Nancy.Hosting.Self;
using Serilog;

namespace Coinium.Servers.Web
{
    public class WebServer
    {
        public int Port { get; private set; }

        /// <summary>
        /// Inits a new instance of embedded web-server.
        /// </summary>
        public WebServer(int port)
        {            
            this.Port = port;
            var uri = new Uri(string.Format("http://127.0.0.1:{0}", this.Port));

            Log.Verbose("Init WebServer() - {0}", uri);

            var hostConfiguration = new HostConfiguration();
            hostConfiguration.UnhandledExceptionCallback += UnhandledExceptionHandler;
            hostConfiguration.UrlReservations.CreateAutomatically = true;

            using (var host = new NancyHost(hostConfiguration, uri))
            {                
                host.Start();
                Console.ReadLine();
                host.Stop();
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
}
