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
using System.Linq;

namespace CoiniumServ.Daemon.Responses
{
    public class Transaction
    {
        public double Amount { get; set; } // seems it's set to 0 for immature transactions or generation transactions.

        /// <summary>
        /// As Amount may not always return the total transaction amount, TotalAmount calculates and return the value using transaction details
        /// </summary>
        public double TotalAmount { get { return Details.Sum(item => item.Amount); } }

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


        /// <summary>
        /// Returns the transaction detail that contains the output for pool.
        /// </summary>
        /// <param name="poolAddress"></param>
        /// <param name="poolAccount"></param>
        /// <returns></returns>
        public TransactionDetail GetPoolOutput(string poolAddress, string poolAccount)
        {
            if (Details == null) // make sure we have valid outputs.
                return null;

            // check if coin includes output address data in transaction details.
            return Details.Any(x => x.Address == null)
                ? Details.FirstOrDefault(x => x.Account == poolAccount) // some coins doesn't include address field in outputs, so try to determine using the associated account name.
                : Details.FirstOrDefault(x => x.Address == poolAddress); // if coin includes address field in outputs, just use it.
        }

        // not sure if fields below even exists / used
        //public double Fee { get; set; }
        //public string Comment { get; set; }
        //public string To { get; set; }        
    }
}
