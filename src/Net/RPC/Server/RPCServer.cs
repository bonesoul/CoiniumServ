using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jayrock.JsonRpc;
using Newtonsoft.Json;
using coinium.Net.RPC.Responses;

namespace coinium.Net.RPC.Server
{
    //public class RPCServer
    //{
    //    public class ExampleCalculatorService : JsonRpcService
    //    {
    //        [JsonRpcMethod]
    //        private string getinfo()
    //        {
    //            var info = new Info();
    //            info.Version = "abc";
    //            string json = JsonConvert.SerializeObject(info);

    //            return json;
    //        }

    //        [JsonRpcMethod("mining.subscribe")]
    //        private string miningsubscribe()
    //        {
    //            var info = new Info();
    //            info.Version = "abc";
    //            string json = JsonConvert.SerializeObject(info);

    //            return json;
    //        }
    //    }
    //}

    class Service : JsonRpcService
    {
        [JsonRpcMethod("add")]
        public int Add(int a, int b) { return a + b; }

        [JsonRpcMethod("env")]
        public IDictionary GetEnvironment() { return Environment.GetEnvironmentVariables(); }

        [JsonRpcMethod("mining.subscribe")]
        private string miningsubscribe()
        {
            var info = new Info();
            info.Version = "abc";
            string json = JsonConvert.SerializeObject(info);

            return json;
        }
    }
}
