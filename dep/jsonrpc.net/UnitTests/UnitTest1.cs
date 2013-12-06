using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        static object[] services = new object[] {
           new CalculatorService()
        };

        [TestMethod]
        public void TestInProcessClient()
        {
            AsyncTestInProcessClient();
        }
        public async void AsyncTestInProcessClient()
        {
            string request = @"{method:'NullableFloatToNullableFloat',params:[0.0],id:1}";
            string expectedResult = "{\"jsonrpc\":\"2.0\",\"result\":0.0,\"id\":1}";
            var result = await AustinHarris.JsonRpc.Client.InProcessClient.Invoke(request);
            Assert.AreEqual(result, expectedResult);
        }

        [TestMethod]
        public void NullableFloatToNullableFloat()
        {
            AsyncNullableFloatToNullableFloat();
        }
        public async void AsyncNullableFloatToNullableFloat()
        {
            string request = @"{method:'NullableFloatToNullableFloat',params:[1.2345],id:1}";
            string expectedResult = "{\"jsonrpc\":\"2.0\",\"result\":1.2345,\"id\":1}";
            var result = await AustinHarris.JsonRpc.Client.InProcessClient.Invoke(request);
            Assert.AreEqual(result, expectedResult);
        }
        [TestMethod]
        public void NullableFloatToNullableFloat2()
        {
            AsyncNullableFloatToNullableFloat2();
        }
        public async void AsyncNullableFloatToNullableFloat2()
        {
            string request = @"{method:'NullableFloatToNullableFloat',params:[null],id:1}";
            string expectedResult = "{\"jsonrpc\":\"2.0\",\"id\":1}";
            var result = await AustinHarris.JsonRpc.Client.InProcessClient.Invoke(request);
            Assert.AreEqual(result, expectedResult);
        }

        [TestMethod]
        public void DecimalToNullableDecimal()
        {
            AsyncDecimalToNullableDecimal();
        }
        public async void AsyncDecimalToNullableDecimal()
        {
            string request = @"{method:'DecimalToNullableDecimal',params:[0.0],id:1}";
            string expectedResult = "{\"jsonrpc\":\"2.0\",\"result\":0.0,\"id\":1}";
            var result = await AustinHarris.JsonRpc.Client.InProcessClient.Invoke(request);
            Assert.AreEqual(result, expectedResult);
        }

        [TestMethod]
        public void StringToListOfString()
        {
            AsyncStringToListOfString();
        }
        public async void AsyncStringToListOfString()
        {
            string request = @"{method:'StringToListOfString',params:['some string'],id:1}";
            string expectedResult = "{\"jsonrpc\":\"2.0\",\"result\":[\"one\",\"two\",\"three\",\"some string\"],\"id\":1}";
            var result = await AustinHarris.JsonRpc.Client.InProcessClient.Invoke(request);
            Assert.AreEqual(result, expectedResult);
        }

        [TestMethod]
        public void CustomStringToListOfString()
        {
            AsyncCustomStringToListOfString();
        }
        public async void AsyncCustomStringToListOfString()
        {
            string request = @"{method:'CustomStringToListOfString',params:[{str:'some string'}],id:1}";
            string expectedResult = "{\"jsonrpc\":\"2.0\",\"result\":[\"one\",\"two\",\"three\",\"some string\"],\"id\":1}";
            var result = await AustinHarris.JsonRpc.Client.InProcessClient.Invoke(request);
            Assert.AreEqual(result, expectedResult);
        }

        [TestMethod]
        public void StringToThrowingException()
        {
            AsyncStringToThrowingException();
        }
        public async void AsyncStringToThrowingException()
        {
            string request = @"{method:'StringToThrowingException',params:['some string'],id:1}";
            string expectedResult = "{\"jsonrpc\":\"2.0\",\"error\":{\"code\":-32603,\"message\":\"Internal Error\",\"data\":";
            var result = await AustinHarris.JsonRpc.Client.InProcessClient.Invoke(request);
            Assert.IsTrue(result.StartsWith(expectedResult));
        }

        [TestMethod]
        public void StringToRefException()
        {
            AsyncStringToRefException();
        }
        public async void AsyncStringToRefException()
        {
            string request = @"{method:'StringToRefException',params:['some string'],id:1}";
            string expectedResult = "{\"jsonrpc\":\"2.0\",\"error\":{\"code\":-1,\"message\":\"refException worked\"";
            var result = await AustinHarris.JsonRpc.Client.InProcessClient.Invoke(request);
            Assert.IsTrue(result.StartsWith(expectedResult));
        }

        [TestMethod]
        public void StringToThrowJsonRpcException()
        {
            AsyncStringToThrowJsonRpcException();
        }
        public async void AsyncStringToThrowJsonRpcException()
        {
            string request = @"{method:'StringToThrowJsonRpcException',params:['some string'],id:1}";
            string expectedResult = "{\"jsonrpc\":\"2.0\",\"error\":{\"code\":-27000,\"message\":\"Just some testing\"";
            var result = await AustinHarris.JsonRpc.Client.InProcessClient.Invoke(request);
            Assert.IsTrue(result.StartsWith(expectedResult));
        }

        [TestMethod]
        public void ReturnsDateTime()
        {
            AsyncReturnsDateTime();
        }
        public async void AsyncReturnsDateTime()
        {
            string request = @"{method:'ReturnsDateTime',params:[],id:1}";
            var result = await AustinHarris.JsonRpc.Client.InProcessClient.Invoke(request);
            Assert.IsFalse(result.Contains("error"));
        }

        [TestMethod]
        public void ReturnsCustomRecursiveClass()
        {
            AsyncReturnsCustomRecursiveClass();
        }
        public async void AsyncReturnsCustomRecursiveClass()
        {
            string request = @"{method:'ReturnsCustomRecursiveClass',params:[],id:1}";
            string expectedResult = "{\"jsonrpc\":\"2.0\",\"result\":{\"Nested1\":{\"Nested1\":null,\"Value1\":5},\"Value1\":10},\"id\":1}";
            var result = await AustinHarris.JsonRpc.Client.InProcessClient.Invoke(request);
            Assert.IsFalse(result.Contains("error"));
        }


        [TestMethod]
        public void FloatToFloat()
        {
            AsyncFloatToFloat();
        }
        public async void AsyncFloatToFloat()
        {
            string request = @"{method:'FloatToFloat',params:[0.123],id:1}";
            string expectedResult = "{\"jsonrpc\":\"2.0\",\"result\":0.123,\"Value1\":10},\"id\":1}";
            var result = await AustinHarris.JsonRpc.Client.InProcessClient.Invoke(request);
            Assert.IsFalse(result.Contains("error"));
        }


        [TestMethod]
        public void IntToInt()
        {
            AsyncIntToInt();
        }
        public async void AsyncIntToInt()
        {
            string request = @"{method:'IntToInt',params:[789],id:1}";
            string expectedResult = "{\"jsonrpc\":\"2.0\",\"result\":789,\"id\":1}";
            var result = await AustinHarris.JsonRpc.Client.InProcessClient.Invoke(request);
            Assert.IsFalse(result.Contains("error"));
        }

        [TestMethod]
        public void Int16ToInt16()
        {
            AsyncInt16ToInt16();
        }
        public async void AsyncInt16ToInt16()
        {
            string request = @"{method:'Int16ToInt16',params:[789],id:1}";
            string expectedResult = "{\"jsonrpc\":\"2.0\",\"result\":789,\"id\":1}";
            var result = await AustinHarris.JsonRpc.Client.InProcessClient.Invoke(request);
            Assert.IsFalse(result.Contains("error"));
        }

        [TestMethod]
        public void Int32ToInt32()
        {
            AsyncInt32ToInt32();
        }
        public async void AsyncInt32ToInt32()
        {
            string request = @"{method:'Int32ToInt32',params:[789],id:1}";
            string expectedResult = "{\"jsonrpc\":\"2.0\",\"result\":789,\"id\":1}";
            var result = await AustinHarris.JsonRpc.Client.InProcessClient.Invoke(request);
            Assert.IsFalse(result.Contains("error"));
        }

        [TestMethod]
        public void Int64ToInt64()
        {
            AsyncInt64ToInt64();
        }
        public async void AsyncInt64ToInt64()
        {
            string request = @"{method:'Int64ToInt64',params:[78915984515564],id:1}";
            string expectedResult = "{\"jsonrpc\":\"2.0\",\"result\":78915984515564,\"id\":1}";
            var result = await AustinHarris.JsonRpc.Client.InProcessClient.Invoke(request);
            Assert.IsFalse(result.Contains("error"));
        }

    }
}
