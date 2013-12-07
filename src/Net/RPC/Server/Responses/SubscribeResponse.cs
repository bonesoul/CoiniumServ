/*
 *   Coinium project - crypto currency pool software - https://github.com/raistlinthewiz/coinium
 *   Copyright (C) 2013 Hüseyin Uslu, Int6 Studios - http://www.coinium.org
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


using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace coinium.Net.RPC.Server.Responses
{
    [JsonArray]
    public class SubscribeResponse:IEnumerable<object>
    {
        [JsonIgnore]   
        public string UniqueId { get; set; }

        [JsonIgnore]   
        public string ExtraNonce1 { get; set; }

        [JsonIgnore]   
        public int ExtraNonce2 { get; set; }

        public IEnumerator<object> GetEnumerator()
        {
            var data = new List<object>
            {
                new List<string>
                {
                    "mining.notify",
                    this.UniqueId
                },
                this.ExtraNonce1,
                this.ExtraNonce2
            };

            return data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
