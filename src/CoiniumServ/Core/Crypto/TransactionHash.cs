/*
 *   Coinium - Crypto Currency Pool Software - https://github.com/CoiniumServ/CoiniumServ
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

// originally from: https://code.google.com/p/bitcoinsharp/source/browse/src/Core/Sha256Hash.cs
using System;
using System.Diagnostics;
using System.Linq;
using Coinium.Common.Extensions;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities.Encoders;

namespace Coinium.Core.Crypto
{
    /// <summary>
    /// A TransactionHash just wraps a byte[] so that equals and hashcode work correctly, allowing it to be used as keys in a
    /// map. It also checks that the length is correct and provides a bit more type safety.
    /// </summary>
    [Serializable]
    public class TransactionHash
    {
        private readonly byte[] _bytes;

        public static readonly TransactionHash ZeroHash = new TransactionHash(new byte[32]);

        /// <summary>
        /// Creates a TransactionHash by wrapping the given byte array. It must be 32 bytes long.
        /// </summary>
        public TransactionHash(byte[] bytes)
        {
            if(bytes.Length != 32)
                throw new ArgumentOutOfRangeException();

            _bytes = bytes;
        }

        /// <summary>
        /// Creates a TransactionHash by decoding the given hex string. It must be 64 characters long.
        /// </summary>
        public TransactionHash(string @string)
        {
            Debug.Assert(@string.Length == 64);
            _bytes = Hex.Decode(@string);
        }

        /// <summary>
        /// Returns true if the hashes are equal.
        /// </summary>
        public override bool Equals(object other)
        {
            if (!(other is TransactionHash)) return false;
            return _bytes.SequenceEqual(((TransactionHash)other)._bytes);
        }

        /// <summary>
        /// Hash code of the byte array as calculated by <see cref="object.GetHashCode"/>. Note the difference between a TransactionHash
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

        public TransactionHash Duplicate()
        {
            return new TransactionHash(_bytes);
        }
    }
}
