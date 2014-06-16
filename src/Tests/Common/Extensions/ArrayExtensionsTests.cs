/*
 *   CoiniumServ - crypto currency pool software - https://github.com/CoiniumServ/CoiniumServ
 *   Copyright (C) 2013 - 2014, Coinium Project - http://www.coinium.org
 *
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Coinium.Common.Extensions;
using Should.Fluent;
using Xunit;

namespace Tests.Common.Extensions
{
    public class ArrayExtensionsTests
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
