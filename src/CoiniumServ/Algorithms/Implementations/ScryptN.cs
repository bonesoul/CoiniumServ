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
using System.Linq;
using System.Collections.Generic;
using CoiniumServ.Utils.Helpers;
using CryptSharp.Utility;
using JsonConfig;

namespace CoiniumServ.Algorithms.Implementations
{
    public sealed class ScryptN : HashAlgorithmBase
    {
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

        public ScryptN()
        {
            // we don't set N value here on initialization because N is computed dynamically for scrypt-n coins.
            _r = 1;
            _p = 1;

            Multiplier = (UInt32)Math.Pow(2, 16);
        }

        public override byte[] Hash(byte[] input, dynamic config)
        {
            var timeTable = new Dictionary<Int32, UInt64>();
            var now = (UInt64)TimeHelpers.NowInUnixTimestamp();

            if (config.timeTable is NullExceptionPreventer) // if we are not provided a timeTable
                timeTable = _defaultTimeTable; // use the default table.
            else
            {
                foreach (KeyValuePair<string, object> pair in config.timeTable)
                {
                    timeTable.Add(Int32.Parse(pair.Key), UInt64.Parse(pair.Value.ToString()));
                }
            }

            var n = timeTable.OrderBy(x => x.Key).First(x => x.Value < now).Key;
            var nFactor = (int) (Math.Log(n)/Math.Log(2));

            return SCrypt.ComputeDerivedKey(input, input, nFactor, _r, _p, null, 32);
        }
    }
}
