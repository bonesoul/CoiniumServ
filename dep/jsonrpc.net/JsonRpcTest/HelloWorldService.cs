namespace TestServer{
    using System;
    using AustinHarris.JsonRpc;

    public class HelloWorldService: JsonRpcService{
        [JsonRpcMethod]
        private string helloWorld(string message){
            return "Hello World "+ message;
        }
    }
}