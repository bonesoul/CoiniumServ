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

using Newtonsoft.Json;

namespace Coinium.Rpc.Service.Socket
{
    /// <summary>
    /// JsonRpc 2.0 over sockets request.
    /// </summary>
    public class SocketServiceRequest
    {
        public string Text { get; private set; }

        public dynamic Data { get; private set; }

        public SocketServiceRequest(string text)
        {
            Text = text;
            Data = JsonConvert.DeserializeObject<dynamic>(Text);
        }
    }
}
