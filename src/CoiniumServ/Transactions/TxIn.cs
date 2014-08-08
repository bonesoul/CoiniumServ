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
