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

namespace Coinium.Transactions
{
    /// <summary>
    /// Outpus for transaction.
    /// </summary>
    /// <remarks>
    /// Structure:  https://en.bitcoin.it/wiki/Protocol_specification#tx
    /// The output sets the conditions to release this bitcoin amount later. The sum of the output values of the 
    /// first transaction is the value of the mined bitcoins for the block plus possible transactions fees of the 
    /// other transactions in the block.
    /// </remarks>
    public class TxOut
    {
        /// <summary>
        /// Transaction Value
        /// </summary>
        public UInt64 Value { get; set; }

        /// <summary>
        /// Length of the pk_script
        /// </summary>
        public byte[] PublicKeyScriptLenght { get; set; }

        /// <summary>
        /// Usually contains the public key as a Bitcoin script setting up conditions to claim this output.
        /// </summary>
        public byte[] PublicKeyScript { get; set; }
    }
}
