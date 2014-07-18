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
using System.Text;
using CoiniumServ.Server.Mining.Stratum.Notifications;
using CoiniumServ.Utils.Extensions;
using CoiniumServ.Utils.Helpers.Arrays;
using Gibbed.IO;

namespace CoiniumServ.Coin.Coinbase
{
    public static class Serializers
    {
        /// <summary>
        /// Block data structure.
        /// </summary>
        /// <remarks>
        /// https://en.bitcoin.it/wiki/Protocol_specification#block
        /// </remarks>
        /// <param name="job"></param>
        /// <param name="header"></param>
        /// <param name="coinbase"></param>
        /// <returns></returns>
        public static byte[] SerializeBlock(IJob job, byte[] header, byte[] coinbase)
        {
            byte[] result;

            using (var stream = new MemoryStream())
            {
                stream.WriteBytes(header);
                stream.WriteBytes(VarInt((UInt32)job.BlockTemplate.Transactions.Length + 1));
                stream.WriteBytes(coinbase);

                foreach (var transaction in job.BlockTemplate.Transactions)
                {
                    stream.WriteBytes(transaction.Data.HexToByteArray());
                }

                // need to implement POS support too.

                result = stream.ToArray();
            }

            return result;
        }

        /// <summary>
        /// Block headers are sent in a headers packet in response to a getheaders message.
        /// </summary>
        /// <remarks>
        /// https://en.bitcoin.it/wiki/Protocol_specification#Block_Headers
        /// </remarks>
        /// <example>
        /// nodejs: https://github.com/zone117x/node-stratum-pool/blob/master/lib/blockTemplate.js#L85
        /// </example>
        /// <param name="job"></param>
        /// <param name="merkleRoot"></param>
        /// <param name="nTime"></param>
        /// <param name="nonce"></param>
        /// <returns></returns>
        public static byte[] SerializeHeader(IJob job, byte[] merkleRoot, UInt32 nTime, UInt32 nonce)
        {
            byte[] result;

            using (var stream = new MemoryStream())
            {
                stream.WriteValueU32(nonce.BigEndian());
                stream.WriteValueU32(Convert.ToUInt32(job.EncodedDifficulty, 16).BigEndian());
                stream.WriteValueU32(nTime.BigEndian());
                stream.WriteBytes(merkleRoot);
                stream.WriteBytes(job.PreviousBlockHash.HexToByteArray());
                stream.WriteValueU32(job.BlockTemplate.Version.BigEndian());

                result = stream.ToArray();
                result = result.ReverseBytes();
            }

            return result;
        }

        public static byte[] SerializeCoinbase(IJob job, UInt32 extraNonce1, UInt32 extraNonce2)
        {
            var extraNonce1Buffer = BitConverter.GetBytes(extraNonce1.BigEndian());
            var extraNonce2Buffer = BitConverter.GetBytes(extraNonce2.BigEndian());

            byte[] result;

            using (var stream = new MemoryStream())
            {
                stream.WriteBytes(job.CoinbaseInitial.HexToByteArray());
                stream.WriteBytes(extraNonce1Buffer);
                stream.WriteBytes(extraNonce2Buffer);
                stream.WriteBytes(job.CoinbaseFinal.HexToByteArray());

                result = stream.ToArray();
            }

            return result;
        }

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
                return new[] { (byte)value };

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
            // TODO: implement a test for it!

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
                    stream.WriteValueU16(((UInt16)input.Length).LittleEndian()); // write packed length.
                }
                else if ((long)input.Length < 0x100000000)
                {
                    stream.WriteValueU8(254);
                    stream.WriteValueU32(((UInt32)input.Length).LittleEndian()); // write packed length.
                }
                else
                {
                    stream.WriteValueU8(255);
                    stream.WriteValueU16(((UInt16)input.Length).LittleEndian()); // write packed length.
                }

                stream.WriteString(input);
                result = stream.ToArray();
            }

            return result;
        }

    }
}
