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

using System;
using System.Text;
using CoiniumServ.Coin.Address;
using CoiniumServ.Coin.Address.Exceptions;
using Org.BouncyCastle.Math;
using Should.Fluent;
using Xunit;

namespace CoiniumServ.Tests.Coin
{
        public class Base58Test
        {
            [Fact]
            public void TestEncode()
            {
                var testbytes = Encoding.UTF8.GetBytes("Hello World");
                Base58.Encode(testbytes).Should().Equal("JxF12TrwUP45BMd");

                var bi = BigInteger.ValueOf(3471844090);
                Base58.Encode(bi.ToByteArray()).Should().Equal("16Ho7Hs");
            }

            [Fact]
            public void TestDecode()
            {
                var testbytes = Encoding.UTF8.GetBytes("Hello World");
                var actualbytes = Base58.Decode("JxF12TrwUP45BMd");
                actualbytes.Should().Equal(testbytes);
            }

            [Fact]
            public void TestInvalidAddress()
            {
                Exception ex = Assert.Throws<AddressFormatException>(() => Base58.Decode("This isn't valid base58"));
            }
        }
}
