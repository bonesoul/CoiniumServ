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
using Coinium.Coin.Daemon.Responses;
using Coinium.Coin.Exceptions;
using Coinium.Common.Extensions;
using Coinium.Common.Helpers.Time;
using Coinium.Crypto;
using Coinium.Mining.Jobs;
using Coinium.Transactions.Coinbase;
using Coinium.Transactions.Script;
using Coinium.Transactions.Utils;
using Gibbed.IO;

namespace Coinium.Transactions
{
    /// <summary>
    /// A generation transaction.
    /// </summary>
    /// <remarks>
    /// * It has exactly one txin.
    /// * Txin's prevout hash is always 0000000000000000000000000000000000000000000000000000000000000000.
    /// * Txin's prevout index is 0xFFFFFFFF.
    /// More info:  http://bitcoin.stackexchange.com/questions/20721/what-is-the-format-of-coinbase-transaction
    ///             http://bitcoin.stackexchange.com/questions/21557/how-to-fully-decode-a-coinbase-transaction
    ///             http://bitcoin.stackexchange.com/questions/4990/what-is-the-format-of-coinbase-input-scripts
    /// </remarks>
    /// <specification>
    /// https://en.bitcoin.it/wiki/Protocol_specification#tx
    /// https://en.bitcoin.it/wiki/Transactions#Generation
    /// </specification>
    public class GenerationTransaction
    {
        /// <summary>
        /// Transaction data format version
        /// </summary>
        public UInt32 Version { get; private set; }

        /// <summary>
        /// Number of Transaction inputs
        /// </summary>
        public UInt32 InputsCount
        {
            get { return (UInt32)this.Inputs.Count; } 
        }

        /// <summary>
        /// A list of 1 or more transaction inputs or sources for coins
        /// </summary>
        public List<TxIn> Inputs { get; private set; } 

        // Number of Transaction outputs
        public UInt32 OutputsCount
        {
            get { return (UInt32)this.Inputs.Count; }
        }

        /// <summary>
        /// A list of 1 or more transaction outputs or destinations for coins
        /// </summary>
        public List<TxOut> Outputs;

        /// <summary>
        ///  For coins that support/require transaction comments
        /// </summary>
        public byte[] Message { get; private set; }

        /// <summary>
        /// The block number or timestamp at which this transaction is locked:
        ///     0 	Always locked
        ///  <  500000000 	Block number at which this transaction is locked
        ///  >= 500000000 	UNIX timestamp at which this transaction is locked
        /// </summary>
        public UInt32 LockTime { get; private set; }

        /// <summary>
        /// Part 1 of the generation transaction.
        /// </summary>
        public byte[] Initial { get; private set; }

        /// <summary>
        /// Part 2 of the generation transaction.
        /// </summary>
        public byte[] Final { get; private set; }

        /// <summary>
        /// Creates a new instance of generation transaction.
        /// </summary>
        /// <param name="extraNonce">The extra nonce.</param>
        /// <param name="daemonClient">The daemon client.</param>
        /// <param name="blockTemplate">The block template.</param>
        /// <param name="supportTxMessages">if set to <c>true</c> [support tx messages].</param>
        /// <remarks>
        /// Reference implementations:
        /// https://github.com/zone117x/node-stratum-pool/blob/b24151729d77e0439e092fe3a1cdbba71ca5d12e/lib/transactions.js
        /// https://github.com/Crypto-Expert/stratum-mining/blob/master/lib/coinbasetx.py
        /// </remarks>
        public GenerationTransaction(IExtraNonce extraNonce, IDaemonClient daemonClient, BlockTemplate blockTemplate, bool supportTxMessages = false)
        {
            // TODO: change internal processing code to functions, so the functions itself are testable.

            this.Version = (UInt32)(supportTxMessages ? 2 : 1);
            this.Message = CoinbaseUtils.SerializeString("https://github.com/CoiniumServ/CoiniumServ");
            this.LockTime = 0;

            var input = new TxIn
            {
                PreviousOutput = new OutPoint
                {
                    Hash = Hash.ZeroHash,
                    Index = (UInt32) Math.Pow(2, 32) - 1
                },
                Sequence = 0x0,
                SignatureScript =
                    new SignatureScript(
                        blockTemplate.Height, 
                        blockTemplate.CoinBaseAux.Flags,
                        TimeHelpers.NowInUnixTime(), 
                        (byte) extraNonce.ExtraNoncePlaceholder.Length,
                        "/CoiniumServ/")
            };


            this.Inputs = new List<TxIn> {input};

            // create the first part.
            using (var stream = new MemoryStream())
            {
                stream.WriteValueU32(this.Version.LittleEndian()); // write version

                // for proof-of-stake coins we need here timestamp - https://github.com/zone117x/node-stratum-pool/blob/b24151729d77e0439e092fe3a1cdbba71ca5d12e/lib/transactions.js#L210
                
                // write transaction input.
                stream.WriteBytes(CoinbaseUtils.VarInt(this.InputsCount));
                stream.WriteBytes(this.Inputs[0].PreviousOutput.Hash.Bytes);
                stream.WriteValueU32(this.Inputs[0].PreviousOutput.Index.LittleEndian());

                // write signature script lenght
                var signatureScriptLenght = (UInt32)(input.SignatureScript.Initial.Length + extraNonce.ExtraNoncePlaceholder.Length + input.SignatureScript.Final.Length);
                stream.WriteBytes(CoinbaseUtils.VarInt(signatureScriptLenght).ToArray());

                stream.WriteBytes(input.SignatureScript.Initial);

                this.Initial = stream.ToArray();
            }

            /*  The generation transaction must be split at the extranonce (which located in the transaction input
                scriptSig). Miners send us unique extranonces that we use to join the two parts in attempt to create
                a valid share and/or block. */

            this.Outputs = this.GenerateOutputTransactions(daemonClient, blockTemplate);
            var outputBuffers = this.GetOutputBuffer();

            // create the second part.
            using (var stream = new MemoryStream())
            {
                stream.WriteBytes(input.SignatureScript.Final);
                stream.WriteValueU32(this.Inputs[0].Sequence); // transaction inputs end here.

                stream.WriteBytes(CoinbaseUtils.VarInt((UInt32) outputBuffers.Length).ToArray()); // transaction output start here.
                stream.WriteBytes(outputBuffers); // transaction output ends here.

                stream.WriteValueU32(this.LockTime.LittleEndian());

                if (supportTxMessages)
                    stream.WriteBytes(this.Message);

                this.Final = stream.ToArray();
            }
        }

