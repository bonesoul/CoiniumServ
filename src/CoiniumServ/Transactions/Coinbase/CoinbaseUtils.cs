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
using System.IO;
using System.Text;
using Coinium.Coin.Address;
using Coinium.Common.Extensions;
using Coinium.Common.Helpers.Arrays;
using Coinium.Crypto;
using Gibbed.IO;
using Serilog;

namespace Coinium.Transactions.Coinbase
{
    /// <summary>
    /// Provides helper functions for "serialized CSscript formatting" as defined here: https://github.com/bitcoin/bips/blob/master/bip-0034.mediawiki#specification
    /// </summary>
    public static class CoinbaseUtils
    {
        /// <summary>
        /// Encoded an integer to save space.
        /// </summary>
        /// <remarks>
        /// Integer can be encoded depending on the represented value to save space. Variable length integers always precede 
        /// an array/vector of a type of data that may vary in length. Longer numbers are encoded in little endian. 
        /// </remarks>
        /// <specification>https://en.bitcoin.it/wiki/Protocol_specification#Variable_length_integer</specification>
        /// <example>
        /// nodejs: https://c9.io/raistlinthewiz/bitcoin-coinbase-varint-nodejs
        /// </example>
        /// <returns></returns>
        public static byte[] VarInt(UInt32 value)
        {
            if (value < 0xfd)
                return new[] {(byte) value};

            byte[] result;

            using (var stream = new MemoryStream())
            {
                if (value < 0xffff)
                {
                    stream.WriteValueU8(0xfd);
                    stream.WriteValueU16(((UInt16)value).LittleEndian());
                }
                else if (value < 0xffffffff)
                {
                    stream.WriteValueU8(0xfe);
                    stream.WriteValueU32(value.LittleEndian());
                }
                else
                {
                    stream.WriteValueU8(0xff);
                    stream.WriteValueU16(((UInt16)value).LittleEndian());
                }
                result = stream.ToArray();
            }

            return result;
        }

        /// <summary>
        /// Used to format height and date when putting into script signature:
        /// </summary>
        /// <remarks>
        /// Used to format height and date when putting into script signature: https://en.bitcoin.it/wiki/Script
        /// </remarks>
        /// <specification>https://github.com/bitcoin/bips/blob/master/bip-0034.mediawiki#specification</specification>
        /// <param name="value"></param>
        /// <returns>Serialized CScript</returns>
        /// <example>
        /// python: http://runnable.com/U3Hb26U1918Zx0NR/bitcoin-coinbase-serialize-number-python
        /// nodejs: http://runnable.com/U3HgCVY2RIAjrw9I/bitcoin-coinbase-serialize-number-nodejs-for-node-js
        /// </example>
        public static byte[] SerializeNumber(int value)
        {

            if (value >= 1 && value <= 16)
                return new byte[] {0x01, (byte)value};

            var buffer = new byte[9];
            byte lenght = 1;

            while (value > 127)
            {
                buffer[lenght++] = (byte)(value & 0xff);
                value >>= 8;
            }

            buffer[0] = lenght;
            buffer[lenght++] = (byte)value;

            return buffer.Slice(0, lenght);
        }

        /// <summary>
        /// Used to format height and date when putting into script signature:
        /// </summary>
        /// <remarks>
        /// Used to format height and date when putting into script signature: https://en.bitcoin.it/wiki/Script
        /// </remarks>
        /// <specification>https://github.com/bitcoin/bips/blob/master/bip-0034.mediawiki#specification</specification>
        /// <param name="value"></param>
        /// <returns>Serialized CScript</returns>
        /// <example>
        /// python: http://runnable.com/U3Hb26U1918Zx0NR/bitcoin-coinbase-serialize-number-python
        /// nodejs: http://runnable.com/U3HgCVY2RIAjrw9I/bitcoin-coinbase-serialize-number-nodejs-for-node-js
        /// </example>
        public static byte[] SerializeNumber(Int64 value)
        {
            if (value >= 1 && value <= 16)
                return new byte[] { 0x01, (byte)value };

            var buffer = new byte[9];
            byte lenght = 1;

            while (value > 127)
            {
                buffer[lenght++] = (byte)(value & 0xff);
                value >>= 8;
            }

            buffer[0] = lenght;
            buffer[lenght++] = (byte)value;

            return buffer.Slice(0, lenght);
        }

        /// <summary>
        /// Creates a serialized string used in script signature.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <example>
        /// python: http://runnable.com/U3Mya-5oZntF5Ira/bitcoin-coinbase-serialize-string-python
        /// nodejs: https://github.com/zone117x/node-stratum-pool/blob/dfad9e58c661174894d4ab625455bb5b7428881c/lib/util.js#L153
        /// </example>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] SerializeString(string input)
        {
            if (input.Length < 253)
                return ArrayHelpers.Combine(new[] { (byte)input.Length }, Encoding.UTF8.GetBytes(input));

            // if input string is >=253, we need need a special format.

            byte[] result;

            using (var stream = new MemoryStream())
            {
                if (input.Length < 0x10000)
                {
                    stream.WriteValueU8(253);
                    stream.WriteValueU16(((UInt16)input.Length).LittleEndian()); // write packed lenght.
                }
                else if (input.Length < 0x100000000)
                {
                    stream.WriteValueU8(254);
                    stream.WriteValueU32(((UInt32)input.Length).LittleEndian()); // write packed lenght.
                }
                else
                {
                    stream.WriteValueU8(255);
                    stream.WriteValueU16(((UInt16)input.Length).LittleEndian()); // write packed lenght.
                }

                stream.WriteString(input);
                result = stream.ToArray();
            }

            return result;
        }

        /// <summary>
        /// For POW coins - used to format wallet address for use in generation transaction's output
        /// </summary>
        /// <param name="address"></param>
        /// <example>
        /// nodejs: https://github.com/zone117x/node-stratum-pool/blob/dfad9e58c661174894d4ab625455bb5b7428881c/lib/util.js#L264
        /// </example>
        /// <returns></returns>
        public static byte[] CoinAddressToScript(string address)
        {
            var decoded = Base58.Decode(address);

            if (decoded.Length != 25)
                Log.Error("invalid address length for: " + address);

            var pubkey = decoded.Slice(1, -4);

            byte[] result;

            using (var stream = new MemoryStream())
            {
                stream.WriteValueU8(0x76);
                stream.WriteValueU8(0xa9);
                stream.WriteValueU8(0x14);
                stream.WriteBytes(pubkey);
                stream.WriteValueU8(0x88);
                stream.WriteValueU8(0xac);

                result = stream.ToArray();
            }

            return result;
        }

        public static Hash HashCoinbase(byte[] coinbase)
        {
            return coinbase.DoubleDigest();
        }
    }
}
