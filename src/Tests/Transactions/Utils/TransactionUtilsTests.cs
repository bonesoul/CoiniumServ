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
using CoiniumServ.Transactions.Utils;
using CoiniumServ.Utils.Extensions;
using Should.Fluent;
using Xunit;

namespace CoiniumServ.Tests.Transactions.Utils
{
    public class TransactionUtilsTests
    {
        const long UnixDateTime = 1402265775319;
        const long FinalUnixDateTime = UnixDateTime / 1000 | 0;

        /// <summary>
        /// -> date: 1402265775319 final:1402265775 serialized: 04afe09453
        /// </summary>
        [Fact]
        public void SerializedUnixDateTimeTest()
        {
            FinalUnixDateTime.Should().Equal((Int64)1402265775);

            var serializedUnixTime = TransactionUtils.GetSerializedUnixDateTime(UnixDateTime);
            serializedUnixTime.ToHexString().Should().Equal("04afe09453");
        }
    }
}
