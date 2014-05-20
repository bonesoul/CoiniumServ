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
using CryptSharp.Utility;

namespace Coinium.Core.Coin.Algorithms
{
    public class Scrypt : IHashAlgorithm
    {
        public UInt32 Multiplier { get; private set; }

        /// <summary>
        /// N parameter - CPU/memory cost parameter.
        /// </summary>
        public int N { get; private set; }

        /// <summary>
        /// R parameter - block size.
        /// </summary>
        public int R { get; private set; }

        /// <summary>
        /// P - parallelization parameter -  a large value of p can increase computational 
        /// cost of scrypt without increasing the memory usage.
        /// </summary>
        public int P { get; private set; }

        public Scrypt()
        {
            this.N = 1024;
            this.R = 1;
            this.P = 1;
            this.Multiplier = (UInt32) Math.Pow(2, 16);
        }

        public byte[] Hash(byte[] input)
        {
            var result = SCrypt.ComputeDerivedKey(input, input, this.N, this.R, this.P, null, 32);
            return result;
        }
    }
}
