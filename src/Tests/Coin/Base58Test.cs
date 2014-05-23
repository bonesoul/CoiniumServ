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
using System.Text;
using Coinium.Coin.Address;
using Coinium.Coin.Exceptions;
using Org.BouncyCastle.Math;
using Should.Fluent;
using Xunit;

namespace Tests.Coin
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
