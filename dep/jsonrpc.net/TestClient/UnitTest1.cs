using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using AustinHarris.JsonRpc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
 
namespace TestClient
{
    [TestClass]
    public class UnitTest1
    {
        Random r = null;
        Uri remoteUri = new Uri("http://localhost.:49718/json.rpc");
        public UnitTest1()
        {
            r = new Random(Environment.TickCount);
        }

        [TestMethod]
        public void TestHelloWorld()
        {
            AutoResetEvent are = new AutoResetEvent(false);
            var client = new AustinHarris.JsonRpc.JsonRpcClient(remoteUri);
            var myObs = client.Invoke<string>("helloWorld", "My Message", Scheduler.TaskPool);

            using (myObs.Subscribe(
                onNext: _ =>
                {
                    Console.WriteLine(_.Result);
                    Assert.IsTrue(_.Result == "Hello World My Message");
                },
                onError: _ =>
                {
                    Assert.Fail();
                    are.Set();
                },
                onCompleted: () => are.Set()
                ))
            {
                are.WaitOne();
            }
        }

        private string getPrintableString(int len)
        {
            return new string(Enumerable.Range(0, r.Next(len)).Select(_ => (char)r.Next(32, 126)).ToArray());
        }

        private string getNonPrintableString(int len)
        {
            return new string(Enumerable.Range(0, r.Next(len)).Select(_ => (char)r.Next(0, 31)).ToArray());
        }

        private string getExtendedAsciiString(int len)
        {
            return new string(Enumerable.Range(0, r.Next(len)).Select(_ => (char)r.Next(0, 255)).ToArray());
        }

        [TestMethod]
        public void TestArbitrary()
        {
            AutoResetEvent are = new AutoResetEvent(false);
            var client = new AustinHarris.JsonRpc.JsonRpcClient(remoteUri);
            var arbitrary = new Newtonsoft.Json.Linq.JObject();
            JObject r = null;
            Exception e = null;
            for (int i = 0; i < 10; i++)
            {
                arbitrary[getPrintableString(10)] = getPrintableString(20);
                arbitrary[getNonPrintableString(10)] = getNonPrintableString(20);
                arbitrary[getExtendedAsciiString(10)] = getExtendedAsciiString(20);
            }

            var myObs = client.Invoke<Newtonsoft.Json.Linq.JObject>("testArbitraryJObject", arbitrary, Scheduler.TaskPool);

            using(myObs.Subscribe(
                onNext: (jo) =>
                {
                   r = jo.Result;
                },
                onError:   _ =>
                {
                    e = _;
                },
                onCompleted: () => are.Set()
                ))
            {
                are.WaitOne();
            };

            
            Assert.IsTrue(r.ToString() == arbitrary.ToString());
            Assert.IsTrue(e == null);
        }

        private JObject CreateArbitraryJObject()
        {
            var arbitrary = new Newtonsoft.Json.Linq.JObject();
            for (int i = 0; i < 1; i++)
            {
                arbitrary[getPrintableString(4)] = getPrintableString(4);
                arbitrary[getNonPrintableString(4)] = getNonPrintableString(4);
                arbitrary[getExtendedAsciiString(4)] = getExtendedAsciiString(4);
            }
            return arbitrary;
        }

        [TestMethod]
        public void TestRpcPerSecond()
        {
            // This test sometimes fails due to overloading
            // the network buffer
            var client = new AustinHarris.JsonRpc.JsonRpcClient(remoteUri);
            var abjo = CreateArbitraryJObject();
            var limit = 50;
            var passes = 5;
            var requestStream = Observable.Generate<int, JObject>(0,
                                            i => i < limit,
                                            i => i+1,
                                            i => abjo
                                            );
            for (int i = 0; i < passes; i++)
            {
                var tmr = Stopwatch.StartNew();
                SendRequestsAndWait(client, requestStream);
                tmr.Stop();
                var perSecond = (decimal)limit  * (1000 / (decimal)tmr.ElapsedMilliseconds);
                Console.WriteLine("Pass{0} - {1} requests in : {2}ms for {3} requests per second", i, limit, tmr.ElapsedMilliseconds, (int)perSecond);
                limit = limit * 2;
                Thread.Sleep(200);
            }            
        }

