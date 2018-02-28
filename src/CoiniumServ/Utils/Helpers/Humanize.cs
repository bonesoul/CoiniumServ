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
        public static string GetReadableHashrate(this double hashrate)
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
