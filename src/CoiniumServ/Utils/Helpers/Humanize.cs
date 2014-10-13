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
using System.Globalization;

namespace CoiniumServ.Utils.Helpers
{
    public static class Humanize
    {

        /// <summary>
        /// Returns given hashrate value as human readable string.
        /// </summary>
        /// <param name="hashrate"></param>
        /// <returns></returns>
        public static string GetReadableHashrate(this UInt64 hashrate)
        {
            var index = -1;
            double rate = hashrate;

            var units = new[] {"KH/s", "MH/s", "GH/s", "TH/s", "PH/s", "EH/s", "ZH/s", "YH/s"};

            do
            {
                rate = rate/1000;
                index++;
            } while (rate > 1000);

            return string.Format("{0:0.00} {1}", rate, units[index]);
        }

        /// <summary>
        /// Returns given difficulty value as human readable string.
        /// </summary>
        /// <param name="difficulty"></param>
        /// <returns></returns>
        public static string GetReadableDifficulty(this double difficulty)
        {
            var index = -1;
            var rate = difficulty;

            var units = new[] {"K", "M", "B", "T", "Q"};

            if (difficulty < 1000)
                return difficulty.ToString(CultureInfo.InvariantCulture);

            do
            {
                rate = rate/1000;
                index++;
            } while (rate > 1000);

            return string.Format("{0:0.00} {1}", rate, units[index]);
        }
    }
}
