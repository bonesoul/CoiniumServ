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
using System.Linq;
using System.Text;

namespace CoiniumServ.Utils.Extensions
{
    public static class EnumerableExtensions
    {
        public static string HexDump(this IEnumerable<byte> collection)
        {
            var sb = new StringBuilder();
            foreach (byte value in collection)
            {
                sb.Append(value.ToString("X2"));
                sb.Append(' ');
            }
            if (sb.Length > 0)
                sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        public static string ToEncodedString(this IEnumerable<byte> collection)
        {
            return ToEncodedString(collection, Encoding.UTF8);
        }

        public static string ToEncodedString(this IEnumerable<byte> collection, Encoding encoding)
        {
            return encoding.GetString(collection.ToArray());
        }

        public static string Dump(this IEnumerable<byte> collection)
        {
            var output = new StringBuilder();
            var hex = new StringBuilder();
            var text = new StringBuilder();
            int i = 0;
            foreach (byte value in collection)
            {
                if (i > 0 && ((i % 16) == 0))
                {
                    output.Append(hex);
                    output.Append(' ');
                    output.Append(text);
                    output.Append(Environment.NewLine);
                    hex.Clear(); text.Clear();
                }
                hex.Append(value.ToString("X2"));
                hex.Append(' ');
                text.Append(string.Format("{0}", (char.IsWhiteSpace((char)value) && (char)value != ' ') ? '.' : (char)value)); // prettify text
                ++i;
            }
            var hexstring = hex.ToString();
            if (text.Length < 16)
            {
                hexstring = hexstring.PadRight(48); // pad the hex representation in-case it's smaller than a regular 16 value line.
            }
            output.Append(hexstring);
            output.Append(' ');
            output.Append(text);
            return output.ToString();
        }
    }
}