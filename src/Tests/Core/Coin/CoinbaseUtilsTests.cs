/*
 *   Coinium - Crypto Currency Pool Software - https://github.com/CoiniumServ/CoiniumServ
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

using Coinium.Common.Helpers.Arrays;
using Coinium.Core.Coin;
using Xunit;

namespace Tests.Core.Coin
{
    public class CoinbaseUtilsTests
    {
        [Fact]
        public void SerializeNumberTests()
        {
            // =< 16 test
            var buffer = CoinbaseUtils.SerializeNumber(16);
            var expected = new byte[] { 0x01, 0x10 };
            ArrayHelpers.CompareByteArrays(buffer, expected);
            
            // > 16 test
            buffer = CoinbaseUtils.SerializeNumber(10000);
            expected = new byte[] {0x02, 0x10, 0x27};
            ArrayHelpers.CompareByteArrays(buffer, expected);
        }
    }
}
