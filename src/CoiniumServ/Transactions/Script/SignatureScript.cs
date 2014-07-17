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
using CoiniumServ.Coin.Coinbase;
using CoiniumServ.Transactions.Utils;
using CoiniumServ.Utils.Extensions;
using Gibbed.IO;

namespace CoiniumServ.Transactions.Script
{
    public class SignatureScript : ISignatureScript
    {
        /// <summary>
        /// Computational Script for confirming transaction authorization - part 1
        /// </summary>
        public byte[] Initial { get; private set; }

        /// <summary>
        /// Computational Script for confirming transaction authorization - part 2
        /// </summary>
        public byte[] Final { get; private set; }

        public SignatureScript(int blockHeight, string coinbaseAuxFlags, Int64 unixTime, byte extraNoncePlaceholder, string signature)
        {
            // cook input signature script.
            // The txin's prevout script is an arbitrary byte array (it doesn't have to be a valid script, though this is commonly 
            // done anyway) of 2 to 100 bytes. It has to start with a correct push of the block height (see BIP34).

            var serializedBlockHeight = Serializers.SerializeNumber(blockHeight);
            var serializedUnixTime = TransactionUtils.GetSerializedUnixDateTime(unixTime);

            using (var stream = new MemoryStream())
            {
                stream.WriteBytes(serializedBlockHeight);
                stream.WriteBytes(coinbaseAuxFlags.HexToByteArray());
                stream.WriteBytes(serializedUnixTime);
                stream.WriteByte(extraNoncePlaceholder);

                Initial = stream.ToArray();
            }

            Final = Serializers.SerializeString(signature);
        }
    }
}
