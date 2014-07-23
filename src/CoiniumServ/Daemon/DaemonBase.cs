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

using System;
using System.IO;
using System.Net;
using System.Text;
using CoiniumServ.Daemon.Config;
using CoiniumServ.Daemon.Exceptions;
using CoiniumServ.Factories;
using CoiniumServ.Logging;
using CoiniumServ.Utils.Extensions;
using Newtonsoft.Json;
using Serilog;

namespace CoiniumServ.Daemon
{
    public class DaemonBase : IDaemonBase
    {
        public string RpcUrl { get; private set; }
        public string RpcUser { get; private set; }
        public string RpcPassword { get; private set; }
        public Int32 RequestCounter { get; private set; }

        private readonly ILogger _logger;

        public DaemonBase(string pool, IDaemonConfig daemonConfig)
        {
            RpcUrl = string.Format("http://{0}:{1}", daemonConfig.Host, daemonConfig.Port);
            RpcUser = daemonConfig.Username;
            RpcPassword = daemonConfig.Password;
            RequestCounter = 0;
            _logger = LogManager.PacketLogger.ForContext<DaemonClient>().ForContext("Component", pool);
        }

        /// <summary>
        /// Make a a request. Invoke the given method with the given parameters.
        /// </summary>
        /// <typeparam name="T">
        /// Type to return as the response. 
        /// We will try to convert the JSON response to this type.
        /// </typeparam>
        /// <param name="method">Method to invoke.</param>
        /// <param name="parameters">Parameters to pass to the method.</param>
        /// <returns>The JSON RPC response deserialized as the given type.</returns>
        protected T MakeRequest<T>(string method, params object[] parameters)
        {
            var rpcResponse = MakeRpcRequest<T>(new DaemonRequest(RequestCounter++, method, parameters));
            return rpcResponse.Result;
        }

        /// <summary>
        /// Make an JSON RPC request, and return a JSON RPC response object with the result 
        /// deserialized as the given type.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the result as.</typeparam>
        /// <param name="walletRequest">The request object.</param>
        /// <returns>A JSON RPC response with the result deserialized as the given type.</returns>
        private DaemonResponse<T> MakeRpcRequest<T>(DaemonRequest walletRequest)
        {
            var httpWebRequest = MakeHttpRequest(walletRequest);
            return GetRpcResponse<T>(httpWebRequest);
        }

        /// <summary>
        /// Make a raw JSON RPC request with the given request object. Returns raw JSON.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public string MakeRawRequest(string method, params object[] parameters)
        {
            var response = MakeRawRpcRequest(new DaemonRequest(RequestCounter++, method, parameters));
            _logger.Verbose("rx: {0}", response.PrettifyJson());

            return response;
        }

        /// <summary>
        /// Make a raw JSON RPC request with the given request object. Returns raw JSON.
        /// </summary>
        /// <param name="walletRequest">The request object.</param>
        /// <returns>The raw JSON string.</returns>
        private string MakeRawRpcRequest(DaemonRequest walletRequest)
        {
            var httpWebRequest = MakeHttpRequest(walletRequest);
            return GetJsonResponse(httpWebRequest);
        }

        /// <summary>
        /// Make the actual HTTP request to the Bitcoin RPC interface.
        /// </summary>
        /// <param name="walletRequest">The request to make.</param>
        /// <returns>The HTTP request object.</returns>
        private HttpWebRequest MakeHttpRequest(DaemonRequest walletRequest)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(RpcUrl);
            webRequest.Credentials = new NetworkCredential(RpcUser, RpcPassword);

            // Important, otherwise the service can't deserialse your request properly
            webRequest.ContentType = "application/json-rpc";
            webRequest.Method = "POST";
            webRequest.Timeout = 1000;

            _logger.Verbose("tx: {0}", Encoding.UTF8.GetString(walletRequest.GetBytes()).PrettifyJson());

            byte[] byteArray = walletRequest.GetBytes();
            webRequest.ContentLength = byteArray.Length;

            try
            {
                using (Stream dataStream = webRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();
                }
            }
            catch (WebException exception)
            {
                throw new RpcException("An unknown web exception occured while trying to send the JSON request.", exception);
            }

            return webRequest;
        }

        /// <summary>
        /// Get the JSON RPC response for the given HTTP request.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the response result to.</typeparam>
        /// <param name="httpWebRequest">The web request.</param>
        /// <returns>A JSON RPC response with the result deserialized as the given type.</returns>
        private DaemonResponse<T> GetRpcResponse<T>(HttpWebRequest httpWebRequest)
        {
            string json = GetJsonResponse(httpWebRequest);

            _logger.Verbose("rx: {0}", json.PrettifyJson());

            try
            {
                return JsonConvert.DeserializeObject<DaemonResponse<T>>(json);
            }
            catch (JsonException jsonEx)
            {
                throw new Exception("There was a problem deserializing the response from the coin wallet.", jsonEx);
            }
        }

        /// <summary>
        /// Gets the JSON response for the given HTTP request.
        /// </summary>
        /// <param name="httpWebRequest">The HTTP request send to the Bitcoin RPC interface.</param>
        /// <returns>The raw JSON string.</returns>
        private string GetJsonResponse(HttpWebRequest httpWebRequest)
        {
            try
            {
                WebResponse webResponse = httpWebRequest.GetResponse();

                // Deserialize the json response
                using (var stream = webResponse.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();
                    reader.Close();

                    return result;
                }
            }
            catch (ProtocolViolationException protocolException)
            {
                throw new RpcException("Unable to connect to the daemon.", protocolException);
            }
            catch (WebException webException)
            {
                var response = webException.Response as HttpWebResponse;

                if(response == null)
                    throw new RpcException(string.Format("Error while reading the json response: {0}.", webException.Message), webException);

                using (var stream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        string error = reader.ReadToEnd();

                        var errorResponse = JsonConvert.DeserializeObject<DaemonErrorResponse>(error);
                        throw new RpcException(errorResponse);
                    }
                }
            }
            catch (Exception exception)
            {
                throw new RpcException("An unknown exception occured while trying to read the JSON response.", exception);
            }
        }
    }
}
