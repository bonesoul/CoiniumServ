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

using System.IO;
using System.Net;
using System.Text;
using Coinium.Core.Mining;
using Jayrock.JsonRpc;

namespace Coinium.Core.ClassicJayrock
{
    public class ClassicJayrockMiner : IMiner
    {
        private JsonRpcDispatcher _dispatcher;

        public ClassicJayrockMiner()
        {
            this.Authenticated = false;
            this._dispatcher = JsonRpcDispatcherFactory.CreateDispatcher(new ClassicJayrockService());
        }

        public void Parse(HttpListenerContext httpContext)
        {
            var httpRequest = httpContext.Request;
            var encoding = Encoding.UTF8;

            using (var reader = new StreamReader(httpRequest.InputStream, encoding))
            using (var writer = new StringWriter())
            {
                this._dispatcher.Process(reader, writer);
                writer.Flush();
                var response = httpContext.Response;
                response.ContentType = "application/json";
                response.ContentEncoding = encoding;
                var buffer = encoding.GetBytes(writer.GetStringBuilder().ToString());
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
            }
        }

        public bool Authenticated { get; private set; }
        public bool Authenticate(string user, string password)
        {
            this.Authenticated = true;
            return this.Authenticated;
        }
    }
}
