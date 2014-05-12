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

namespace Coinium.Core.Coin
{
    public class GenerationTransaction
    {
        public int txInputsCount { get; private set; }

        public int txOutputsCount { get; private set; }

        public int txVersion { get; private set; }

        public int txInPrevOutHash { get; private set; }

        public UInt32 txInPrevOutIndex { get; private set; }

        public string txMessage { get; private set; }

        public GenerationTransaction(bool supportTxMessages = false)
        {
            this.txInputsCount = 1;
            this.txOutputsCount = 1;
            this.txVersion = supportTxMessages == true ? 2 : 1;
            this.txInPrevOutHash = 0;
            this.txInPrevOutIndex = ((UInt32) Math.Pow(2, 32) - 1);

            this.txMessage = "https://github.com/CoiniumServ/CoiniumServ";
        }
    }
}
