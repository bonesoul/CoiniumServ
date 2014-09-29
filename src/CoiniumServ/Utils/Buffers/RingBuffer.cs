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
