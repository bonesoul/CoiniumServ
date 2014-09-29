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
using CoiniumServ.Coin.Config;
using CoiniumServ.Daemon;
using Gibbed.IO;

namespace CoiniumServ.Transactions
{
    public class Outputs : IOutputs
    {
        public List<TxOut> List { get; private set; }

        private readonly IDaemonClient _daemonClient;

        private readonly ICoinConfig _coinConfig;

        public Outputs(IDaemonClient daemonClient, ICoinConfig coinConfig)
        {
            _daemonClient = daemonClient;
            _coinConfig = coinConfig;

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

        private void Add(string walletAddress, double amount, bool poolCentralAddress)
        {
            // check if the supplied wallet address is correct.
            var result = _daemonClient.ValidateAddress(walletAddress);

            if (!result.IsValid)
                throw new InvalidWalletAddressException(walletAddress);

            // POS coin's require the PubKey to be used in coinbase for pool's central wallet address and
            // and we can only get PubKey of an address when the wallet owns it.
            // so check if we own the address when we are on a POS coin and adding the output for pool central address.
            if (_coinConfig.Options.IsProofOfStakeHybrid && poolCentralAddress) 
            {
                if(!result.IsMine || string.IsNullOrEmpty(result.PubKey)) // given address should be ours and PubKey should not be empty.
                    throw new AddressNotOwnedException(walletAddress);
            }

            // generate the script to claim the output for recipient.
            var recipientScript = _coinConfig.Options.IsProofOfStakeHybrid && poolCentralAddress
                ? Coin.Coinbase.Utils.PubKeyToScript(result.PubKey) // pos coins use pubkey within script for pool central address.
                : Coin.Coinbase.Utils.CoinAddressToScript(walletAddress); // for others (pow coins, reward recipients in pos coins) use wallet address instead.

            var txOut = new TxOut
            {
                Value = ((UInt64)amount).LittleEndian(),
                PublicKeyScriptLenght = Serializers.VarInt((UInt32)recipientScript.Length),
                PublicKeyScript = recipientScript
            };

            if (poolCentralAddress) // if we are adding output for the pool's central wallet address.
                List.Insert(0, txOut); // add it to the front of the queue.
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
