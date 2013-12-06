using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AustinHarris.JsonRpc;
using Newtonsoft.Json;
using coinium.Net.RPC.Responses;
using Newtonsoft.Json.Linq;

public class RPCServer
{
    public class ExampleCalculatorService : JsonRpcService
    {
        [JsonRpcMethod("mining.subscribe")]
        private string MiningSubscribe(string miner)
        {
            //var info = new Info();
            //info.Version = "abc";
            //string json = JsonConvert.SerializeObject(info);

            return @"[[[""mining.set_difficulty"",""b4b6693b72a50c7116db18d6497cac52""],[""mining.notify"",""ae6812eb4cd7735a302a8a9dd95cf71f""]],""08000002"",4]\n";
        }

        //[JsonRpcMethod("mining.subscribe")]
        //private string MiningSubscribe()
        //{
        //    return @"[[[""mining.set_difficulty"",""b4b6693b72a50c7116db18d6497cac52""],[""mining.notify"",""ae6812eb4cd7735a302a8a9dd95cf71f""]],""08000002"",4]";
        //}
    }
}