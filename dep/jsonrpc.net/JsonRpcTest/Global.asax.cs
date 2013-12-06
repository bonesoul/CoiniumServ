using AustinHarris.JsonRpc;
using System;

namespace JsonRpcTest{
    public class Global : System.Web.HttpApplication {
        static object[] services = new object[] {
           new TestServer.HelloWorldService(),
           new TestServer.TestService(),
          
        };
        protected void Application_Start(object sender, EventArgs e) {
            AustinHarris.JsonRpc.Config.SetErrorHandler(OnJsonRpcException);
            Config.SetPreProcessHandler(new PreProcessHandler(PreProcess));
        }

        private AustinHarris.JsonRpc.JsonRpcException OnJsonRpcException(AustinHarris.JsonRpc.JsonRequest rpc, AustinHarris.JsonRpc.JsonRpcException ex)
        {
            return ex;
        }

        private JsonRpcException PreProcess(JsonRequest rpc, object context)
        {
            // Useful for logging or authentication using the context.

            if(!string.Equals(rpc.Method, "RequiresCredentials",StringComparison.CurrentCultureIgnoreCase))
                return null;
                        
            // If this is using the ASP.Net handler then the context will contain the httpRequest
            // you could use that for cookies or IP authentication.

            // Here we will just check that the first parameter is a magic Key
            // DO NOT do this type of thing in production code. You would be better just checking the parameter inside the JsonRpcMethod.
            var j = rpc.Params as Newtonsoft.Json.Linq.JArray;
            if (j == null 
                || j[0] == null
                || j[0].Type != Newtonsoft.Json.Linq.JTokenType.String
                || !string.Equals(j[0].ToString(), "GoodPassword", StringComparison.CurrentCultureIgnoreCase)
                ) return new JsonRpcException(-2, "This exception was thrown using: JsonRpcTest.Global.PreProcess, Not Authenticated", null);

            return null;
        }
    }
}