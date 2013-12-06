using AustinHarris.JsonRpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestServer_Console
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
        public decimal? Test2(decimal x)
        {
            return x;
        }
    }
}
