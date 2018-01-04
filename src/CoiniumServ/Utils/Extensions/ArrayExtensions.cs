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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Gibbed.IO;

namespace CoiniumServ.Utils.Extensions
{
    public static class ArrayExtensions
    {
        public static IEnumerable<T> EnumerateFrom<T>(this T[] array, int start)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            return Enumerate(array, start, array.Length);
        }

        public static IEnumerable<T> Enumerate<T>(this T[] array, int start, int count)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            for (int i = 0; i < count; i++)
                yield return array[start + i];
        }

        public static byte[] Append(this byte[] a, byte[] b)
        {
            var result = new byte[a.Length + b.Length];

            a.CopyTo(result, 0);
            b.CopyTo(result, a.Length);

            return result;
        }

        public static bool CompareTo(this byte[] byteArray, byte[] second)
        {
            if (byteArray.Length != second.Length)
                return false;

            return !byteArray.Where((t, i) => second[i] != t).Any();
        }

        public static string Dump(this byte[] array)
        {
            return EnumerableExtensions.Dump(array);
        }

        public static string ToHexString(this IEnumerable<byte> byteArray)
        {
            return ToHexString(byteArray.ToArray());
        }

        public static string ToHexString(this byte[] byteArray)
        {
            return byteArray.Aggregate("", (current, b) => current + b.ToString("x2"));
        }

        public static string ToFormatedHexString(this byte[] byteArray)
        {
            return byteArray.Aggregate("", (current, b) => current + " 0x" + b.ToString("x2"));
        }

        /// <summary>
        /// Returns a copy of the given byte array in reverse order.
        /// </summary>
        public static byte[] ReverseBytes(this byte[] bytes)
        {
            var reversed = new byte[bytes.Length];
            for (var i = bytes.Length - 1; i >= 0; i--)
                reversed[bytes.Length - i - 1] = bytes[i];
            return reversed;
        }

        /// <summary>
        /// Reverses the byte order.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static byte[] ReverseByteOrder(this byte[] bytes)
        {
            byte[] result;

            using (var stream = new MemoryStream())
            {
                for (var i = 0; i < 8; i++)
                {
                    var value = BitConverter.ToUInt32(bytes, i*4).BigEndian();
                    stream.WriteValueU32(value);
                }

                result = stream.ToArray();
                
            }

            return result.ReverseBuffer();
        }

        public static byte[] ReverseBuffer(this byte[] bytes)
        {
            byte[] result;

            using (var stream = new MemoryStream())
            {
                for (var i = bytes.Length -1 ; i >= 0; i--)
                {
                    stream.WriteByte(bytes[i]);
                }

                result = stream.ToArray();
            }

            return result;
        }

        /// <summary>
        /// Get the array slice between the two indexes.
        /// ... Inclusive for start index, exclusive for end index.
        /// </summary>
        public static T[] Slice<T>(this T[] source, int start, int end)
        {
            // Handles negative ends.
            if (end < 0)
            {
                end = source.Length + end;
            }
            int len = end - start;

            // Return new array.
            var res = new T[len];
            for (int i = 0; i < len; i++)
            {
                res[i] = source[i + start];
            }
            return res;
        }
    }
}
