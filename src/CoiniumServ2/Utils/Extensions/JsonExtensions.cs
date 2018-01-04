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
            try
            {
                input = input.Replace('\"', '\'');
                var output = new StringBuilder(input.Length*2);
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
            catch (Exception)
            {
                return input; // in case we fail to prettify json, handle the exception and just return the input.
            }
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
