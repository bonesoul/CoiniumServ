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

using System.Text;

namespace CoiniumServ.Utils.Extensions
{
    /// <summary>
    /// Utility class to format a given JSON string so it is more readable and nicely indented.
    /// Original source from: http://stackoverflow.com/questions/4580397/json-formatter-in-c
    /// </summary>
    public static class JsonExtensions
    {
        public static string Indent = "    ";

        public static string PrettifyJson(this string input)
        {
            input = input.Replace('\"', '\'');
            var output = new StringBuilder(input.Length * 2);
            char? quote = null;
            int depth = 0;

            for (int i = 0; i < input.Length; ++i)
            {
                char ch = input[i];

                switch (ch)
                {
                    case '{':
                    case '[':
                        output.Append(ch);
                        if (!quote.HasValue)
                        {
                            output.AppendLine();
                            output.Append(Indent.Repeat(++depth));
                        }
                        break;
                    case '}':
                    case ']':
                        if (quote.HasValue)
                            output.Append(ch);
                        else
                        {
                            output.AppendLine();
                            output.Append(Indent.Repeat(--depth));
                            output.Append(ch);
                        }
                        break;
                    case '"':
                    case '\'':
                        output.Append(ch);
                        if (quote.HasValue)
                        {
                            if (!output.IsEscaped(i))
                                quote = null;
                        }
                        else quote = ch;
                        break;
                    case ',':
                        output.Append(ch);
                        if (!quote.HasValue)
                        {
                            output.AppendLine();
                            output.Append(Indent.Repeat(depth));
                        }
                        break;
                    case ':':
                        if (quote.HasValue) output.Append(ch);
                        else output.Append(" : ");
                        break;
                    default:
                        if (quote.HasValue || !char.IsWhiteSpace(ch))
                            output.Append(ch);
                        break;
                }
            }

            return output.ToString();
        }

        public static string Repeat(this string str, int count)
        {
            return new StringBuilder().Insert(0, str, count).ToString();
        }

        public static bool IsEscaped(this string str, int index)
        {
            bool escaped = false;
            while (index > 0 && str[--index] == '\\') escaped = !escaped;
            return escaped;
        }

        public static bool IsEscaped(this StringBuilder str, int index)
        {
            return str.ToString().IsEscaped(index);
        }
    }
}
