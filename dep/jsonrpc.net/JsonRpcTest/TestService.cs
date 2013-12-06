namespace TestServer
{
    using System;
    using System.Collections.Generic;
    using AustinHarris.JsonRpc;
    using Newtonsoft.Json.Linq;

    public class TestService: JsonRpcService
    {
        [JsonRpcMethod("internal.echo")]
        private string Handle_Echo(string s)
        {
            return s;
        }


        [JsonRpcMethod]
        private string myIp()
        {
            var req = JsonRpcContext.Current().Value as System.Web.HttpRequest;
            if (req != null)
                return req.UserHostAddress;
            return "IP not available";
        }

        [JsonRpcMethod]
        private string myUserAgent()
        {
            var req = JsonRpcContext.Current().Value as System.Web.HttpRequest;
            if (req != null)
                return req.UserAgent.ToString();
            return "hmm. no UserAgent";
        }

        [JsonRpcMethod("error1")]
        private string devideByZero(string s)
        {
            var i = 0;
            var j = 15;
            return s + j / i; // This causes the framework to throw an exception
        }

        [JsonRpcMethod("error2")]
        private string throwsException(string s, ref JsonRpcException refException)
        {
            refException = new JsonRpcException(-1, "This exception was thrown using: ref JsonRpcException", null);
            return s;
        }

        [JsonRpcMethod("error3")]
        private string throwsException2(string s)
        {
            throw new JsonRpcException(-27000, "This exception was thrown using: throw new JsonRpcException()", null);
            return s;
        }

        [JsonRpcMethod("error4")]
        private string throwsException3(string s)
        {
            JsonRpcContext.SetException(new JsonRpcException(-27000, "This exception was thrown using: JsonRpcContext.Current().SetException()", null));
            return s;
        }

        [JsonRpcMethod]
        private string RequiresCredentials(string magicKey)
        {
            return "Passed Authentication";
        }

        [JsonRpcMethod]
        private DateTime testDateTime()
        {
            return DateTime.Now;
        }

        [JsonRpcMethod]
        private recursiveClass testRecursiveClass()
        {
            var obj = new recursiveClass() { Value1 = 10, Nested1 = new recursiveClass() { Value1 = 5 } };
            //obj.Nested1.Nested1 = obj;
            return obj;
        }

        [JsonRpcMethod]
        private JObject testArbitraryJObject(JObject input)
        {
            return input;
        }

        [JsonRpcMethod]
        private List<string> testFloat(float input)
        {
            return new List<string>() { "one", "two", "three", input.ToString() };
        }

        [JsonRpcMethod]
        private List<string> testInt(int input)
        {
            return new List<string>() { "one", "two", "three", input.ToString() };
        }

        [JsonRpcMethod]
        private object[] testMultipleParameters(string one, int two, float three, CustomString four)
        {
            return new object[] { one, two, three, four };
        }

        [JsonRpcMethod("testSimpleString")]
        private List<string> testSimpleString(string input)
        {
            return new List<string>() { "one", "two", "three", input };
        }

        [JsonRpcMethod]
        private List<string> testThrowingException(string input)
        {
            throw new Exception("Throwing Exception");
            return new List<string>() { "one", "two", "three", input };
        }

        public class CustomString
        {
            public string str;
        }

        [JsonRpcMethod("testCustomString")]
        private List<string> testCustomString(CustomString input)
        {
            return new List<string>() { "one", "two", "three", input.str };
        }

        private class recursiveClass
        {
            public recursiveClass Nested1 { get; set; }
            public int Value1 { get; set; }
        }
    }
}