        [TestMethod]
        public void TestNetworkBuffer()
        {
            // This will overflow the network buffer.

            //var client = new AustinHarris.JsonRpc.JsonRpcClient(remoteUri);
            //var abjo = CreateArbitraryJObject();
            //var limit = 10000;
            //var passes = 2;
            //var requestStream = Observable.Generate<int, JObject>(0,
            //                                i => i < limit,
            //                                i => i + 1,
            //                                i => abjo
            //                                );
            //for (int i = 0; i < passes; i++)
            //{
            //    var tmr = Stopwatch.StartNew();
            //    SendRequestsAndWait(client, requestStream);
            //    tmr.Stop();
            //    var perSecond = (decimal)limit * (1000 / (decimal)tmr.ElapsedMilliseconds);
            //    Console.WriteLine("Pass{0} - {1} requests in : {2}ms for {3} requests per second", i, limit, tmr.ElapsedMilliseconds, (int)perSecond);
            //    limit = limit * 2;
            //}
        }

        private void SendRequestsAndWait(JsonRpcClient client, IObservable<JObject> requestStream)
        {
            // chaining is fun
            var mre = new ManualResetEventSlim(false);
            using ((from request in requestStream
                    select client.Invoke<Newtonsoft.Json.Linq.JObject>("testArbitraryJObject", request, Scheduler.TaskPool))
                    .Merge()
                    .Subscribe(
                    onNext: _ => { /* do nothing and like it */ },
                    onError: _ => {Debug.WriteLine(_.Message); mre.Set();},
                    onCompleted: mre.Set))
            {
                mre.Wait();
            } 
        }
        
        [TestMethod]
        public void TestMetaData()
        {
            AutoResetEvent are = new AutoResetEvent(false);
            var rpc = new JsonRpcClient(remoteUri);
            string method = "?";
            JsonResponse<Newtonsoft.Json.Linq.JObject> result = null;
            var myObs = rpc.Invoke<Newtonsoft.Json.Linq.JObject>(method, null, Scheduler.TaskPool);

            myObs.Subscribe(
                onNext: _ =>
                {
                    result = _;
                    are.Set();
                },
                onError: _ =>
                {
                    are.Set();
                },
                onCompleted: () => { are.Set(); }
                );

            are.WaitOne();

            Assert.IsTrue(result != null);
            var res = result.Result;

        }

        [TestMethod]
        public void TestEcho()
        {
            AutoResetEvent are = new AutoResetEvent(false);
            var rpc = new JsonRpcClient(remoteUri);
            string method = "internal.echo";
            string input = "Echo this sucka";
            JsonResponse<string> result = null;
            var myObs = rpc.Invoke<string>(method,  input , Scheduler.TaskPool);

            myObs.Subscribe(
                onNext: _ =>
                {
                    result = _;
                    are.Set();
                },
                onError: _ =>
                {
                    are.Set();
                },
                onCompleted: () => { are.Set(); }
                );

            are.WaitOne();

            Assert.IsTrue(result != null);
            var res = result.Result;      
            Assert.IsTrue(res == input.ToString());
        }

        [TestMethod]
        public void TestFloat()
        {
            AutoResetEvent are = new AutoResetEvent(false);
            var rpc = new JsonRpcClient(remoteUri);
            string method = "testFloat";
            float input = 7.1f;
            JsonResponse<string[]> result = null;
            var myObs = rpc.Invoke<string[]>(method,  input , Scheduler.TaskPool);

            myObs.Subscribe(
                onNext: _ =>
                {
                    result = _;
                    are.Set();
                },
                onError: _ =>
                {
                    are.Set();
                },
                onCompleted: () => { are.Set(); }
                );

            are.WaitOne();

            Assert.IsTrue(result != null);
            var res = result.Result;
            Assert.IsTrue(res is IList<string>);
            var il = res as IList<string>;
            Assert.IsTrue(il[0] == "one");
            Assert.IsTrue(il[1] == "two");
            Assert.IsTrue(il[2] == "three");
            Assert.IsTrue(il[3] == input.ToString());
            Assert.IsTrue(il.Count == 4);
        }

