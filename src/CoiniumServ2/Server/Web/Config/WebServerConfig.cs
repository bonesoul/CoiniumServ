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

using System;
using Serilog;

namespace CoiniumServ.Server.Web.Config
{
    public class WebServerConfig : IWebServerConfig
    {
        public bool Enabled { get; private set; }

        public string BindInterface { get; private set; }

        public int Port { get; private set; }

        public string Template { get; private set; }

        public string Feed { get; private set; }

        public IBackendConfig Backend { get; private set; }

        public bool Valid { get; private set; }

        public WebServerConfig(dynamic config)
        {
            try
            {
                // load the config data.
                Enabled = config.enabled;
                BindInterface = string.IsNullOrEmpty(config.bind) ? "localhost" : config.bind;
                Port = config.port == 0 ? 80 : config.port;
                Template = string.IsNullOrEmpty(config.template) ? "default" : config.template;
                Feed = string.IsNullOrEmpty(config.feed) ? "" : config.feed;
                Backend = new BackendConfig(config.backend);
                Valid = true;
            }
            catch (Exception e)
            {
                Valid = false;
                Log.Logger.ForContext<WebServerConfig>().Error(e, "Error loading web-server configuration");
            }
        }
    }
}
