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
using System.Text;
using Coinium.Core.Mining;
using Coinium.Net.Http;
using Jayrock.JsonRpc;

namespace Coinium.Core.Getwork
{
    public class GetworkMiner : IMiner
    {
        public bool Authenticated { get; private set; }

        public GetworkMiner()
        {
            this.Authenticated = false;
        }

        public void Parse(HttpRequestEventArgs e)
        {
            var context = e.Context;
            var request = context.Request;

            var dispatcher = JsonRpcDispatcherFactory.CreateDispatcher(new GetworkService());
            var encoding = Encoding.UTF8;

            using (var reader = new StreamReader(request.InputStream, encoding))
            using (var writer = new StringWriter())
            {
                dispatcher.Process(reader, writer);
                writer.Flush();
                var response = context.Response;
                response.ContentType = "application/json";
                response.ContentEncoding = encoding;
                var buffer = encoding.GetBytes(writer.GetStringBuilder().ToString());
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
            }

        }

        public bool Authenticate(string user, string password)
        {
            this.Authenticated = true;
            return this.Authenticated;
        }
    }
}
