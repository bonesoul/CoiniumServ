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
