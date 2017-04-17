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
