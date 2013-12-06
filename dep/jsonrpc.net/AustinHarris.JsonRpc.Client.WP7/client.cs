using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;
using AustinHarris.JsonRpc;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Concurrency;

namespace AustinHarris.JsonRpc
{
    public class JsonRpcClient
    {
        private static object idLock = new object();
        private static int id = 0;
        public Uri ServiceEndpoint = null;

        public JsonRpcClient(Uri serviceEndpoint)
        {
            ServiceEndpoint = serviceEndpoint;
        }
        
        private static Stream CopyAndClose(Stream inputStream)
        {
            const int readSize = 256;
            byte[] buffer = new byte[readSize];
            MemoryStream ms = new MemoryStream();

            int count = inputStream.Read(buffer, 0, readSize);
            while (count > 0)
            {
                ms.Write(buffer, 0, count);
                count = inputStream.Read(buffer, 0, readSize);
            }
            ms.Position = 0;
            inputStream.Close();
            return ms;
        }

        public IObservable<JsonResponse<T>> Invoke<T>(string method, object arg, IScheduler scheduler)
        {
            var req = new AustinHarris.JsonRpc.JsonRequest()
            {
                Method = method,
                Params = new object[] { arg }
            };
            return Invoke<T>(req, scheduler);
        }

        public IObservable<JsonResponse<T>> Invoke<T>(string method, object[] args, IScheduler scheduler)
        {
            var req = new AustinHarris.JsonRpc.JsonRequest()
            {
                Method = method,
                Params = args
            };
            return Invoke<T>(req,scheduler);
        }

        public IObservable<JsonResponse<T>> Invoke<T>(JsonRequest jsonRpc, IScheduler scheduler)
        {
            var subj = new Subject<JsonResponse<T>>();

            int myId;
            lock (idLock)
            {
                myId = ++id;
            }
            jsonRpc.Id = myId.ToString();

            WebRequest req = null;
            try
            {
                req = HttpWebRequest.CreateHttp(new Uri(ServiceEndpoint +  "?callid=" + myId.ToString()));
                req.Method = "Post";
                req.ContentType = "application/json-rpc";
            }
            catch (Exception ex)
            {
                return Observable.Throw<JsonResponse<T>>(ex);
            }

            var ar = req.BeginGetRequestStream(new AsyncCallback((iar) =>
            {
                HttpWebRequest request = null;

                try
                {
                    request = (HttpWebRequest)iar.AsyncState;
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(jsonRpc);
                    var reqStream = req.EndGetRequestStream(iar);
                    using (var stream = new StreamWriter(reqStream))
                    {
                        stream.Write(json);
                        stream.Flush();
                        stream.Close();
                    }
                }
                catch (Exception ex)
                {
                    subj.OnError(ex);
                }

                var rar = req.BeginGetResponse(new AsyncCallback((riar) =>
                {
                    JsonResponse<T> rjson = null;
                    string sstream = "";
                    try
                    {
                        var request1 = (HttpWebRequest)riar.AsyncState;
                        var resp = (HttpWebResponse)request1.EndGetResponse(riar);
                        var respStream = resp.GetResponseStream();
                        using (var rstream = new StreamReader(CopyAndClose(respStream)))
                        {
                            sstream = rstream.ReadToEnd();
                        }

                        rjson = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonResponse<T>>(sstream);
                    }
                    catch (Exception ex)
                    {
                        subj.OnError(ex);
                        return;
                    }

                    try
                    {
                        if (rjson == null)
                        {
                            JObject jo = Newtonsoft.Json.JsonConvert.DeserializeObject(sstream) as JObject;
                            subj.OnError(new Exception(jo["Error"].ToString()));
                            return;
                        }
                    }
                    catch (Exception)
                    { }

                    subj.OnNext(rjson);
                    subj.OnCompleted();
                }), request);
            }), req);

            return subj;
        }
    }
}
