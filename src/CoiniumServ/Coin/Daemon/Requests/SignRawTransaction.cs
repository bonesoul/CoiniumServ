/*
 *   CoiniumServ - crypto currency pool software - https://github.com/CoiniumServ/CoiniumServ
 *   Copyright (C) 2013 - 2014, Coinium Project - http://www.coinium.org
 *
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

/* This file is based on https://github.com/BitKoot/BitcoinRpcSharp */

using System.Collections.Generic;

namespace Coinium.Coin.Daemon.Requests
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
