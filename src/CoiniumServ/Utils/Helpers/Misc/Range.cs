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

using System.Collections;
using System.Collections.Generic;

namespace CoiniumServ.Utils.Helpers.Misc
{
    /// <summary>
    /// Port of python's range from
    /// </summary>
    /// <remarks>
    /// Ported from: http://stackoverflow.com/a/8273091
    /// </remarks>
    public class Range : IEnumerable<int>
    {
        private readonly int _start;
        private int _stop;
        private int _step = 1;

        public Range(int start)
        {
            _start = _stop = start;
        }

        public static Range From(int startRange)
        {
            return new Range(startRange);
        }

        public Range To(int endRange)
        {
            _stop = endRange;
            return this;
        }

        public Range WithStepSize(int step)
        {
            _step = step;
            return this;
        }

        public IEnumerator<int> GetEnumerator()
        {
            for (var i = _start; _step > 0 ? i < _stop : i > _stop; i += _step)
            {
                yield return i;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