        [TestMethod]
        public void TestInt()
        {
            AutoResetEvent are = new AutoResetEvent(false);
            var rpc = new JsonRpcClient(remoteUri);
            string method = "testInt";
            int input = 7;
            JsonResponse<string[]> result = null;
            var myObs = rpc.Invoke<string[]>(method,  input , Scheduler.TaskPool);

            myObs.Subscribe(
                onNext: _ =>
                {
                    result = _;
                    are.Set();
                },
                onError: _ =>
                {
                    are.Set();
                },
                onCompleted: () => { are.Set(); }
                );

            are.WaitOne();

            Assert.IsTrue(result != null);
            var res = result.Result;
            Assert.IsTrue(res is IList<string>);
            var il = res as IList<string>;
            Assert.IsTrue(il[0] == "one");
            Assert.IsTrue(il[1] == "two");
            Assert.IsTrue(il[2] == "three");
            Assert.IsTrue(il[3] == input.ToString());
            Assert.IsTrue(il.Count == 4);
        }

        [TestMethod]
        public void TestSimpleString()
        {
            AutoResetEvent are = new AutoResetEvent(false);
            var rpc = new JsonRpcClient(remoteUri);
            string method = "testSimpleString";
            string input = "Hello";
            JsonResponse<string[]> result = null;
            var myObs = rpc.Invoke<string[]>(method, input, Scheduler.TaskPool);

            myObs.Subscribe(
                onNext: _ =>
                {
                    result = _;
                    are.Set();
                },
                onError: _ =>
                {
                    are.Set();
                },
                onCompleted: () => { are.Set(); }
                );


            are.WaitOne();

            Assert.IsTrue(result != null);
            var res = result.Result;
            Assert.IsTrue(res is IList<string>);
            var il = res as IList<string>;
            Assert.IsTrue(il[0] == "one");
            Assert.IsTrue(il[1] == "two");
            Assert.IsTrue(il[2] == "three");
            Assert.IsTrue(il[3] == input.ToString());
            Assert.IsTrue(il.Count == 4);
        }

        [TestMethod]
        public void TestThrowingException()
        {
            AutoResetEvent are = new AutoResetEvent(false);
            var rpc = new JsonRpcClient(remoteUri);
            string method = "testThrowingException";
            string input = "Hello";
            JsonResponse<string[]> result = null;
            var myObs = rpc.Invoke<string[]>(method, input, Scheduler.TaskPool);

            myObs.Subscribe(
                onNext: _ =>
                {
                    result = _;
                    are.Set();
                },
                onError: _ =>
                {
                    are.Set();
                },
                onCompleted: () => { are.Set(); }
                );


            are.WaitOne();

            Assert.IsTrue(result != null);
            Assert.IsTrue(result.Result == null);
            var res = result.Error;
            Assert.IsTrue(res is AustinHarris.JsonRpc.JsonRpcException);
            if (res is JsonRpcException)
            {
                Assert.IsTrue(res.message == "Internal Error");
            }
        }

        [TestMethod]
        public void TestException()
        {
            AutoResetEvent are = new AutoResetEvent(false);
            var rpc = new JsonRpcClient(remoteUri);
            string method = "error1";
            string input = "Hello";
            JsonResponse<string[]> result = null;
            var myObs = rpc.Invoke<string[]>(method, input, Scheduler.TaskPool);

            myObs.Subscribe(
                onNext: _ =>
                {
                    result = _;
                    are.Set();
                },
                onError: _ =>
                {
                    are.Set();
                },
                onCompleted: () => { are.Set(); }
                );


            are.WaitOne();

            Assert.IsTrue(result != null);
            Assert.IsTrue(result.Result == null);
            var res = result.Error;
            Assert.IsTrue(res is AustinHarris.JsonRpc.JsonRpcException);
            if (res is JsonRpcException)
            {
                Assert.IsTrue(res.message == "Internal Error");
            }
        }

