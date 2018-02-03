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
using System.Linq;
using System.Text;
using CoiniumServ.Coin.Address.Exceptions;
using CoiniumServ.Cryptology;
using Org.BouncyCastle.Math;

namespace CoiniumServ.Coin.Address
{

    /// <summary>
    /// Base58 encoder & decoder
    /// </summary>
    /// <specification>https://en.bitcoin.it/wiki/Base58Check_encoding</specification>
    /// <remarks>
    /// A custom form of base58 is used to encode BitCoin addresses. Note that this is not the same 
    /// base58 as used by Flickr, which you may see reference to around the internet.
    /// Satoshi says: why base-58 instead of standard base-64 encoding?
    /// * Don't want 0OIl characters that look the same in some fonts and could be used to create visually identical looking account numbers.
    /// * A string with non-alphanumeric characters is not as easily accepted as an account number.
    /// * E-mail usually won't line-break if there's no punctuation to break at.
    /// * Doubleclicking selects the whole number as one word if it's all alphanumeric.
    /// Original implementation: https://github.com/CoiniumServ/BitcoinSharp/blob/55ca27107d200ede9896c1064de76b04d4daf9ef/src/Core/Base58.cs
    /// </remarks>
    public class Base58
    {
        private const string Alphabet = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
        private static readonly BigInteger Base = BigInteger.ValueOf(58);

        public static string Encode(byte[] input)
        {
            var bi = new BigInteger(1, input);
            var s = new StringBuilder();
            while (bi.CompareTo(Base) >= 0)
            {
                var mod = bi.Mod(Base);
                s.Insert(0, new[] {Alphabet[mod.IntValue]});
                bi = bi.Subtract(mod).Divide(Base);
            }
            s.Insert(0, new[] {Alphabet[bi.IntValue]});
            // Convert leading zeros too.
            foreach (var anInput in input)
            {
                if (anInput == 0)
                    s.Insert(0, new[] {Alphabet[0]});
                else
                    break;
            }
            return s.ToString();
        }

        /// <exception cref="AddressFormatException"/>
        public static byte[] Decode(string input)
        {
            var bytes = DecodeToBigInteger(input).ToByteArray();
            // We may have got one more byte than we wanted, if the high bit of the next-to-last byte was not zero. This
            // is because BigIntegers are represented with twos-compliment notation, thus if the high bit of the last
            // byte happens to be 1 another 8 zero bits will be added to ensure the number parses as positive. Detect
            // that case here and chop it off.
            var stripSignByte = bytes.Length > 1 && bytes[0] == 0 && bytes[1] >= 0x80;
            // Count the leading zeros, if any.
            var leadingZeros = 0;
            for (var i = 0; input[i] == Alphabet[0]; i++)
            {
                leadingZeros++;
            }
            // Now cut/pad correctly. Java 6 has a convenience for this, but Android can't use it.
            var tmp = new byte[bytes.Length - (stripSignByte ? 1 : 0) + leadingZeros];
            Array.Copy(bytes, stripSignByte ? 1 : 0, tmp, leadingZeros, tmp.Length - leadingZeros);
            return tmp;
        }

        /// <exception cref="AddressFormatException"/>
        public static BigInteger DecodeToBigInteger(string input)
        {
            var bi = BigInteger.ValueOf(0);
            // Work backwards through the string.
            for (var i = input.Length - 1; i >= 0; i--)
            {
                var alphaIndex = Alphabet.IndexOf(input[i]);
                if (alphaIndex == -1)
                {
                    throw new AddressFormatException("Illegal character " + input[i] + " at " + i);
                }
                bi = bi.Add(BigInteger.ValueOf(alphaIndex).Multiply(Base.Pow(input.Length - 1 - i)));
            }
            return bi;
        }

        /// <summary>
        /// Uses the checksum in the last 4 bytes of the decoded data to verify the rest are correct. The checksum is
        /// removed from the returned data.
        /// </summary>
        /// <exception cref="AddressFormatException">If the input is not base 58 or the checksum does not validate.</exception>
        public static byte[] DecodeChecked(string input)
        {
            var tmp = Decode(input);
            if (tmp.Length < 4)
                throw new AddressFormatException("Input too short");
            var checksum = new byte[4];
            Array.Copy(tmp, tmp.Length - 4, checksum, 0, 4);
            var bytes = new byte[tmp.Length - 4];
            Array.Copy(tmp, 0, bytes, 0, tmp.Length - 4);
            tmp = bytes.DoubleDigest();
            var hash = new byte[4];
            Array.Copy(tmp, 0, hash, 0, 4);
            if (!hash.SequenceEqual(checksum))
                throw new AddressFormatException("Checksum does not validate");
            return bytes;
        }
    }
}
