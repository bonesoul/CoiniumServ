using System;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace AustinHarris.JsonRpc
{
    public static class JsonRpcProcessor
    {
        public static void Process(JsonRpcStateAsync async, object context = null)
        {
            var t = Task.Factory.StartNew((_async) =>
            {
                var i = (Tuple<JsonRpcStateAsync, object>)_async;
                ProcessJsonRpcState(i.Item1,i.Item2);
            }, new Tuple<JsonRpcStateAsync,object>(async,context));
            
        }

        public static void Process(string sessionId, JsonRpcStateAsync async, object context = null)
        {
            var t = Task.Factory.StartNew((_async) =>
            {
                var i = (Tuple<string, JsonRpcStateAsync, object>)_async;
                ProcessJsonRpcState(i.Item1, i.Item2, i.Item3);
            }, new Tuple<string, JsonRpcStateAsync, object>(sessionId, async, context));

        }
        internal static void ProcessJsonRpcState(JsonRpcStateAsync async, object jsonRpcContext = null)
        {
            ProcessJsonRpcState(Handler.DefaultSessionId(), async, jsonRpcContext);
        }
        internal static void ProcessJsonRpcState(string sessionId, JsonRpcStateAsync async, object jsonRpcContext = null)
        {
            var context = async.AsyncState;

            JsonRequest[] rpcBatch = null;
            JsonResponse[] responseBatch = null;

            JsonRequest rpc = null;

            var callback = string.Empty;

            var response = new JsonResponse();

            response.Result = null;
            response.Error = null;

                string json = async.JsonRpc; 
                                
                if (isSingleRpc(json))
                {
                    try
                    {
                        if (json.Length > 0)
                        {
                            rpc = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonRequest>(json);
                            if (rpc == null)
                            {
                                response.Result = null;
                                response.Id = null;
                                response.Error = new JsonRpcException(-32700, "Parse error", "Invalid JSON was received by the server. An error occurred on the server while parsing the JSON text.");
                            }
                            else
                            {
                                response.Id = rpc.Id;
                                if (rpc.Method == null)
                                {
                                    response.Result = null;
                                    response.Id = rpc.Id;
                                    response.Error = new JsonRpcException(-32600, "Invalid Request", "Missing property 'method'");
                                }
                            }
                        }
                        else
                        {
                            response.Result = null;
                            response.Id = null;
                            response.Error = new JsonRpcException(-32600, "Invalid Request", "The JSON sent is not a valid Request object.");
                        }
                    }
                    catch (Exception ex)
                    {
                        response.Result = null;
                        if (rpc != null) response.Id = rpc.Id;
                        response.Error = new JsonRpcException(-32700, "Parse error", ex);
                        var result = Newtonsoft.Json.JsonConvert.SerializeObject(response);
                        async.Result = result;                        
                        async.SetCompleted();
                        return;
                    }
                    
                    if (response.Error == null
                        && rpc != null
                        && rpc.Method != null)
                    {
                        var data = Handler.GetSessionHandler(sessionId).Handle(rpc, jsonRpcContext);
                        if (data != null)
                        {
                            response.Error = data.Error;
                            response.Result = data.Result;
                            var result = "";
                            if (response.Id != null)// dont return a result for notifications
                            {
                                result=Newtonsoft.Json.JsonConvert.SerializeObject(response);
                            }
                            async.Result = result;
                            async.SetCompleted();
                            return;
                        }
                    }

                    var err = Newtonsoft.Json.JsonConvert.SerializeObject(response);

                    async.Result = err;
                    async.SetCompleted();
                }
                else // this is a batch of requests
                {
                    try
                    {
                        rpcBatch = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonRequest[]>(json);
                        responseBatch = new JsonResponse[rpcBatch.Length];
                        
                        for (int i = 0; i < rpcBatch.Length; i++)
                        {
                            responseBatch[i] = new JsonResponse();
                            if (rpcBatch[i] == null)
                            {
                                responseBatch[i].Result = null;
                                responseBatch[i].Id = null;
                                responseBatch[i].Error = new JsonRpcException(-32700, "Parse error", "Invalid JSON was received by the server. An error occurred on the server while parsing the JSON text.");
                            }
                            else
                            {
                                responseBatch[i].Id = rpcBatch[i].Id;
                                if (rpcBatch[i].Method == null)
                                {
                                    responseBatch[i].Result = null;
                                    responseBatch[i].Error = new JsonRpcException(-32600, "Invalid Request", "Missing property 'method'");
                                }
                            }
                        }                        
                    }
                    catch (Exception ex)
                    {
                        response.Result = null;
                        if (rpc != null) response.Id = rpc.Id;
                        response.Error = new JsonRpcException(-32700, "Parse error", ex);
                        var result = Newtonsoft.Json.JsonConvert.SerializeObject(response);
                        async.Result = result;
                        async.SetCompleted();
                        return;
                    }

                    // we should have a batch of RPC at this point
                    var respBuilder = new StringBuilder();
                    for (int i = 0; i < rpcBatch.Length; i++)
                    {
                        if (i == 0)
                        {
                            respBuilder.Append("[");
                        }

                        if (rpcBatch[i] == null || rpcBatch[i].Method == null)
                        {
                            responseBatch[i].Error = new JsonRpcException(-32600, "Invalid Request", "Missing property 'method'");
                        }
                        else if (responseBatch[i].Error == null)
                        {
                            var data = Handler.GetSessionHandler(sessionId).Handle(rpcBatch[i], jsonRpcContext);
                            if (data != null)
                            {
                                responseBatch[i].Error = data.Error;
                                responseBatch[i].Result = data.Result;
                                                            
                            }
                        }
                        // dont return a response for notifications.
                        if (responseBatch[i].Id != null || responseBatch[i].Error != null)
                        {
                            var result = Newtonsoft.Json.JsonConvert.SerializeObject(responseBatch[i]);
                            respBuilder.Append(result);
                            if (i != rpcBatch.Length - 1)
                            {
                                respBuilder.Append(',');
                            }
                        }

                        if (i == rpcBatch.Length - 1)
                        {
                            respBuilder.Append("]");
                            var str = respBuilder.ToString();
                            async.Result = str;
                            async.SetCompleted(); // let IIS think we are completed now.
                            return;
                        }
                    }

                    // if we made it this far, then there were no items in the array
                    response.Id = null;
                    response.Result = null;
                    response.Error = new JsonRpcException(3200, "Invalid Request", "Batch of calls was empty.");

                    var err = Newtonsoft.Json.JsonConvert.SerializeObject(response);

                    async.Result = err;
                    async.SetCompleted();
                }     
        }

        private static bool isSingleRpc(string json)
        {
            for (int i = 0; i < json.Length; i++)
            {
                if (json[i] == '{') return true;
                else if (json[i] == '[') return false;
            }
            return true;
        }
    }
}
