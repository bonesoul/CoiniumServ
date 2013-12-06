using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AustinHarris.JsonRpc.Client
{
    public class InProcessClient
    {
        private static Dictionary<long, AustinHarris.JsonRpc.JsonResponse> results = new Dictionary<long, AustinHarris.JsonRpc.JsonResponse>();
        private static Dictionary<long, ManualResetEvent> resultSignals = new Dictionary<long, ManualResetEvent>();
        /// <summary>
        /// This is not very performant with all the conversions going on.
        /// </summary>
        /// <param name="jsonrpc"></param>
        /// <returns></returns>
        public static Task<string> Invoke(string jsonrpc)
        {
            var request = Newtonsoft.Json.JsonConvert.DeserializeObject<AustinHarris.JsonRpc.JsonRequest>(jsonrpc);

            // we need to remember this ID so we can return a task when we get a response with this ID.
            var resultTask = Task<string>.Factory.StartNew(_id =>
            {
                var myid = (long)_id;

                ManualResetEvent mr = new ManualResetEvent(false);
                resultSignals[myid] = mr;
                mr.WaitOne();
                var result = results[myid];
                // clean up dictionary
                results.Remove(myid);
                resultSignals.Remove(myid);
                return Newtonsoft.Json.JsonConvert.SerializeObject(result);
            }, request.Id);// Probly need to add a continuation to check for errors and 
            // clean up our dictionarys there also..

            var rpcResultHandler = new AsyncCallback(_ => 
                {
                    var response = ((JsonRpcStateAsync)_).Result;
                    DispatchResults(Newtonsoft.Json.JsonConvert.DeserializeObject<AustinHarris.JsonRpc.JsonResponse>(response));
                });
            
            var async = new JsonRpcStateAsync(rpcResultHandler, null);
            async.JsonRpc = jsonrpc;
            JsonRpcProcessor.Process(async);

            return resultTask;
        }

        private static void DispatchResults(JsonResponse response)
        {
            // find our signal and result dictionary.
            var signal = resultSignals[(long)response.Id];// must be long
            // store our result
            results[(long)response.Id] = response;
            signal.Set();
        }
    }
}
