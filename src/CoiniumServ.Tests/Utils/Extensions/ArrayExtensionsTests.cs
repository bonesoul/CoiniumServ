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

using System.Linq;
using CoiniumServ.Utils.Extensions;
using Should.Fluent;
using Xunit;

namespace CoiniumServ.Tests.Utils.Extensions
{
    public class ArrayExtensionsTests
    {
        [Fact]
        public void ToHexStringTest()
        {
            var input = new byte[]
            {
                0x71, 0xf6, 0x39, 0x1e, 0x10, 0x60, 0x0e, 0x54, 0xd8, 0x3b, 0xd8, 0xc0, 0x26, 0x53, 0x03, 0xcc,
                0xca, 0xaf, 0xe3, 0x75, 0xbc, 0xc3, 0xb0, 0x11, 0x72, 0x96, 0xeb, 0xd1, 0x44, 0xe3, 0xf9, 0x5a
            };

            var output = input.ToHexString();
            output.Should().Equal("71f6391e10600e54d83bd8c0265303cccaafe375bcc3b0117296ebd144e3f95a");
        }

        [Fact]
        public void ToHexStringFromEnumareableTest()
        {
            var input = new byte[]
            {
                0x71, 0xf6, 0x39, 0x1e, 0x10, 0x60, 0x0e, 0x54, 0xd8, 0x3b, 0xd8, 0xc0, 0x26, 0x53, 0x03, 0xcc,
                0xca, 0xaf, 0xe3, 0x75, 0xbc, 0xc3, 0xb0, 0x11, 0x72, 0x96, 0xeb, 0xd1, 0x44, 0xe3, 0xf9, 0x5a
            }.Take(32);

            var output = input.ToHexString();
            output.Should().Equal("71f6391e10600e54d83bd8c0265303cccaafe375bcc3b0117296ebd144e3f95a");            
        }

        [Fact]
        public void ReverseByteOrderTest()
        {
            /*  sample data:
                orig    : 2fb6f87a2eaa2f5ca970ef355d66d394ab907fb1ebe3b7688e5979e7a4d2120a
                mid     : 7af8b62f5c2faa2e35ef70a994d3665db17f90ab68b7e3ebe779598e0a12d2a4
                reversed: a4d2120a8e5979e7ebe3b768ab907fb15d66d394a970ef352eaa2f5c2fb6f87a
             */

            var input = "2fb6f87a2eaa2f5ca970ef355d66d394ab907fb1ebe3b7688e5979e7a4d2120a".HexToByteArray();
            var output = input.ReverseByteOrder().ToHexString();

            output.Should().Equal("a4d2120a8e5979e7ebe3b768ab907fb15d66d394a970ef352eaa2f5c2fb6f87a");
        }

        [Fact]
        public void ReverseBufferTest()
        {
            /*  sample data:
                previousblockhash: 5af9e344d1eb967211b0c3bc75e3afcacc035326c0d83bd8540e60101e39f671 
                previousblockhashreversed: 71f6391e10600e54d83bd8c0265303cccaafe375bcc3b0117296ebd144e3f95a
             */

            var input = "5af9e344d1eb967211b0c3bc75e3afcacc035326c0d83bd8540e60101e39f671".HexToByteArray();
            var output = input.ReverseBuffer().ToHexString();

            output.Should().Equal("71f6391e10600e54d83bd8c0265303cccaafe375bcc3b0117296ebd144e3f95a");
        }
    }
}
