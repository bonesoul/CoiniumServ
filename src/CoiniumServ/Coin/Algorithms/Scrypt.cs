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
using CryptSharp.Utility;
using Org.BouncyCastle.Math;

namespace Coinium.Coin.Algorithms
{
    public class Scrypt : IHashAlgorithm
    {
        /// <summary>
        /// Gets the multiplier.
        /// </summary>
        /// <value>
        /// The multiplier.
        /// </value>
        public UInt32 Multiplier { get; private set; }

        /// <summary>
        /// Gets the difficulty.
        /// </summary>
        /// <value>
        /// The difficulty.
        /// </value>
        public BigInteger Difficulty { get; private set; }

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
            N = 1024;
            R = 1;
            P = 1;
            Multiplier = (UInt32) Math.Pow(2, 16);

            Difficulty = new BigInteger("00000000ffff0000000000000000000000000000000000000000000000000000", 16);
        }

        public byte[] Hash(byte[] input)
        {
            var result = SCrypt.ComputeDerivedKey(input, input, N, R, P, null, 32);
            return result;
        }
    }
}
