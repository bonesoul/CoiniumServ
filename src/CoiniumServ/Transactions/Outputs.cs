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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CoiniumServ.Coin.Address.Exceptions;
using CoiniumServ.Coin.Coinbase;
using CoiniumServ.Daemon;
using Gibbed.IO;

namespace CoiniumServ.Transactions
{
    public class Outputs : IOutputs
    {
        public List<TxOut> List { get; private set; }

        public IDaemonClient DaemonClient { get; private set; }


        public Outputs(IDaemonClient daemonClient)
        {
            DaemonClient = daemonClient;
            List = new List<TxOut>();
        }

        public void AddPoolWallet(string address, double amount)
        {
            Add(address, amount, true);
        }

        public void AddRecipient(string address, double amount)
        {
            Add(address, amount, false);
        }

        private void Add(string walletAddress, double amount, bool toFront)
        {
            // check if the supplied wallet address is correct.

            if (!DaemonClient.ValidateAddress(walletAddress).IsValid)
                throw new InvalidWalletAddressException(walletAddress);

            var recipientScript = Coin.Coinbase.Utils.CoinAddressToScript(walletAddress); // generate the script to claim the output for recipient.

            var txOut = new TxOut
            {
                Value = ((UInt64)amount).LittleEndian(),
                PublicKeyScriptLenght = Serializers.VarInt((UInt32)recipientScript.Length),
                PublicKeyScript = recipientScript
            };   

            if(toFront)
                List.Insert(0,txOut);
            else
                List.Add(txOut);
        }

        public byte[] GetBuffer()
        {
            byte[] result;

            using (var stream = new MemoryStream())
            {
                stream.WriteBytes(Serializers.VarInt((UInt32)List.Count).ToArray());

                foreach (var transaction in List)
                {
                    stream.WriteValueU64(transaction.Value);
                    stream.WriteBytes(transaction.PublicKeyScriptLenght);
                    stream.WriteBytes(transaction.PublicKeyScript);
                }

                result = stream.ToArray();
            }

            return result;
        }
    }
}
