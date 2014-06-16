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
using Coinium.Common.Extensions;
using Coinium.Server.Stratum.Notifications;
using Coinium.Transactions.Coinbase;
using Gibbed.IO;

namespace Coinium.Coin.Helpers
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
        public static byte[] SerializeBlock(Job job, byte[] header, byte[] coinbase)
        {
            byte[] result;

            using (var stream = new MemoryStream())
            {
                stream.WriteBytes(header);
                stream.WriteBytes(CoinbaseUtils.VarInt((UInt32)job.BlockTemplate.Transactions.Length));
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
        public static byte[] SerializeHeader(Job job, byte[] merkleRoot, UInt32 nTime, UInt32 nonce)
        {
            byte[] result;

            using (var stream = new MemoryStream())
            {
                stream.WriteValueU32(nonce);
                stream.WriteValueU32(Convert.ToUInt32(job.NetworkDifficulty, 16));
                stream.WriteValueU32(nTime);
                stream.WriteBytes(merkleRoot);
                stream.WriteBytes(job.PreviousBlockHashReversed.HexToByteArray());
                stream.WriteValueU32(job.BlockTemplate.Version.BigEndian());

                result = stream.ToArray();
                result = result.ReverseBytes();
            }

            return result;
        }

        public static byte[] SerializeCoinbase(Job job, UInt64 extraNonce1, UInt32 extraNonce2)
        {
            var extraNonce1Buffer = BitConverter.GetBytes(extraNonce1);
            var extraNonce2Buffer = BitConverter.GetBytes(extraNonce2);

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
    }
}
