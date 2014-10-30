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
using System.Reflection;
using System.Text;
using CoiniumServ.Coin.Config;
using CoiniumServ.Daemon.Config;
using CoiniumServ.Daemon.Errors;
using CoiniumServ.Daemon.Exceptions;
using CoiniumServ.Logging;
using CoiniumServ.Utils.Extensions;
using libCoiniumServ.Versions;
using Metrics;
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

        private readonly Int32 _timeout;

        private readonly IRpcExceptionFactory _rpcExceptionFactory;

        private readonly ILogger _logger;

        private static readonly Meter RequestsMeter = Metric.Meter("[Daemon] Requests", Unit.Requests, TimeUnit.Seconds);

        public DaemonBase(IDaemonConfig daemonConfig, ICoinConfig coinConfig, IRpcExceptionFactory rpcExceptionFactory)
        {
            _rpcExceptionFactory = rpcExceptionFactory;
            _logger = LogManager.PacketLogger.ForContext<DaemonClient>().ForContext("Component", coinConfig.Name);

            _timeout = daemonConfig.Timeout * 1000; // set the daemon timeout.
        	
            RpcUrl = string.Format("http://{0}:{1}", daemonConfig.Host, daemonConfig.Port);
            RpcUser = daemonConfig.Username;
            RpcPassword = daemonConfig.Password;
            
            RequestCounter = 0;
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
            RequestsMeter.Mark();

            var webRequest = (HttpWebRequest)WebRequest.Create(RpcUrl);
            webRequest.Credentials = new NetworkCredential(RpcUser, RpcPassword);

            // Important, otherwise the service can't deserialse your request properly
            webRequest.UserAgent = string.Format("CoiniumServ {0:} {1:}", VersionInfo.CodeName, Assembly.GetAssembly(typeof (Program)).GetName().Version);
            webRequest.ContentType = "application/json-rpc";
            webRequest.Method = "POST";
            webRequest.Timeout = _timeout;

            _logger.Verbose("tx: {0}", Encoding.UTF8.GetString(walletRequest.GetBytes()).PrettifyJson());

            byte[] byteArray = walletRequest.GetBytes();
            webRequest.ContentLength = byteArray.Length;

            try
            {
                using (Stream dataStream = webRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }

                return webRequest;
            }
            catch (WebException webException)
            {
                webRequest = null;
                throw _rpcExceptionFactory.GetRpcException(webException);
            }
            catch (Exception exception)
            {
                webRequest = null;
                throw _rpcExceptionFactory.GetRpcException("An unknown exception occured while making json request.", exception);                
            }
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
                httpWebRequest = null;
                throw new Exception("There was a problem deserializing the response from the coin wallet.", jsonEx);
            }
            catch (Exception exception)
            {
                httpWebRequest = null;
                throw _rpcExceptionFactory.GetRpcException("An unknown exception occured while reading json response.", exception);
            }
        }

        /// <summary>
        /// Gets the JSON response for the given HTTP request.
        /// </summary>
        /// <param name="httpWebRequest">The HTTP request send to the Bitcoin RPC interface.</param>
        /// <returns>The raw JSON string.</returns>
        private string GetJsonResponse(HttpWebRequest httpWebRequest)
        {
            WebResponse webResponse = null;

            try
            {
                using (webResponse = httpWebRequest.GetResponse())
                {
                    // Deserialize the json response
                    using (var stream = webResponse.GetResponseStream())
                    {
                        if (stream == null)
                            return string.Empty;

                        using (var reader = new StreamReader(stream))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (ProtocolViolationException protocolException)
            {
                throw _rpcExceptionFactory.GetRpcException(protocolException);
            }
            catch (WebException webException)
            {
                var response = webException.Response as HttpWebResponse;

                if (response == null)
                    throw _rpcExceptionFactory.GetRpcException(webException);

                var error = ReadJsonError(response); // try to read the error response.

                if (error != null)
                    throw _rpcExceptionFactory.GetRpcErrorException(error); //throw the error.
                else
                    throw _rpcExceptionFactory.GetRpcException(
                        "An unknown exception occured while reading json response.", webException);
            }
            catch (Exception exception)
            {
                throw _rpcExceptionFactory.GetRpcException("An unknown exception occured while reading json response.",
                    exception);
            }
            finally
            {
                if (webResponse != null)
                    webResponse.Close(); //Close webresponse connection

                webResponse = null; //To clear up the webresponse
            }
        }

        private RpcErrorResponse ReadJsonError(HttpWebResponse response)
        {
            using (var stream = response.GetResponseStream())
            {
                if (stream == null)
                    return null;

                using (var reader = new StreamReader(stream))
                {
                    string data = reader.ReadToEnd(); // read the error response.

                    // we actually expect a json error response here, but it seems some coins may return non-json responses.
                    try
                    {
                        var error = JsonConvert.DeserializeObject<RpcErrorResponse>(data); // so let's try parsing the error response as json.    
                        return error;
                    }
                    catch (JsonException e) // if we can't parse the error response as json
                    {
                        throw _rpcExceptionFactory.GetRpcException(data, e); // then just use the error text.
                    }
                    catch (Exception exception)
                    {
                        throw _rpcExceptionFactory.GetRpcException("An unknown exception occured while reading json response.", exception);
                    }
                }
            }
        }
    }
}
