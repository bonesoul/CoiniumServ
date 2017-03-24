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
using System.Diagnostics;
using System.Linq;
using CoiniumServ.Utils.Extensions;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities.Encoders;

// originally from: https://code.google.com/p/bitcoinsharp/source/browse/src/Core/Sha256Hash.cs

namespace CoiniumServ.Cryptology
{
    /// <summary>
    /// A Hash just wraps a byte[] so that equals and hashcode work correctly, allowing it to be used as keys in a
    /// map. It also checks that the length is correct and provides a bit more type safety.
    /// </summary>
    [Serializable]
    public class Hash
    {
        private readonly byte[] _bytes;

        public static readonly Hash ZeroHash = new Hash(new byte[32]);

        /// <summary>
        /// Creates a Hash by wrapping the given byte array. It must be 32 bytes long.
        /// </summary>
        public Hash(byte[] bytes)
        {
            if(bytes.Length != 32)
                throw new ArgumentOutOfRangeException();

            _bytes = bytes;
        }

        /// <summary>
        /// Creates a Hash by decoding the given hex string. It must be 64 characters long.
        /// </summary>
        public Hash(string @string)
        {
            Debug.Assert(@string.Length == 64);
            _bytes = Hex.Decode(@string);
        }

        /// <summary>
        /// Returns true if the hashes are equal.
        /// </summary>
        public override bool Equals(object other)
        {
            if (!(other is Hash)) return false;
            return _bytes.SequenceEqual(((Hash)other)._bytes);
        }

        /// <summary>
        /// Hash code of the byte array as calculated by <see cref="object.GetHashCode"/>. Note the difference between a Hash
        /// secure bytes and the type of quick/dirty bytes used by the Java hashCode method which is designed for use in
        /// bytes tables.
        /// </summary>
        public override int GetHashCode()
        {
            return _bytes != null ? _bytes.Aggregate(1, (current, element) => 31 * current + element) : 0;
        }

        public override string ToString()
        {
            return _bytes.ToHexString();
        }

        /// <summary>
        /// Returns the bytes interpreted as a positive integer.
        /// </summary>
        public BigInteger ToBigInteger()
        {
            return new BigInteger(1, _bytes);
        }

        public byte[] Bytes
        {
            get { return _bytes; }
        }

        public Hash Duplicate()
        {
            return new Hash(_bytes);
        }

        public static implicit operator byte[](Hash hash)
        {
            return hash.Bytes;
        }

        public static implicit operator Hash(byte[] bytes)
        {
            return new Hash(bytes);
        }
    }
}
