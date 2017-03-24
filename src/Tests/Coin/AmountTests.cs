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

using CoiniumServ.Utils.Helpers;
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
