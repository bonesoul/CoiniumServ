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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Gibbed.IO;

namespace Coinium.Common.Extensions
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

        public static byte[] HexToByteArray(this string str)
        {
            str = str.Replace(" ", String.Empty);

            var res = new byte[str.Length / 2];
            for (int i = 0; i < res.Length; ++i)
            {
                string temp = String.Concat(str[i * 2], str[i * 2 + 1]);
                res[i] = Convert.ToByte(temp, 16);
            }
            return res;
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
