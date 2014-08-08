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

using System.Collections.Generic;

namespace CoiniumServ.Daemon.Responses
{
    public class Transaction
    {
        public double Amount { get; set; }
        public int Confirmations { get; set; }

        public bool Generated { get; set; }

        public string BlockHash { get; set; }

        public int BlockIndex { get; set; }

        public int BlockTime { get; set; }

        public string TxId { get; set; }

        public string NormTxId { get; set; }

        public int Time { get; set; }
        public int TimeReceived { get; set; }

        public List<TransactionDetail> Details { get; set; }
        
        // not sure if fields below even exists / used
        //public double Fee { get; set; }
        //public string Comment { get; set; }
        //public string To { get; set; }        
    }
}
