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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Coinium.Coin.Daemon;
using Coinium.Coin.Exceptions;
using Coinium.Transactions.Coinbase;
using Gibbed.IO;

namespace Coinium.Transactions
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

        public void AddPool(string address, double amount)
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

            var recipientScript = CoinbaseUtils.CoinAddressToScript(walletAddress); // generate the script to claim the output for recipient.

            var txOut = new TxOut
            {
                Value = ((UInt64)amount).LittleEndian(),
                PublicKeyScriptLenght = CoinbaseUtils.VarInt((UInt32)recipientScript.Length),
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
                stream.WriteBytes(CoinbaseUtils.VarInt((UInt32)List.Count).ToArray());

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
