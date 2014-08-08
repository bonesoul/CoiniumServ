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

using CoiniumServ.Coin.Helpers;
using Should.Fluent;
using Xunit;

namespace CoiniumServ.Tests.Coin
{
    public class AmountTests
    {
        /// <summary>
        /// Tests <see cref="Amount"/>.
        /// </summary>
        [Fact]
        public void ConversionTests()
        {
            const double coins = 7.05132704;
            const double millibits = 7051.32704;            
            const double microbits = 7051327.04;
            const int satoshis = 705132704;

            // set new instances using different methods.
            var amount1 = new Amount() {Coins = coins};
            var amount2 = new Amount() { MilliBits = millibits };
            var amount3 = new Amount() { MicroBits = microbits };
            var amount4 = new Amount() { Satoshis = satoshis };

            // test all instances against each other.

            amount1.Coins.Should().Equal(amount2.Coins);
            amount1.MilliBits.Should().Equal(amount2.MilliBits);
            amount1.MicroBits.Should().Equal(amount2.MicroBits);
            amount1.Satoshis.Should().Equal(amount2.Satoshis);

            amount2.Coins.Should().Equal(amount3.Coins);
            amount2.MilliBits.Should().Equal(amount3.MilliBits);
            amount2.MicroBits.Should().Equal(amount3.MicroBits);
            amount2.Satoshis.Should().Equal(amount3.Satoshis);

            amount3.Coins.Should().Equal(amount4.Coins);
            amount3.MilliBits.Should().Equal(amount4.MilliBits);
            amount3.MicroBits.Should().Equal(amount4.MicroBits);
            amount3.Satoshis.Should().Equal(amount4.Satoshis);
        }
    }
}
