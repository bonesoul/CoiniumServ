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
using System.Net;
using System.Threading.Tasks;

namespace Coinium.Net.Http
{
    // based on sample code from: https://gist.github.com/atifaziz/5940164

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

            if (!HttpListener.IsSupported)
                throw new NotSupportedException("HttpListener not supported. Switch to mono provided one.");

            listener.Prefixes.Add(string.Format("http://localhost:{0}/", this.Port));

            listener.Start();

            var tcs = new TaskCompletionSource<object>();

            listener.GetContextAsync().ContinueWith(async t =>
            {
                try
                {
                    while (true)
                    {
                        var context = await t;
                        this.HttpRequestRecieved(new HttpRequestEventArgs(context));
                        t = listener.GetContextAsync();
                    }
                }
                catch (Exception e)
                {
                    listener.Close();
                    tcs.TrySetException(e);
                }
            });
        }
        protected virtual void HttpRequestRecieved(HttpRequestEventArgs e)
        {
            var handler = OnHttpRequest;
            if (handler != null)
                handler(this, e);
        }
    }
}
