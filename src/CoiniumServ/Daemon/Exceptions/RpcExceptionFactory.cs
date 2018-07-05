#region License
// 
//     MIT License
//
//     CoiniumServ - Crypto Currency Mining Pool Server Software
//     Copyright (C) 2013 - 2017, CoiniumServ Project
//     Hüseyin Uslu, shalafiraistlin at gmail dot com
//     https://github.com/bonesoul/CoiniumServ
// 
//     Permission is hereby granted, free of charge, to any person obtaining a copy
//     of this software and associated documentation files (the "Software"), to deal
//     in the Software without restriction, including without limitation the rights
//     to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//     copies of the Software, and to permit persons to whom the Software is
//     furnished to do so, subject to the following conditions:
//     
//     The above copyright notice and this permission notice shall be included in all
//     copies or substantial portions of the Software.
//     
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//     FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//     LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//     OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//     SOFTWARE.
// 
#endregion

using System;
using CoiniumServ.Daemon.Errors;

namespace CoiniumServ.Daemon.Exceptions
{
    public class RpcExceptionFactory:IRpcExceptionFactory
    {
        public RpcException GetRpcException(Exception inner)
        {
            if (inner.Message.Equals("The operation has timed out", StringComparison.OrdinalIgnoreCase))
            {
                return new RpcTimeoutException(inner);
            }

            if (inner.Message.Equals("Unable to connect to the remote server", StringComparison.OrdinalIgnoreCase))
                return new RpcConnectionException(inner);

            return new RpcException(inner);
        }

        public RpcException GetRpcException(string message, Exception inner)
        {
            return new RpcException(message, inner);
        }

        public RpcException GetRpcErrorException(RpcErrorResponse response)
        {
            return new RpcErrorException(response);
        }
    }
}
