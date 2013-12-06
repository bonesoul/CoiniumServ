using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AustinHarris.JsonRpc;
using Newtonsoft.Json;
using coinium.Net.RPC.Responses;

namespace coinium.Net.RPC.Server
{
    public class RPCServer
    {
        public class ExampleCalculatorService : JsonRpcService
        {
            [JsonRpcMethod]
            private string getinfo()
            {
                var info = new Info();
                info.Version = "abc";
                string json = JsonConvert.SerializeObject(info);

                return json;
            }

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
}
