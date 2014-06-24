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

using System.IO;
using System.Numerics;
using Coinium.Coin.Address;
using Coinium.Common.Extensions;
using Coinium.Crypto;
using Gibbed.IO;
using Serilog;

namespace Coinium.Coin.Coinbase
{
    /// <summary>
    /// Provides helper functions for "serialized CSscript formatting" as defined here: https://github.com/bitcoin/bips/blob/master/bip-0034.mediawiki#specification
    /// </summary>
    public static class Utils
    {
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
            // TODO: implement a test for it!

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

        /// <summary>
        /// Hashes the coinbase.
        /// </summary>
        /// <param name="coinbase"></param>
        /// <returns></returns>
        public static Hash HashCoinbase(byte[] coinbase)
        {
            return coinbase.DoubleDigest();
        }

        public static BigInteger FromBitsHex(string bits)
        {
            return BigInteger.One;
        }

        public static BigInteger FromBitsBuffer(byte[] buffer)
        {
            return BigInteger.One;
        }
    }
}
