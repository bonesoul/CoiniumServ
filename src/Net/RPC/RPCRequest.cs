/*
 * Coinium project, Copyright (C) 2013, Int6 Studios - All Rights Reserved. - http://www.coinium.org
 *
 */

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace coinium.Net.RPC
{
    /// <summary>
    /// Class containing data sent to the Bitcoin wallet as a JSON RPC call.
    /// </summary>
    public class JsonRpcRequest
    {
        /// <summary>
        /// The method to call on the Bitcoin wallet.
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
        public JsonRpcRequest(int id, string method, params object[] parameters)
        {
            Id = id;
            Method = method;

            if (parameters != null)
            {
                Parameters = parameters.ToList<object>();
            }
            else
            {
                Parameters = new List<object>();
            }
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
