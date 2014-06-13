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

using System.Net;
using Newtonsoft.Json;

namespace Coinium.Rpc.Service.Http
{
    /// <summary>
    /// JsonRpc 1.0 over http request.
    /// </summary>
    public class HttpServiceRequest
    {
        public string Text { get; private set; }

        public dynamic Data { get; private set; }

        public HttpListenerContext Context { get; private set; }

        public HttpListenerResponse Response { get; private set; }

        public HttpServiceRequest(string text, HttpListenerContext context)
        {
            Text = text;
            Data = JsonConvert.DeserializeObject<dynamic>(Text);
            Context = context;
            Response = Context.Response;
        }
    }
}
