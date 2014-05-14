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

using System;
using System.IO;
using Coinium.Common.Extensions;
using Coinium.Common.Helpers.Arrays;
using Gibbed.IO;
using Serilog;

namespace Coinium.Core.Coin
{
    /// <summary>
    /// Provides helper functions for "serialized CSscript formatting" as defined here: https://github.com/bitcoin/bips/blob/master/bip-0034.mediawiki#specification
    /// </summary>
    public static class CoinbaseUtils
    {
        /// <summary>
        /// first byte is number of bytes in the number (will be 0x03 on main net for the next 300 or so years), following bytes 
        /// are little-endian representation of the number.
        /// </summary>
        /// <remarks>
        /// Specification: https://github.com/bitcoin/bips/blob/master/bip-0034.mediawiki#specification
        /// python: https://github.com/Crypto-Expert/stratum-mining/blob/master/lib/util.py#L204
        ///         http://runnable.com/U3Hb26U1918Zx0NR/bitcoin-coinbase-serialize-number-python
        /// nodejs: https://github.com/zone117x/node-stratum-pool/blob/a06ba67ab327e09f74cb7d14291ea1ece541104c/lib/util.js#L135
        ///         http://runnable.com/U3HgCVY2RIAjrw9I/bitcoin-coinbase-serialize-number-nodejs-for-node-js
        /// Used to format height and date when putting into script signature: https://en.bitcoin.it/wiki/Script
        /// </remarks>
        /// <param name="value"></param>
        /// <returns>Serialized CScript</returns>
        public static byte[] SerializeNumber(int value)
        {

            if (value >= 1 && value <= 16)
                return new byte[] {0x01, (byte)value};

            var buffer = new byte[9];
            byte lenght = 1;

            while (value > 127)
            {
                buffer[lenght++] = (byte)(value & 0xff); // n % 256
                value >>= 8;
            }

            buffer[0] = lenght;
            buffer[lenght++] = (byte)value;

            return buffer.Slice(0, lenght);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// python: https://github.com/Crypto-Expert/stratum-mining/blob/master/lib/util.py#L20
        ///         http://runnable.com/U3Mya-5oZntF5Ira/bitcoin-coinbase-serialize-string-python
        /// nodejs: https://github.com/zone117x/node-stratum-pool/blob/b24151729d77e0439e092fe3a1cdbba71ca5d12e/lib/util.js#L153
        /// </remarks>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] SerializeString(string input)
        {
            if (input.Length < 253)
                return ArrayHelpers.Combine(new[] { (byte)input.Length }, System.Text.Encoding.UTF8.GetBytes(input));

            // if input string is >=253, we need need a special format.

            byte[] result;

            using (var stream = new MemoryStream())
            {
                if (input.Length < 0x10000)
                {
                    var packedLenght = ((UInt16) input.Length).LittleEndian();
                    stream.WriteByte(253);
                    stream.WriteValueU16(packedLenght);
                }
                else if (input.Length < 0x100000000)
                {
                    var packedLenght = ((UInt32) input.Length).LittleEndian();
                    stream.WriteByte(254);
                    stream.WriteValueU32(packedLenght);
                }
                else
                {
                    var packedLenght = ((UInt16)input.Length).LittleEndian();
                    stream.WriteByte(255);
                    stream.WriteValueU16(packedLenght);
                }

                stream.WriteString(input);
                result = stream.ToArray();
            }

            return result;
        }
    }
}
