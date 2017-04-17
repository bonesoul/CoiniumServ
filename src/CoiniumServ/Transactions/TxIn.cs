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

using System;
using CoiniumServ.Transactions.Script;

namespace CoiniumServ.Transactions
{
    /// <summary>
    /// Inputs for transaction.
    /// </summary>
    /// <remarks>
    /// Structure:  https://en.bitcoin.it/wiki/Protocol_specification#tx
    /// Information: http://bitcoin.stackexchange.com/a/20725/8899
    /// The input sufficiently describes where and how to get the bitcoin amout to be redeemed. If it is the (only) input of the first transaction 
    /// of a block, it is called the generation transaction input and its content completely ignored. (Historically the Previous Transaction hash is 0 
    /// and the Previous Txout-index is -1.)
    /// </remarks>
    public class TxIn
    {
        /// <summary>
        /// The previous output transaction reference, as an OutPoint structure
        /// </summary>
        public OutPoint PreviousOutput { get; set; }

        /// <summary>
        /// Computational Script for confirming transaction authorization
        /// </summary>
        public ISignatureScript SignatureScript { get; set; }

        /// <summary>
        /// Transaction version as defined by the sender. Intended for "replacement" of transactions when information is updated before inclusion into a block.
        /// </summary>
        public UInt32 Sequence { get; set; }
    }

}
