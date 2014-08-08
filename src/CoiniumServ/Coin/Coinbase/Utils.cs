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
using System.IO;
using System.Linq;
using CoiniumServ.Coin.Address;
using CoiniumServ.Cryptology;
using CoiniumServ.Utils.Extensions;
using CoiniumServ.Utils.Numerics;
using Gibbed.IO;
using Serilog;

namespace CoiniumServ.Coin.Coinbase
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
        /// <param name="doubleDigest"></param>
        /// <returns></returns>
        public static Hash HashCoinbase(byte[] coinbase, bool doubleDigest = true)
        {
            return doubleDigest ? coinbase.DoubleDigest() : coinbase.Digest();

            // TODO: fix this according - https://github.com/zone117x/node-stratum-pool/blob/eb4b62e9c4de8a8cde83c2b3756ca1a45f02b957/lib/jobManager.js#L69
        }

        /// <summary>
        /// Used to convert getblocktemplate bits field into target if target is not included.
        //// More info: https://en.bitcoin.it/wiki/Target
        /// </summary>
        /// <param name="bits"></param>
        /// <returns></returns>
        public static BigInteger BigIntFromBitsHex(this string bits)
        {
            // TODO: implement a test for it!

            return bits.HexToByteArray().BigIntFromBitsBuffer();
        }

        public static BigInteger BigIntFromBitsBuffer(this byte[] buffer)
        {
            // TODO: implement a test for it!

            var numBytes = Convert.ToByte(buffer.Take(1));
            var bigIntBits = new BigInteger(buffer.Slice(1, buffer.Length - 1));

            var multiplier = new BigInteger(2 ^ 8*(numBytes - 3));
            var target = BigInteger.Multiply(bigIntBits, multiplier);

            return target;
        }
    }
}