        [TestMethod]
        public void TestrefException()
        {
            AutoResetEvent are = new AutoResetEvent(false);
            var rpc = new JsonRpcClient(remoteUri);
            string method = "error2";
            string input = "Hello";
            JsonResponse<string[]> result = null;
            var myObs = rpc.Invoke<string[]>(method, input, Scheduler.TaskPool);

            myObs.Subscribe(
                onNext: _ =>
                {
                    result = _;
                    are.Set();
                },
                onError: _ =>
                {
                    are.Set();
                },
                onCompleted: () => { are.Set(); }
                );


            are.WaitOne();

            Assert.IsTrue(result != null);
            Assert.IsTrue(result.Result == null);
            var res = result.Error;
            Assert.IsTrue(res is AustinHarris.JsonRpc.JsonRpcException);
            if (res is JsonRpcException)
            {
                Assert.IsTrue(res.message == "This exception was thrown using: ref JsonRpcException");
            }
        }

        [TestMethod]
        public void TestThrowingJsonRpcException()
        {
            AutoResetEvent are = new AutoResetEvent(false);
            var rpc = new JsonRpcClient(remoteUri);
            string method = "error3";
            string input = "Hello";
            JsonResponse<string[]> result = null;
            var myObs = rpc.Invoke<string[]>(method, input, Scheduler.TaskPool);

            myObs.Subscribe(
                onNext: _ =>
                {
                    result = _;
                    are.Set();
                },
                onError: _ =>
                {
                    are.Set();
                },
                onCompleted: () => { are.Set(); }
                );


            are.WaitOne();

            Assert.IsTrue(result != null);
            Assert.IsTrue(result.Result == null);
            var res = result.Error;
            Assert.IsTrue(res is AustinHarris.JsonRpc.JsonRpcException);
            if (res is JsonRpcException)
            {
                Assert.IsTrue(res.message == "This exception was thrown using: throw new JsonRpcException()");
            }
        }

        [TestMethod]
        public void TestSettingJsonRpcExceptionWithContext()
        {
            AutoResetEvent are = new AutoResetEvent(false);
            var rpc = new JsonRpcClient(remoteUri);
            string method = "error4";
            string input = "Hello";
            JsonResponse<string[]> result = null;
            var myObs = rpc.Invoke<string[]>(method, input, Scheduler.TaskPool);

            myObs.Subscribe(
                onNext: _ =>
                {
                    result = _;
                    are.Set();
                },
                onError: _ =>
                {
                    are.Set();
                },
                onCompleted: () => { are.Set(); }
                );


            are.WaitOne();

            Assert.IsTrue(result != null);
            Assert.IsTrue(result.Result == null);
            var res = result.Error;
            Assert.IsTrue(res is AustinHarris.JsonRpc.JsonRpcException);
            if (res is JsonRpcException)
            {
                Assert.IsTrue(res.message == "This exception was thrown using: JsonRpcContext.Current().SetException()");
            }
        }

        [TestMethod]
        public void TestPreProcessingException()
        {
            AutoResetEvent are = new AutoResetEvent(false);
            var rpc = new JsonRpcClient(remoteUri);
            string method = "RequiresCredentials";
            string input = "BadPassword";
            JsonResponse<string[]> result = null;
            var myObs = rpc.Invoke<string[]>(method, input, Scheduler.TaskPool);

            myObs.Subscribe(
                onNext: _ =>
                {
                    result = _;
                    are.Set();
                },
                onError: _ =>
                {
                    are.Set();
                },
                onCompleted: () => { are.Set(); }
                );


            are.WaitOne();

            Assert.IsTrue(result != null);
            Assert.IsTrue(result.Result == null);
            var res = result.Error;
            Assert.IsTrue(res is AustinHarris.JsonRpc.JsonRpcException);
            if (res is JsonRpcException)
            {
                Assert.IsTrue(res.message == "This exception was thrown using: JsonRpcTest.Global.PreProcess, Not Authenticated");
            }
        }