        /// <summary>
        /// Returns the buffer that contains output transactions.
        /// </summary>
        /// <returns></returns>
        private byte[] GetOutputBuffer()
        {
            byte[] result;

            using (var stream = new MemoryStream())
            {
                foreach (var transaction in this.Outputs)
                {
                    stream.WriteValueU64(transaction.Value);
                    stream.WriteBytes(transaction.PublicKeyScriptLenght);
                    stream.WriteBytes(transaction.PublicKeyScript);
                }

                result = stream.ToArray();
            }

            return result;
        }

        /// <summary>
        /// Generates the output transactions.
        /// </summary>
        /// <param name="daemonClient">The daemon client.</param>
        /// <param name="blockTemplate">The block template.</param>
        /// <returns></returns>
        /// <exception cref="InvalidWalletAddressException">
        /// </exception>
        private List<TxOut> GenerateOutputTransactions(IDaemonClient daemonClient, BlockTemplate blockTemplate)
        {
            const string poolCentralWalletAddress = "n3Mvrshbf4fMoHzWZkDVbhhx4BLZCcU9oY"; // pool's central wallet address.

            var transactions = new List<TxOut>();

            var rewardRecipients = new Dictionary<string, double>() // reward recipients addresses.
            {
                {"myxWybbhUkGzGF7yaf2QVNx3hh3HWTya5t", 1} // pool fee

            };

            // validate our pool wallet address.
            if (!daemonClient.ValidateAddress(poolCentralWalletAddress).IsValid)
                throw new InvalidWalletAddressException(poolCentralWalletAddress);

            // validate reward recipients addresses too.
            foreach (var pair in rewardRecipients)
            {
                if (!daemonClient.ValidateAddress(pair.Key).IsValid)
                    throw new InvalidWalletAddressException(pair.Key);
            }

            double blockReward = blockTemplate.Coinbasevalue;

            // generate output transactions for recipients (set in config).
            foreach (var pair in rewardRecipients)
            {
                var recipientScript = CoinbaseUtils.CoinAddressToScript(pair.Key); // generate the script to claim the output for recipient.
                var amount = blockReward * pair.Value / 100; // calculate the amount he recieves based on the percent of his shares.
                blockReward -= amount;

                var recipientTxOut = new TxOut()
                {
                    Value = ((UInt64)amount).LittleEndian(),
                    PublicKeyScriptLenght = CoinbaseUtils.VarInt((UInt32)recipientScript.Length),
                    PublicKeyScript = recipientScript
                };
                transactions.Add(recipientTxOut);
            }

            // send the remaining coins to pool's central wallet.
            var centralWalletScript = CoinbaseUtils.CoinAddressToScript(poolCentralWalletAddress); // script to claim the output for pool-fee.
          
            var poolFeeTxOut = new TxOut
            {
                Value = ((UInt64) blockReward).LittleEndian(),
                PublicKeyScriptLenght = CoinbaseUtils.VarInt((UInt32) centralWalletScript.Length),
                PublicKeyScript = centralWalletScript
            };

            transactions.Add(poolFeeTxOut);

            return transactions;
        }
    }    
}
