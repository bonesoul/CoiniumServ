using System;

namespace AustinHarris.JsonRpc
{
    /// <summary>
    /// Required to expose a method to the JsonRpc service.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class JsonRpcMethodAttribute : Attribute
    {
        readonly string jsonMethodName;

        /// <summary>
        /// Required to expose a method to the JsonRpc service.
        /// </summary>
        /// <param name="jsonMethodName">Lets you specify the method name as it will be referred to by JsonRpc.</param>
        public JsonRpcMethodAttribute(string jsonMethodName = "")
        {
            this.jsonMethodName = jsonMethodName;
        }

        public string JsonMethodName
        {
            get { return jsonMethodName; }
        }
    }
}
