/*
 *   Coinium project - crypto currency pool software - https://github.com/raistlinthewiz/coinium
 *   Copyright (C) 2013 Hüseyin Uslu, Int6 Studios - http://www.coinium.org
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
using System.Linq;

namespace Coinium.Common.Extensions
{
    public static class ArrayExtensions
    {
        public static IEnumerable<T> EnumerateFrom<T>(this T[] array, int start)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            return Enumerate<T>(array, start, array.Length);
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

        public static string ToHexString(this byte[] byteArray)
        {
            return byteArray.Aggregate("", (current, b) => current + b.ToString("X2"));
        }

        public static string ToFormatedHexString(this byte[] byteArray)
        {
            return byteArray.Aggregate("", (current, b) => current + " " + b.ToString("X2"));
        }

        public static byte[] ToByteArray(this string str)
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
    }
}
