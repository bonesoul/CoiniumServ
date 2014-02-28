using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy.Hosting.Self;
using Serilog;

namespace coinium.Net.Web
{
    public class WebServer
    {
        public int Port { get; private set; }

        /// <summary>
        /// Inits a new instance of embedded web-server.
        /// </summary>
        public WebServer(int port)
        {
            Log.Verbose("Init WebServer()");
            
            this.Port = port;
            var uri = new Uri(string.Format("http://127.0.0.1:{0}", this.Port));

            var hostConfiguration = new HostConfiguration();

            using (var host = new NancyHost(hostConfiguration, uri))
            {
                
                host.Start();
            }
        }
    }
}
