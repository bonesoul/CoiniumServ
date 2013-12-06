/*
 * Coinium project, Copyright (C) 2013, Int6 Studios - All Rights Reserved. - http://www.coinium.org
 *
 */

using Newtonsoft.Json;

namespace coinium.Net.RPC.Client
{
    /// <summary>
    /// Class containing the information contained in a JSON RPC response received from
    /// the Bitcoin wallet.
    /// </summary>
    /// <typeparam name="T">
    /// Type of the result object. This can simply be a string like a transaction id, 
    /// or a more complex object like TransactionDetails.
    /// </typeparam>
    public class JsonRpcResponse<T>
    {
        /// <summary>
        /// The result object.
        /// </summary>
        [JsonProperty(PropertyName = "result", Order = 0)]
        public T Result { get; set; }

        /// <summary>
        /// The id of the corresponding request.
        /// </summary>
        [JsonProperty(PropertyName = "id", Order = 1)]
        public int Id { get; set; }

        /// <summary>
        /// The error returned by the wallet, if any.
        /// </summary>
        [JsonProperty(PropertyName = "error", Order = 2)]
        public string Error { get; set; }

        /// <summary>
        /// Create a new JSON RPC response with the given id, error and result object.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="error">The error.</param>
        /// <param name="result">The result object.</param>
        public JsonRpcResponse(int id, string error, T result)
        {
            Id = id;
            Error = error;
            Result = result;
        }
    }
}
