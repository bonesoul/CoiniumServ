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
using CoiniumServ.Persistance.Blocks;

namespace CoiniumServ.Payments
{
    public class Payment : IPayment
    {
        public int Id { get; private set; }
        public int BlockId { get; private set; }
        public int AccountId { get; private set; }
        public decimal Amount { get; private set; }
        public bool Completed { get; set; }

        public Payment(IPersistedBlock block, int accountId, decimal amount)
        {
            BlockId = (int)block.Height;
            AccountId = accountId;
            Amount = amount;
            Completed = false;
        }

        public Payment(Int32 id, Int32 block, Int32 accountId, Decimal amount, bool completed)
        {
            Id = id;
            BlockId = block;
            AccountId = accountId;
            Amount = amount;
            Completed = completed;
        }
    }
}
