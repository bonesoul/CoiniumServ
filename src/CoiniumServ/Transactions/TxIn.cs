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

using System;
using Coinium.Transactions.Script;

namespace Coinium.Transactions
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
