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

using System.Collections;
using System.Collections.Generic;

namespace CoiniumServ.Utils.Helpers
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
