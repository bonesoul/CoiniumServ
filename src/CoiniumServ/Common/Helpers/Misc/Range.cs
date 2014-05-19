/*
 *   Coinium - Crypto Currency Pool Software - https://github.com/CoiniumServ/CoiniumServ
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
using System.Collections;
using System.Collections.Generic;

namespace Coinium.Common.Helpers.Misc
{
    /// <summary>
    /// Port of python's range from
    /// </summary>
    /// <remarks>
    /// Ported from: http://stackoverflow.com/a/8273091
    /// </remarks>
    public class Range : IEnumerable<int>
    {
        private int start = 0;
        private int stop = 0;
        private int step = 1;

        public Range(int start)
        {
            this.start = stop = start;
        }

        public static Range From(int startRange)
        {
            return new Range(startRange);
        }

        public Range To(int endRange)
        {
            stop = endRange;
            return this;
        }

        public Range WithStepSize(int step)
        {
            this.step = step;
            return this;
        }

        public IEnumerator<int> GetEnumerator()
        {
            for (var i = start; step > 0 ? i < stop : i > stop; i += step)
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
