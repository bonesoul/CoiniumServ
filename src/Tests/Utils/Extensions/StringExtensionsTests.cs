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

using CoiniumServ.Utils.Extensions;
using Should.Fluent;
using Xunit;

namespace CoiniumServ.Tests.Utils.Extensions
{
    public class StringExtensionsTests
    {
        [Fact]
        public void HexToByteArrayTest()
        {
            const string input = "22a9174d9db64f1919febc9577167764c301b755768b675291f7d34454561e9e";
            var output = input.HexToByteArray();

            output.Should().Equal(new byte[]
            {
                0x22, 0xa9, 0x17, 0x4d, 0x9d, 0xb6, 0x4f, 0x19, 0x19, 0xfe, 0xbc, 0x95, 0x77, 0x16, 0x77, 0x64,
                0xc3, 0x01, 0xb7, 0x55, 0x76, 0x8b, 0x67, 0x52, 0x91, 0xf7, 0xd3, 0x44, 0x54, 0x56, 0x1e, 0x9e 
            });
        }
    }
}
