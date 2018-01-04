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
using System.Linq;
using System.Collections.Generic;
using CoiniumServ.Coin.Config;
using CoiniumServ.Utils.Helpers;
using CryptSharp.Utility;
using JsonConfig;

namespace CoiniumServ.Algorithms.Implementations
{
    public sealed class ScryptN : IHashAlgorithm
    {
        public uint Multiplier { get; private set; }

        /// <summary>
        /// R parameter - block size.
        /// </summary>
        private readonly int _r;

        /// <summary>
        /// P - parallelization parameter -  a large value of p can increase computational 
        /// cost of scrypt without increasing the memory usage.
        /// </summary>
        private readonly int _p;

        // default time-table for scrypt-n
        private readonly Dictionary<Int32, UInt64> _defaultTimeTable = new Dictionary<Int32, UInt64>
        {
            {2048, 1389306217},
            {4096, 1456415081},
            {8192, 1506746729},
            {16384, 1557078377},
            {32768, 1657741673},
            {65536, 1859068265},
            {131072, 2060394857},
            {262144, 1722307603},
            {524288, 1769642992},
        };

        private Dictionary<Int32, UInt64> _timeTable; // timeTable for the coin.

        public ScryptN(ICoinConfig coinConfig)
        {
            _r = 1;
            _p = 1;
            Multiplier = (UInt32)Math.Pow(2, 16);

            InitTimeTable(coinConfig.Extra.timeTable); // init the timeTable for the coin.
        }

        private void InitTimeTable(dynamic table)
        {
            _timeTable = new Dictionary<int, ulong>();

            if (table is NullExceptionPreventer) // if we are not provided a timeTable
                _timeTable = _defaultTimeTable; // use the default table.
            else
            {
                // else use the table provided by the coin configuration within extra.timeTable section.
                foreach (KeyValuePair<string, object> pair in table)
                {
                    _timeTable.Add(Int32.Parse(pair.Key), UInt64.Parse(pair.Value.ToString()));
                }
            }
        }

        public byte[] Hash(byte[] input)
        {
            var now = (UInt64)TimeHelpers.NowInUnixTimestamp();

            var index = _timeTable.OrderBy(x => x.Key).First(x => x.Value < now).Key;
            var nFactor = (int)(Math.Log(index) / Math.Log(2));
            var n = 1 << nFactor;

            return SCrypt.ComputeDerivedKey(input, input, n, _r, _p, null, 32);
        }
    }
}
