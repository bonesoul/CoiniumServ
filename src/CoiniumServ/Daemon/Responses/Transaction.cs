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

            // kinda weird stuff goin here;
            // bitcoin variants;
            // case 1) some of bitcoin variants can include the "address" in the transaction detail and we can basically find the output comparing against it.
            // case 2) some other bitcoin variants can include "address account" name in transaction detail and we again find the output comparing against it.

            // check for case 1.
            if (Details.Any(x => x.Address == poolAddress))
                return Details.First(x => x.Address == poolAddress); // return the output that matches pool address.

            // check for case 2.
            if (Details.Any(x => x.Account == poolAccount))
                return Details.First(x => x.Account == poolAccount); // return the output that matches pool account.
            
            return null;
        }

        // not sure if fields below even exists / used
        //public double Fee { get; set; }
        //public string Comment { get; set; }
        //public string To { get; set; }        
    }
}
