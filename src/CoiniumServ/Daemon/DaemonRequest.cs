#region License
// 
//     CoiniumServ - Crypto Currency Mining Pool Server Software
//     Copyright (C) 2013 - 2014, CoiniumServ Project - http://www.coinium.org
//     http://www.coiniumserv.com - https://github.com/CoiniumServ/CoiniumServ
// 
//     This software is dual-licensed: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
//    
//     For the terms of this license, see licenses/gpl_v3.txt.
// 
//     Alternatively, you can license this software under a commercial
//     license or white-label it as set out in licenses/commercial.txt.
// 
#endregion

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace CoiniumServ.Daemon
{
    /// <summary>
    /// Class containing data sent to the coin wallet as a JSON RPC call.
    /// </summary>
    public class DaemonRequest
    {
        /// <summary>
        /// The method to call on the coin wallet.
        /// </summary>
        [JsonProperty(PropertyName = "method", Order = 0)]
        public string Method { get; set; }

        /// <summary>
        /// A list of parameters to pass to the method.
        /// </summary>
        [JsonProperty(PropertyName = "params", Order = 1)]
        public IList<object> Parameters { get; set; }

        /// <summary>
        /// Id of the RPC call. This id will be returned in the response.
        /// </summary>
        [JsonProperty(PropertyName = "id", Order = 2)]
        public int Id { get; set; }

        /// <summary>
        /// Create a new JSON RPC request with the given id, method and optionally parameters.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        public DaemonRequest(int id, string method, params object[] parameters)
        {
            Id = id;
            Method = method;

            Parameters = parameters != null ? parameters.ToList() : new List<object>();
        }

        /// <summary>
        /// Get the bytes of the JSON representation of this object.
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            string json = JsonConvert.SerializeObject(this);
            return Encoding.UTF8.GetBytes(json);
        }
    }
}
