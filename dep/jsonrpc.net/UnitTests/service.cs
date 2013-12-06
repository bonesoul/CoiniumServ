using AustinHarris.JsonRpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitTests
{
    public class CalculatorService : JsonRpcService
    {
        [JsonRpcMethod]
        private double add(double l, double r)
        {
            return l + r;
        }

        [JsonRpcMethod]
        public float? NullableFloatToNullableFloat(float? a)
        {
            return a;
        }

        [JsonRpcMethod]
        public decimal? DecimalToNullableDecimal(decimal x)
        {
            return x;
        }

        [JsonRpcMethod]
        private List<string> StringToListOfString(string input)
        {
            return new List<string>() { "one", "two", "three", input };
        }

        [JsonRpcMethod]
        private List<string> StringToThrowingException(string input)
        {
            throw new Exception("Throwing Exception");
            return new List<string>() { "one", "two", "three", input };
        }

        public class CustomString
        {
            public string str;
        }

        [JsonRpcMethod]
        private List<string> CustomStringToListOfString(CustomString input)
        {
            return new List<string>() { "one", "two", "three", input.str };
        }



        [JsonRpcMethod("internal.echo")]
        private string Handle_Echo(string s)
        {
            return s;
        }

        [JsonRpcMethod("error1")]
        private string devideByZero(string s)
        {
            var i = 0;
            var j = 15;
            return s + j / i;
        }

        [JsonRpcMethod]
        private string StringToRefException(string s, ref JsonRpcException refException)
        {
            refException = new JsonRpcException(-1, "refException worked", null);
            return s;
        }

        [JsonRpcMethod("error3")]
        private string StringToThrowJsonRpcException(string s)
        {
            throw new JsonRpcException(-27000, "Just some testing", null);
            return s;
        }

        [JsonRpcMethod]
        private DateTime ReturnsDateTime()
        {
            return DateTime.Now;
        }

        [JsonRpcMethod]
        private recursiveClass ReturnsCustomRecursiveClass()
        {
            var obj = new recursiveClass() { Value1 = 10, Nested1 = new recursiveClass() { Value1 = 5 } };
            //obj.Nested1.Nested1 = obj;
            return obj;
        }

        [JsonRpcMethod]
        private float FloatToFloat(float input)
        {
            return input;
        }

        [JsonRpcMethod]
        private double DoubleToDouble(double input)
        {
            return input;
        }



        [JsonRpcMethod]
        private int IntToInt(int input)
        {
            return input;
        }

        [JsonRpcMethod]
        private Int32 Int32ToInt32(Int32 input)
        {
            return input;
        }

        [JsonRpcMethod]
        private Int16 Int16ToInt16(Int16 input)
        {
            return input;
        }

        [JsonRpcMethod]
        private Int64 Int64ToInt64(Int64 input)
        {
            return input;
        }

        private class recursiveClass
        {
            public recursiveClass Nested1 { get; set; }
            public int Value1 { get; set; }
        }
    }
}
