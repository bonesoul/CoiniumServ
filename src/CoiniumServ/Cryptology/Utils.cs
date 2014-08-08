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
using System.Security.Cryptography;

namespace CoiniumServ.Cryptology
{
    public static class Utils
    {
        private static readonly SHA256Managed Sha256Managed;

        static Utils()
        {
            Sha256Managed = new SHA256Managed();
        }

        public static byte[] Digest(this byte[] input)
        {
            return Sha256Managed.ComputeHash(input, 0, input.Length);
        }

        /// <summary>
        /// Calculates the SHA-256 hash of the given byte range, and then hashes the resulting hash again. This is
        /// standard procedure in BitCoin. The resulting hash is in big endian form.
        /// </summary>
        public static byte[] DoubleDigest(this byte[] input)
        {
            var first = Sha256Managed.ComputeHash(input, 0, input.Length);
            return Sha256Managed.ComputeHash(first);
        }

        /// <summary>
        /// Calculates SHA256(SHA256(byte range 1 + byte range 2)).
        /// </summary>
        public static byte[] DoubleDigestTwoBuffers(byte[] input1, int offset1, int length1, byte[] input2, int offset2, int length2)
        {
            var buffer = new byte[length1 + length2];
            Array.Copy(input1, offset1, buffer, 0, length1);
            Array.Copy(input2, offset2, buffer, length1, length2);

            var first = Sha256Managed.ComputeHash(buffer, 0, buffer.Length);
            return Sha256Managed.ComputeHash(first);
        }
    }
}
