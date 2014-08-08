#region License
// 
//     CoiniumServ - Crypto Currency Mining Pool Server Software
//     Copyright (C) 2013 - 2014, CoiniumServ Project - http://www.coinium.org
//     http://www.coiniumserv.com - https://github.com/CoiniumServ/CoiniumServ
// 
//     This software is dual-licensed: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
//    
//     For the terms of this license, see licenses/gpl_v3.txt.
// 
//     Alternatively, you can license this software under a commercial
//     license or white-label it as set out in licenses/commercial.txt.
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
