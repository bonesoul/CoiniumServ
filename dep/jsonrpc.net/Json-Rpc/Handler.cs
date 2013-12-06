namespace AustinHarris.JsonRpc
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Newtonsoft.Json;
    using System.Threading.Tasks;
    using System.Collections.Concurrent;
using Newtonsoft.Json.Linq;

    public class Handler
    {
        #region Members

        //private static Handler current;
        private static Dictionary<string, Handler> _sessionHandlers;
        private static string _defaultSessionId;
        #endregion

        #region Constructors

        static Handler()
        {
            //current = new Handler(Guid.NewGuid().ToString());
            _defaultSessionId = Guid.NewGuid().ToString();
            _sessionHandlers = new Dictionary<string, Handler>();
            _sessionHandlers.Add(_defaultSessionId, new Handler(_defaultSessionId));
        }

        private Handler(string sessionId)
        {
            SessionId = sessionId;
            this.MetaData = new SMD();
            this.Handlers = new Dictionary<string, Delegate>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns the SessionID of the default session
        /// </summary>
        /// <returns></returns>
        public static string DefaultSessionId() { return _defaultSessionId; }

        /// <summary>
        /// Gets a specific session
        /// </summary>
        /// <param name="sessionId">The sessionId of the handler you want to retrieve.</param>
        /// <returns></returns>
        public static Handler GetSessionHandler(string sessionId)
        {
            // double check lock. We only lock if we think we are going to add one.
            if (_sessionHandlers.ContainsKey(sessionId) == false)
            {
                lock (_sessionHandlers)
                {
                    if (_sessionHandlers.ContainsKey(sessionId) == false)
                    {
                        _sessionHandlers.Add(sessionId, new Handler(sessionId));
                    }
                }
            }

            return _sessionHandlers[sessionId];
        }
        
        /// <summary>
        /// gets the default session
        /// </summary>
        /// <returns>The default Session Handler</returns>
        public static Handler GetSessionHandler()
        {
            return _sessionHandlers[_defaultSessionId];
        }

        /// <summary>
        /// Gets the default session handler
        /// </summary>
        public static Handler DefaultHandler { get { return _sessionHandlers[_defaultSessionId]; } }

        /// <summary>
        /// The sessionID of this Handler
        /// </summary>
        public string SessionId { get; private set; } 

        private static ConcurrentDictionary<int, object> RpcContexts = new ConcurrentDictionary<int, object>();
        private static ConcurrentDictionary<int, JsonRpcException> RpcExceptions = new ConcurrentDictionary<int, JsonRpcException>();

        /// <summary>
        /// Provides access to a context specific to each JsonRpc method invocation.
        /// Warning: Must be called from within the execution context of the jsonRpc Method to return the context
        /// </summary>
        /// <returns></returns>
        public static object RpcContext()
        {
            if (Task.CurrentId == null) 
                return null;

            if (RpcContexts.ContainsKey(Task.CurrentId.Value) == false)
                return null;

            return RpcContexts[Task.CurrentId.Value];
        }

        /// <summary>
        /// Allows you to set the exception used in in the JsonRpc response.
        /// Warning: Must be called from within the execution context of the jsonRpc method.
        /// </summary>
        /// <param name="exception"></param>
        public static void RpcSetException(JsonRpcException exception)
        {
            if (Task.CurrentId != null)
                RpcExceptions[Task.CurrentId.Value] = exception;
            else
                throw new InvalidOperationException("This method is only valid when used within the context of a method marked as a JsonRpcMethod, and that method must of been invoked by the JsonRpc Handler.");
        }

        private void RemoveRpcException()
        {
            if (Task.CurrentId != null)
            {
                var id = Task.CurrentId.Value;
                RpcExceptions[id] = null;
                JsonRpcException va;
                RpcExceptions.TryRemove(id, out va);
            }
        }

        private AustinHarris.JsonRpc.PreProcessHandler externalPreProcessingHandler;
        private Func<JsonRequest, JsonRpcException, JsonRpcException> externalErrorHandler;
        private Dictionary<string,Delegate> Handlers { get; set; }
        #endregion

        /// <summary>
        /// This metadata contains all the types and mappings of all the methods in this handler. Warning: Modifying this directly could cause your handler to no longer function. 
        /// </summary>
        public SMD MetaData { get; set; }

        #region Public Methods

        /// <summary>
        /// Registers a jsonRpc method name (key) to be mapped to a specific function
        /// </summary>
        /// <param name="key">The Method as it will be called from JsonRpc</param>
        /// <param name="handle">The method that will be invoked</param>
        /// <returns></returns>
        public bool Register(string key, Delegate handle)
        {
            var result = false;

            if (!this.Handlers.ContainsKey(key))
            {
                this.Handlers.Add(key, handle);
            }

            return result;
        }

        /// <summary>
        /// Invokes a method to handle a JsonRpc request.
        /// </summary>
        /// <param name="Rpc">JsonRpc Request to be processed</param>
        /// <param name="RpcContext">Optional context that will be available from within the jsonRpcMethod.</param>
        /// <returns></returns>
        public JsonResponse Handle(JsonRequest Rpc, Object RpcContext = null)
        {
            AddRpcContext(RpcContext);

            var preProcessingException = PreProcess(Rpc, RpcContext);
            if (preProcessingException != null)
            {
                return new JsonResponse() { Error = preProcessingException,
                    Id = Rpc.Id };
            }

            SMDService metadata = null;
            Delegate handle = null;
            var haveDelegate = this.Handlers.TryGetValue(Rpc.Method, out handle);
            var haveMetadata = this.MetaData.Services.TryGetValue(Rpc.Method, out metadata);

            if (haveDelegate == false || haveMetadata == false || metadata == null || handle == null)
            {
                return new JsonResponse() { Result = null, Error = new JsonRpcException(-32601, "Method not found", "The method does not exist / is not available."), Id = Rpc.Id };
            }

            if (Rpc.Params == null) // allow params element to be missing without rewriting the params counting code below.
            {
                Rpc.Params = new Newtonsoft.Json.Linq.JArray();
            }

            if (Rpc.Params is ICollection == false)
            {
                return new JsonResponse()
                {
                    Result = null,
                    Error = new JsonRpcException(-32602,
                        "Invalid params", "The number of parameters could not be counted"),
                    Id = Rpc.Id
                };
            }

            bool isJObject = Rpc.Params is Newtonsoft.Json.Linq.JObject;
            bool isJArray = Rpc.Params is Newtonsoft.Json.Linq.JArray;
            object[] parameters = null;
            bool expectsRefException = false;
            var metaDataParamCount = metadata.parameters.Count(x => x != null);
            
            var getCount = Rpc.Params as ICollection;
            var loopCt = getCount.Count;
            var paramCount = loopCt;
            if (paramCount == metaDataParamCount - 1 && metadata.parameters[metaDataParamCount-1].ObjectType.Name.Contains(typeof(JsonRpcException).Name))
            {
                paramCount++;
                expectsRefException = true;
            }
            parameters = new object[paramCount];

            if (isJArray)
            {
                var jarr = ((Newtonsoft.Json.Linq.JArray)Rpc.Params);
                //var loopCt = jarr.Count;
                //var pCount = loopCt;
                //if (pCount == metaDataParamCount - 1 && metadata.parameters[metaDataParamCount].GetType() == typeof(JsonRpcException))
                //    pCount++;
                //parameters = new object[pCount];
                for (int i = 0; i < loopCt; i++)
                {
                     parameters[i] = CleanUpParameter(jarr[i], metadata.parameters[i]);                    
                }
            }
            else if (isJObject)
            {
                var jo = Rpc.Params as Newtonsoft.Json.Linq.JObject;
                //var loopCt = jo.Count;
                //var pCount = loopCt;
                //if (pCount == metaDataParamCount - 1 && metadata.parameters[metaDataParamCount].GetType() == typeof(JsonRpcException))
                //    pCount++;
                //parameters = new object[pCount];
                var asDict = jo as IDictionary<string, Newtonsoft.Json.Linq.JToken>;
                for (int i = 0; i < loopCt; i++)
                {
                    if (asDict.ContainsKey(metadata.parameters[i].Name) == false)
                    {
                        return new JsonResponse()
                        {
                            Error = ProcessException(Rpc,
                            new JsonRpcException(-32602,
                                "Invalid params",
                                string.Format("Named parameter '{0}' was not present.",
                                                metadata.parameters[i].Name)
                                ))
                                ,Id = Rpc.Id
                        };
                    }
                    parameters[i] = CleanUpParameter(jo[metadata.parameters[i].Name], metadata.parameters[i]);
                }
            }
            
            if (parameters.Length != metaDataParamCount)
            {
                return new JsonResponse()
                {
                    Error = ProcessException(Rpc,
                    new JsonRpcException(-32602,
                        "Invalid params",
                        string.Format("Expecting {0} parameters, and received {1}",
                                        metadata.parameters.Length,
                                        parameters.Length)
                        )),
                    Id = Rpc.Id
                };
            }

            try
            {
                var results = handle.DynamicInvoke(parameters);
                var last = parameters.Length>0 ? parameters[paramCount - 1]:null;
                JsonRpcException contextException;
                if (Task.CurrentId.HasValue && RpcExceptions.TryRemove(Task.CurrentId.Value, out contextException))
                {
                    return new JsonResponse() { Error = ProcessException(Rpc, contextException), Id = Rpc.Id };
                }
                if (expectsRefException && last != null && last is JsonRpcException)
                {
                    return new JsonResponse() { Error = ProcessException(Rpc, last as JsonRpcException), Id = Rpc.Id };
                }

                return new JsonResponse() { Result = results };
            }
            catch (Exception ex)
            {
                if (ex is TargetParameterCountException)
                {
                    return new JsonResponse() { Error = ProcessException(Rpc, new JsonRpcException(-32602, "Invalid params", ex)) };
                }

                // We really dont care about the TargetInvocationException, just pass on the inner exception
                if (ex is JsonRpcException)
                {
                    return new JsonResponse() { Error = ProcessException(Rpc, ex as JsonRpcException) };
                }
                if (ex.InnerException != null && ex.InnerException is JsonRpcException)
                {
                    return new JsonResponse() { Error = ProcessException(Rpc, ex.InnerException as JsonRpcException) };
                }
                else if (ex.InnerException != null)
                {
                    return new JsonResponse() { Error = ProcessException(Rpc, new JsonRpcException(-32603, "Internal Error", ex.InnerException)) };
                }

                return new JsonResponse() { Error = ProcessException(Rpc, new JsonRpcException(-32603, "Internal Error", ex)) };
            }
            finally
            {
                RemoveRpcContext();
            }
        }

        private void AddRpcContext(object RpcContext)
        {
            if (Task.CurrentId != null)
                RpcContexts[Task.CurrentId.Value] = RpcContext;
        }
        private void RemoveRpcContext()
        {
            if (Task.CurrentId != null)
            {
                var id = Task.CurrentId.Value;
                RpcContexts[id] = null;
                object va;
                RpcContexts.TryRemove(id, out va);
            }
        }
        
        private JsonRpcException ProcessException(JsonRequest req,JsonRpcException ex)
        {
            if(externalErrorHandler!=null)
                return externalErrorHandler(req,ex);
            return ex;
        }

        internal void SetErrorHandler(Func<JsonRequest, JsonRpcException, JsonRpcException> handler)
        {
            externalErrorHandler = handler;
        }
        
        #endregion
        private object CleanUpParameter(object p, SMDAdditionalParameters metaData)
        {
            var bob = p as JValue;
            if (bob != null && (bob.Value == null || bob.Value.GetType() == metaData.ObjectType))
            {
                return bob.Value;
            }

            var paramI = p;
            try
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject(paramI.ToString(), metaData.ObjectType);
            }
            catch (Exception ex)
            {
                // no need to throw here, they will
                // get an invalid cast exception right after this.
            }

            return paramI;
        }

        private JsonRpcException PreProcess(JsonRequest request, object context)
        {
            if (externalPreProcessingHandler == null)
                return null;
            return externalPreProcessingHandler(request, context);
        }

        internal void SetPreProcessHandler(AustinHarris.JsonRpc.PreProcessHandler handler)
        {
            externalPreProcessingHandler = handler;
        }
    }

}
