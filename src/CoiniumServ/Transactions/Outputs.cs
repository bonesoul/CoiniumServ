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
