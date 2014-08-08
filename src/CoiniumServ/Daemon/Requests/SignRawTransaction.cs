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

namespace CoiniumServ.Daemon.Requests
{
    public class SignRawTransaction
    {
        /// <summary>
        /// Hexadecimal encoded version of the raw transaction to sign.
        /// </summary>
        public string RawTransactionHex { get; set; }

        /// <summary>
        /// A list of explicitly specified inputs to sign. This can be used
        /// if you do not want to sign all inputs in this transaction just yet.
        /// </summary>
        public List<SignRawTransactionInput> Inputs { get; set; }

        /// <summary>
        /// A list with the private keys needed to sign the transaction.
        /// There keys only have to be included if they are not in the wallet.
        /// </summary>
        public List<string> PrivateKeys { get; set; }

        public SignRawTransaction(string rawTransactionHex)
        {
            RawTransactionHex = rawTransactionHex;
            Inputs = new List<SignRawTransactionInput>();
            PrivateKeys = new List<string>();
        }

        public void AddInput(string transactionId, int output, string scriptPubKey)
        {
            Inputs.Add(new SignRawTransactionInput { TransactionId = transactionId, Output = output, ScriptPubKey = scriptPubKey });
        }

        public void AddKey(string privateKey)
        {
            PrivateKeys.Add(privateKey);
        }
    }
}
