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

namespace CoiniumServ.Tests.Coin.Coinbase
{
    public class UtilsTests
    {
        /// <summary>
        /// Tests conversion of wallet address to coinbase script with a valid wallet address input.
        /// </summary>
        [Fact]
        public void WalletAddressToScriotTest_WithValidInput_ShouldSuccess()
        {
            const string input = "F959TJaFMXCqbj9nKcGGy5JemeQ3pM4y5S";
            const string expected = "76a914238fdcf8c710c698a1f2c32d378da10fcab1082888ac";

            var result = CoiniumServ.Coin.Coinbase.Utils.CoinAddressToScript(input);
            result.ToHexString().Should().Equal(expected);
        }

        /// <summary>
        /// Tests conversion of wallet address to coinbase script with a valid wallet address input.
        /// </summary>
        [Fact]
        public void WalletAddressToScriotTest_WithInvalidInput_ShouldReturnNull()
        {
            const string input = "invalid";
            byte[] expected = null;

            var result = CoiniumServ.Coin.Coinbase.Utils.CoinAddressToScript(input);
            result.Should().Equal(expected);
        }

        /// <summary>
        /// Tests conversion of pubkey to coinbase script with a valid PubKey input.
        /// Test data from: http://runnable.com/me/VCFHE0RrZnwbsQ6y
        /// </summary>
        [Fact]
        public void PubKeyToScriptToScriptTest_WithValidInput_ShouldSuccess()
        {
            const string input = "03679839c140a5d48b19f8d3b7c799d7534c5716d7131c5d36ec26422ba52bd378";
            const string expected = "2103679839c140a5d48b19f8d3b7c799d7534c5716d7131c5d36ec26422ba52bd378ac";

            var result = CoiniumServ.Coin.Coinbase.Utils.PubKeyToScript(input);
            result.ToHexString().Should().Equal(expected);
        }

        /// <summary>
        /// Tests conversion of pubkey to coinbase script with a invalid PubKey input.
        /// Test data from: http://runnable.com/me/VCFHE0RrZnwbsQ6y
        /// </summary>
        [Fact]
        public void PubKeyToScriptToScriptTest_WithInvalidInput_ShouldReturnNull()
        {
            const string input = "00AABBCCDDEEFF"; // invalid data as input PubKey's need to a exactly 33 bytes long.
            byte[] expected = null;

            var result = CoiniumServ.Coin.Coinbase.Utils.PubKeyToScript(input);
            result.Should().Equal(expected);
        }
    }
}
