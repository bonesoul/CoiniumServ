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
