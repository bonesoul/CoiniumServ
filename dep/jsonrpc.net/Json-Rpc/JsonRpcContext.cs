using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AustinHarris.JsonRpc
{
    /// <summary>
    /// Provides access to a context specific to each JsonRpc method invocation.
    /// This is a convienence class that wraps calls to Context specific methods on AustinHarris.JsonRpc.Handler
    /// </summary>
    public class JsonRpcContext
    {
        private JsonRpcContext(object value)
        {
            Value = value;
        }

        /// <summary>
        /// The data associated with this context.
        /// </summary>
        public object Value { get; private set; }

        /// <summary>
        /// Allows you to set the exception used in in the JsonRpc response.
        /// Warning: Must be called from within the execution context of the jsonRpc method to function.
        /// </summary>
        /// <param name="exception"></param>
        public static void SetException(JsonRpcException exception)
        {
            Handler.RpcSetException(exception);
        }


        /// <summary>
        /// Must be called from within the execution context of the jsonRpc Method to return the context
        /// </summary>
        /// <returns></returns>
        public static JsonRpcContext Current()
        {
            return new JsonRpcContext(Handler.RpcContext());
        }
    }
}
