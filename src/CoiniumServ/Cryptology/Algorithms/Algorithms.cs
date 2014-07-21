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

using System.Globalization;
using CoiniumServ.Utils.Numerics;

namespace CoiniumServ.Cryptology.Algorithms
{
    public static class Algorithms
    {
        public const string Blake = "blake";
        public const string Fugue = "fugue";
        public const string Groestl = "groestl";
        public const string Keccak = "keccak";
        public const string Scrypt = "scrypt";
        public const string Sha256 = "sha256";
        public const string Shavite3 = "shavite3";
        public const string Skein = "skein";
        public const string X11 = "X11";
        public const string X13 = "X13";
        public const string X15 = "X15";
        public const string X17 = "X17";

        // todo: add hefty1, qubit support

        /// <summary>
        /// Global diff1
        /// </summary>
        public static BigInteger Diff1 { get; private set; }

        static Algorithms()
        {
            Diff1 = BigInteger.Parse("00000000ffff0000000000000000000000000000000000000000000000000000", NumberStyles.HexNumber);                                    
        }
    }
}
