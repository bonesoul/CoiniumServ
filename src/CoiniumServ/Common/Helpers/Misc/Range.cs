/*
 *   CoiniumServ - crypto currency pool software - https://github.com/CoiniumServ/CoiniumServ
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
