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
