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
using Coinium.Transactions.Coinbase;
using Should.Fluent;
using Xunit;

namespace Tests.Transactions.Coinbase
{
    public class CoinbaseUtilsTests
    {
        /// <summary>
        /// Tests <see cref="CoinbaseUtils.VarInt"/>.
        /// </summary>
        [Fact]
        public void VarIntegerTest()
        {
            // < 0xfd test
            var varInt = CoinbaseUtils.VarInt(0xfc);
            var expected = new byte[] {0xfc};
            Assert.Equal(varInt, expected);

            // < 0xffff
            varInt = CoinbaseUtils.VarInt(0xfffe);
            expected = new byte[] {0xfd, 0xfe, 0xff};
            Assert.Equal(varInt, expected);

            // 0xffffffff
            varInt = CoinbaseUtils.VarInt(0xfffffffe);
            expected = new byte[] {0xfe, 0xfe, 0xff, 0xff, 0xff};
            Assert.Equal(varInt, expected);
        }


        /// <summary>
        /// Tests <see cref="CoinbaseUtils.SerializeNumber"/>.
        /// </summary>
        [Fact]
        public void SerializeNumberTest()
        {
            // =< 16 test
            var buffer = CoinbaseUtils.SerializeNumber(16);
            var expected = new byte[] { 0x01, 0x10 };
            Assert.Equal(buffer, expected);
            
            // > 16 test
            buffer = CoinbaseUtils.SerializeNumber(10000);
            expected = new byte[] {0x02, 0x10, 0x27};
            Assert.Equal(buffer, expected);
        }

        /// <summary>
        /// Tests <see cref="CoinbaseUtils.SerializeString"/>.
        /// </summary>
        [Fact]
        public void SerializeStringTest()
        {
            // < 253 test
            var serialized = CoinbaseUtils.SerializeString("https://github.com/CoiniumServ/CoiniumServ");
            serialized.ToHexString().Should().Equal("2a68747470733a2f2f6769746875622e636f6d2f436f696e69756d536572762f436f696e69756d53657276");

            // >= 253 & <65536 (0x10000) test
            serialized = CoinbaseUtils.SerializeString(@"Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.");
            serialized.ToHexString().Should().Equal("fd3e024c6f72656d20497073756d2069732073696d706c792064756d6d792074657874206f6620746865207072696e74696e6720616e64207479706573657474696e6720696e6475737472792e204c6f72656d20497073756d20686173206265656e2074686520696e6475737472792773207374616e646172642064756d6d79207465787420657665722073696e6365207468652031353030732c207768656e20616e20756e6b6e6f776e207072696e74657220746f6f6b20612067616c6c6579206f66207479706520616e6420736372616d626c656420697420746f206d616b65206120747970652073706563696d656e20626f6f6b2e20497420686173207375727669766564206e6f74206f6e6c7920666976652063656e7475726965732c2062757420616c736f20746865206c65617020696e746f20656c656374726f6e6963207479706573657474696e672c2072656d61696e696e6720657373656e7469616c6c7920756e6368616e6765642e2049742077617320706f70756c61726973656420696e207468652031393630732077697468207468652072656c65617365206f66204c657472617365742073686565747320636f6e7461696e696e67204c6f72656d20497073756d2070617373616765732c20616e64206d6f726520726563656e746c792077697468206465736b746f70207075626c697368696e6720736f667477617265206c696b6520416c64757320506167654d616b657220696e636c7564696e672076657273696f6e73206f66204c6f72656d20497073756d2e");       
        }
    }
}
