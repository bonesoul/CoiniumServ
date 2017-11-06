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
