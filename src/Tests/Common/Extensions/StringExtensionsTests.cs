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

using Coinium.Common.Extensions;
using Should.Fluent;
using Xunit;

namespace Tests.Common.Extensions
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
