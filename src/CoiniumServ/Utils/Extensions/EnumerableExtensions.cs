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