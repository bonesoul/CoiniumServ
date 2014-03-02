/*
 *   Coinium - Crypto Currency Pool Software - https://github.com/CoiniumServ/coinium
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Mono.Net;
using Serilog;

namespace Coinium.Net.Http
{
    public class HttpServer
    {
        public int Port { get; private set; }

        public delegate void HttpRequestEventHandler(object sender, HttpRequestEventArgs e);
        public event HttpRequestEventHandler OnHttpRequest;

        public HttpServer(int port)
        {
            this.Port = port;
        }

        public void Listen()
        {
            var listener = new HttpListener();

            Log.Verbose("Http-Server starting to listen on port {0}", this.Port);

            listener.Prefixes.Add(string.Format("http://+:{0}/",this.Port));
            listener.Start();

            try
            {
                while (true)
                {
                    IAsyncResult result = listener.BeginGetContext(new AsyncCallback(ProcessHttpRequest), listener);
                    result.AsyncWaitHandle.WaitOne();
                }
            }
            catch (Exception)
            {
                listener.Stop();
            }
        }

        private void ProcessHttpRequest(IAsyncResult result)
        {
            var listener = (HttpListener) result.AsyncState;

            // Call EndGetContext to complete the asynchronous operation.
            var context = listener.EndGetContext(result);
            var request = context.Request;

            var encoding = Encoding.UTF8;

            using (var reader = new StreamReader(request.InputStream, encoding))
            {
                var data = reader.ReadToEnd();
                var response = context.Response;
                response.ContentType = "application/json-rpc";
                response.ContentEncoding = encoding;

                this.HttpRequestRecieved(new HttpRequestEventArgs(data, response));
            }
        }

        protected virtual void HttpRequestRecieved(HttpRequestEventArgs e)
        {
            var handler = OnHttpRequest;
            if (handler != null)
                handler(this, e);
        }

    }
}
