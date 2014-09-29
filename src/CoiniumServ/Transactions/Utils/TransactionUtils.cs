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
using System.Collections.Generic;
using System.Linq;
using CoiniumServ.Coin.Coinbase;
using CoiniumServ.Daemon.Responses;
using CoiniumServ.Utils.Extensions;

namespace CoiniumServ.Transactions.Utils
{
    public static class TransactionUtils
    {
        public static byte[] GetSerializedUnixDateTime(Int64 unixDateTime)
        {
            var dateTime = unixDateTime / 1000 | 0;
            return Serializers.SerializeNumber(dateTime);
        }

        public static List<byte[]> GetHashList(this BlockTemplateTransaction[] transactions)
        {
            return transactions.Select(transaction => transaction.Hash.HexToByteArray().ReverseBuffer()).ToList();
        }
    }
}
