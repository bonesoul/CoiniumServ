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

namespace CoiniumServ.Payments
{
    public class PaymentDetails:IPaymentDetails
    {
        public int PaymentId { get; set; }
        public int TransactionId { get; set; }
        public int AccountId { get; set; }
        public string Address { get; set; }
        public int Block { get; set; }
        public decimal Amount { get; set; }
        public decimal SentAmount { get; set; }
        public string Currency { get; set; }
        public string TxHash { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime TransactionDate { get; set; }
        public bool Completed { get; set; }
    }
}
