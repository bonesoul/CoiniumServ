/*
 *   Coinium - Crypto Currency Pool Software - https://github.com/CoiniumServ/CoiniumServ
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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Coinium.Core.Crypto;

namespace Coinium.Core.Coin
{
    public class GenerationTransaction
    {
        /// <summary>
        /// Transaction data format version
        /// </summary>
        public UInt32 txVersion { get; private set; }

        /// <summary>
        /// Number of Transaction inputs
        /// </summary>
        public UInt32 txInputsCount { get; private set; }

        // Number of Transaction outputs
        public UInt32 txOutputsCount { get; private set; }

        /// <summary>
        /// The hash of the referenced transaction - as we creating a generation transaction - none.
        /// </summary>
        public Sha256Hash txInPrevOutHash { get; private set; }

        /// <summary>
        /// The index of the specific output in the transaction. The first output is 0, etc.
        /// </summary>
        public UInt32 txInPrevOutIndex { get; private set; }

        /// <summary>
        ///  For coins that support/require transaction comments
        /// </summary>
        public string txMessage { get; private set; }

        /// <summary>
        /// The block number or timestamp at which this transaction is locked:
        /// 0 	Always locked
        /// < 500000000 	Block number at which this transaction is locked
        /// >= 500000000 	UNIX timestamp at which this transaction is locked
        /// </summary>
        public UInt32 txLockTime { get; private set; }


        public GenerationTransaction(bool supportTxMessages = false)
        {
            this.txInputsCount = 1;
            this.txOutputsCount = 1;
            this.txVersion = (UInt32)(supportTxMessages ? 2 : 1);
            this.txInPrevOutHash = new Sha256Hash(new byte[] {0x0});
            this.txInPrevOutIndex = ((UInt32) Math.Pow(2, 32) - 1);

            this.txMessage = "https://github.com/CoiniumServ/CoiniumServ";
        }
    }
}