        [TestMethod]
        public void TestCustomString()
        {
            AutoResetEvent are = new AutoResetEvent(false);
            var rpc = new JsonRpcClient(remoteUri);
            string method = "testCustomString";
            var input = new { str = "Hello" };
            JsonResponse<string[]> result = null;
            var myObs = rpc.Invoke<string[]>(method, input, Scheduler.TaskPool);
            
            myObs.Subscribe(
                onNext: _ =>
                    {
                        result = _;
                        are.Set();
                    },
                onError: _ =>
                    {
                        are.Set();                    
                    }, 
                onCompleted: () => { are.Set(); }
                );

            are.WaitOne();

            Assert.IsTrue(result != null);
            var res = result.Result;
            Assert.IsTrue(res is IList<string>);
            var il = res as IList<string>;
            Assert.IsTrue(il[0] == "one");
            Assert.IsTrue(il[1] == "two");
            Assert.IsTrue(il[2] == "three");
            Assert.IsTrue(il[3] == input.str);
            Assert.IsTrue(il.Count == 4);
        }

        [TestMethod]
        public void TestMultipleParameters()
        {
            AutoResetEvent are = new AutoResetEvent(false);
            var rpc = new JsonRpcClient(remoteUri);
            string method = "testMultipleParameters";
            var anon = new CustomString { str = "Hello" };
            var input = new object[] {"one", 2, 3.3f, anon};
            JsonResponse<object[]> result = null;
            var myObs = rpc.Invoke<object[]>(method, input, Scheduler.TaskPool);

            myObs.Subscribe(
                onNext: _ =>
                {
                    result = _;
                    are.Set();
                },
                onError: _ =>
                {
                    are.Set();
                },
                onCompleted: () => { are.Set(); }
                );

            are.WaitOne();

            Assert.IsTrue(result != null);
            Assert.IsTrue(result.Result != null);
            var res = result.Result;
            Assert.IsTrue(res.Length == 4);
            Assert.IsTrue((string)res[0] == (string)input[0]);
            Assert.IsTrue((long)res[1] == (long)(int)input[1]);
            Assert.IsTrue((double)res[2] == Double.Parse(input[2].ToString()));
            Assert.IsTrue(Newtonsoft.Json.JsonConvert.DeserializeObject<CustomString>(res[3].ToString()).str == anon.str);
        }

        [TestMethod]
        public void TestJsonpWithHttpGet()
        {
            string method = "internal.echo";
            string input = "Echo this sucka";
            string id = "1";
            string callbackName = "myCallback";
            object[] parameters = new object[1];
            parameters[0] = input;

            JsonRequest jsonParameters = new JsonRequest()
            {
                Method = method,
                Params = parameters,
                Id = id
            };
            var serailaizedParameters = Newtonsoft.Json.JsonConvert.SerializeObject(jsonParameters);
            string uri = string.Format("{0}?jsonrpc={1}&callback={2}", remoteUri, serailaizedParameters, callbackName, id);

            WebRequest request = WebRequest.Create(uri);
            WebResponse response = request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());

            var regexPattern = callbackName + @"\({.*}\)";

            var result = reader.ReadToEnd().Trim();

            Assert.IsTrue(Regex.IsMatch(result, regexPattern));
        }

        [TestMethod]
        public void TestJsonPWithHttpPost()
        {
            string method = "internal.echo";
            string input = "Echo this sucka";
            string id = "1";
            string callbackName = "myCallback";
            object[] parameters = new object[1];
            parameters[0] = input;

            JsonRequest jsonParameters = new JsonRequest()
            {
                Method = method,
                Params = parameters,
                Id = id
            };

            var serailaizedParameters = Newtonsoft.Json.JsonConvert.SerializeObject(jsonParameters);
            var postData = string.Format("jsonrpc={0}&callback={1}", serailaizedParameters, callbackName);

            WebRequest request = WebRequest.Create(remoteUri);
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";

            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(postData);
            request.ContentLength = bytes.Length;

            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(bytes, 0, bytes.Length);
                stream.Close();
            }

            WebResponse response = request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            //myCallback({"jsonrpc":"2.0","result":"Echo this sucka","id":"1"})
            var regexPattern = callbackName + @"\({.*}\)";
            var result = reader.ReadToEnd().Trim();

            Assert.IsTrue(Regex.IsMatch(result, regexPattern));
        }
        public class CustomString
        {
            public string str;
        }
    }
}
