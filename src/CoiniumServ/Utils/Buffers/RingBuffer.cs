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

using System.Linq;

namespace CoiniumServ.Utils.Buffers
{
    public class RingBuffer:IRingBuffer
    {
        public int Size { get { return _isFull ? Capacity : _cursor; } }

        public float Average { get { return (float)_buffer.Sum() / (float)(_isFull ? Capacity : _cursor); } }

        private readonly int[] _buffer;

        private int _cursor;

        private bool _isFull;
        private int Capacity { get { return _buffer.Length; } }

        public RingBuffer(int capacity)
        {
            _buffer = new int[capacity];
            _cursor = 0;
            _isFull = false;
        }

        public void Clear()
        {
            for (int i = 0; i < Capacity; i++)
                _buffer[i] = default(int);

            _cursor = 0;
            _isFull = false;
        }

        public void Append(int item)
        {
            _buffer[_cursor] = item;

            if (_isFull)
            {                
                _cursor = (_cursor + 1) % Capacity;
            }
            else
            {
                _cursor++;

                if (_cursor >= Capacity)
                {
                    _cursor = 0;
                    _isFull = true;
                }
            }
        }
    }
}
