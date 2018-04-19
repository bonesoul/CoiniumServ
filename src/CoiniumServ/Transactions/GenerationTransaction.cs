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
using CoiniumServ.Coin.Coinbase;
using CoiniumServ.Cryptology;
using CoiniumServ.Daemon;
using CoiniumServ.Daemon.Responses;
using CoiniumServ.Jobs;
using CoiniumServ.Pools;
using CoiniumServ.Transactions.Script;
using CoiniumServ.Utils.Helpers;
using Gibbed.IO;

namespace CoiniumServ.Transactions
{
    // TODO: convert generation transaction to ioc & DI based.

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
    public class GenerationTransaction : IGenerationTransaction
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
            get { return (UInt32)Inputs.Count; } 
        }

        /// <summary>
        /// A list of 1 or more transaction inputs or sources for coins
        /// </summary>
        public List<TxIn> Inputs { get; private set; } 

        /// <summary>
        /// A list of 1 or more transaction outputs or destinations for coins
        /// </summary>
        public IOutputs Outputs { get; set; }

        /// <summary>
        ///  For coins that support/require transaction comments
        /// </summary>
        public byte[] TxMessage { get; private set; }

        /// <summary>
        /// The block number or timestamp at which this transaction is locked:
        ///                 0 	        Always locked
        ///  LESS THEN      500000000 	Block number at which this transaction is locked
        ///  EQUAL GREATER  500000000 	UNIX timestamp at which this transaction is locked
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

        public IDaemonClient DaemonClient { get; private set; }

        public IBlockTemplate BlockTemplate { get; private set; }

        public IExtraNonce ExtraNonce { get; private set; }

        public IPoolConfig PoolConfig { get; private set; }

        /// <summary>
        /// Creates a new instance of generation transaction.
        /// </summary>
        /// <param name="extraNonce">The extra nonce.</param>
        /// <param name="daemonClient">The daemon client.</param>
        /// <param name="blockTemplate">The block template.</param>
        /// <param name="poolConfig">The associated pool's configuration</param>
        /// <remarks>
        /// Reference implementations:
        /// https://github.com/zone117x/node-stratum-pool/blob/b24151729d77e0439e092fe3a1cdbba71ca5d12e/lib/transactions.js
        /// https://github.com/Crypto-Expert/stratum-mining/blob/master/lib/coinbasetx.py
        /// </remarks>
        public GenerationTransaction(IExtraNonce extraNonce, IDaemonClient daemonClient, IBlockTemplate blockTemplate, IPoolConfig poolConfig)
        {
            // TODO: we need a whole refactoring here.
            // we should use DI and it shouldn't really require daemonClient connection to function.

            BlockTemplate = blockTemplate;
            ExtraNonce = extraNonce;
            PoolConfig = poolConfig;

            Version = blockTemplate.Version;
            TxMessage = Serializers.SerializeString(poolConfig.Meta.TxMessage);
            LockTime = 0;

            // transaction inputs
            Inputs = new List<TxIn>
            {
                new TxIn
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
                            TimeHelpers.NowInUnixTimestamp(),
                            (byte) extraNonce.ExtraNoncePlaceholder.Length,
                            "/CoiniumServ/")
                }
            }; 

            // transaction outputs
            Outputs = new Outputs(daemonClient, poolConfig.Coin);

            double blockReward = BlockTemplate.Coinbasevalue; // the amount rewarded by the block.

            // generate output transactions for recipients (set in config).
            foreach (var pair in poolConfig.Rewards)
            {
                var amount = blockReward * pair.Value / 100; // calculate the amount he recieves based on the percent of his shares.
                blockReward -= amount;

                Outputs.AddRecipient(pair.Key, amount);
            }

            // send the remaining coins to pool's central wallet.
            Outputs.AddPoolWallet(poolConfig.Wallet.Adress, blockReward); 
        }

        public void Create()
        {
            // create the first part.
            using (var stream = new MemoryStream())
            {
                stream.WriteValueU32(Version.LittleEndian()); // write version

                if(PoolConfig.Coin.Options.IsProofOfStakeHybrid) // if coin is a proof-of-stake coin
                    stream.WriteValueU32(BlockTemplate.CurTime); // include time-stamp in the transaction.

                // write transaction input.
                stream.WriteBytes(Serializers.VarInt(InputsCount));
                stream.WriteBytes(Inputs.First().PreviousOutput.Hash.Bytes);
                stream.WriteValueU32(Inputs.First().PreviousOutput.Index.LittleEndian());

                // write signature script lenght
                var signatureScriptLenght = (UInt32)(Inputs.First().SignatureScript.Initial.Length + ExtraNonce.ExtraNoncePlaceholder.Length + Inputs.First().SignatureScript.Final.Length);
                stream.WriteBytes(Serializers.VarInt(signatureScriptLenght).ToArray());

                stream.WriteBytes(Inputs.First().SignatureScript.Initial);

                Initial = stream.ToArray();
            }

            /*  The generation transaction must be split at the extranonce (which located in the transaction input
                scriptSig). Miners send us unique extranonces that we use to join the two parts in attempt to create
                a valid share and/or block. */

            // create the second part.
            using (var stream = new MemoryStream())
            {
                // transaction input
                stream.WriteBytes(Inputs.First().SignatureScript.Final);
                stream.WriteValueU32(Inputs.First().Sequence); 
                // transaction inputs end here.

                // transaction output
                var outputBuffer = Outputs.GetBuffer();
                stream.WriteBytes(outputBuffer); 
                // transaction output ends here.

                stream.WriteValueU32(LockTime.LittleEndian());

                if (PoolConfig.Coin.Options.TxMessageSupported)
                    stream.WriteBytes(TxMessage);

                Final = stream.ToArray();
            }
        }
    }    
}
