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
using CryptSharp.Utility;

namespace CoiniumServ.Algorithms.Implementations
{
    public sealed class ScryptOg : IHashAlgorithm
    {
        public uint Multiplier { get; private set; }

        /// <summary>
        /// N parameter - CPU/memory cost parameter.
        /// </summary>
        private readonly int _n;

        /// <summary>
        /// R parameter - block size.
        /// </summary>
        private readonly int _r;

        /// <summary>
        /// P - parallelization parameter -  a large value of p can increase computational 
        /// cost of scrypt without increasing the memory usage.
        /// </summary>
        private readonly int _p;

        public ScryptOg()
        {
            _n = 64;
            _r = 1;
            _p = 1;

            Multiplier = (UInt32)Math.Pow(2, 16);
        }

        public byte[] Hash(byte[] input)
        {
            return SCrypt.ComputeDerivedKey(input, input, _n, _r, _p, null, 32);
        }
    }
}
