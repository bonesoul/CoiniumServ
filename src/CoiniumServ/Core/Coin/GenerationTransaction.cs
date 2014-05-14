/*
 *   Coinium - Crypto Currency Pool Software - https://github.com/CoiniumServ/CoiniumServ
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
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Coinium.Common.Extensions;
using Coinium.Common.Helpers.Time;
using Coinium.Core.Coin.Daemon.Responses;
using Coinium.Core.Crypto;
using Coinium.Core.Server.Stratum;
using Serilog;

namespace Coinium.Core.Coin
{
    /// <summary>
    /// A generation transaction.
    /// It has exactly one txin.
    /// <remarks>
    /// Structure:  https://en.bitcoin.it/wiki/Protocol_specification#tx
    /// More info:  http://bitcoin.stackexchange.com/questions/20721/what-is-the-format-of-coinbase-transaction
    ///             http://bitcoin.stackexchange.com/questions/21557/how-to-fully-decode-a-coinbase-transaction
    ///             http://bitcoin.stackexchange.com/questions/4990/what-is-the-format-of-coinbase-input-scripts
    /// 
    /// Txin's prevout hash is always 0000000000000000000000000000000000000000000000000000000000000000.
    /// Txin's prevout index is 0xFFFFFFFF.
    /// </remarks>
    /// </summary>
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
        /// Creates a new instance of generation transaction.
        /// </summary>
        /// <remarks>
        /// Reference implementations:
        ///     https://github.com/zone117x/node-stratum-pool/blob/b24151729d77e0439e092fe3a1cdbba71ca5d12e/lib/transactions.js
        ///     https://github.com/Crypto-Expert/stratum-mining/blob/master/lib/coinbasetx.py
        /// </remarks>
        /// <param name="blockTemplate"></param>
        /// <param name="supportTxMessages"></param>
        public GenerationTransaction(BlockTemplate blockTemplate, bool supportTxMessages = false)
        {
            this.Version = (UInt32)(supportTxMessages ? 2 : 1);
            this.Message = CoinbaseUtils.SerializeString("https://github.com/CoiniumServ/CoiniumServ");
            this.LockTime = 0;

            var input = new TxIn();
            input.PreviousOutput = new OutPoint();
            input.PreviousOutput.Hash = Sha256Hash.ZeroHash;
            input.PreviousOutput.Index = (UInt32) Math.Pow(2, 32) - 1;
            input.Sequence = 0x0;

            // cook input signature script.
            // The txin's prevout script is an arbitrary byte array (it doesn't have to be a valid script, though this is commonly 
            // done anyway) of 2 to 100 bytes. It has to start with a correct push of the block height (see BIP34).

            input.SignatureScriptPart1 = new byte[0]; 

            var serializedBlockHeight = CoinbaseUtils.SerializeNumber(blockTemplate.Height);
            var coinBaseAuxFlags = blockTemplate.CoinBaseAux.Flags.HexToByteArray();
            var serializedUnixTime = CoinbaseUtils.SerializeNumber(TimeHelpers.NowInUnixTime()/1000 | 0);

            input.SignatureScriptPart1 = input.SignatureScriptPart1.Concat(serializedBlockHeight).ToArray();
            input.SignatureScriptPart1 = input.SignatureScriptPart1.Concat(coinBaseAuxFlags).ToArray();
            input.SignatureScriptPart1 = input.SignatureScriptPart1.Concat(serializedUnixTime).ToArray();
            input.SignatureScriptPart1 = input.SignatureScriptPart1.Concat(new byte[StratumService.ExtraNoncePlaceholder.Length]).ToArray();

            input.SignatureScriptPart2 = CoinbaseUtils.SerializeString("/CoiniumServ/");

            this.Inputs = new List<TxIn>();
            this.Inputs.Add(input);



        }
    }

    /// <summary>
    /// Inputs for transaction.
    /// <remarks>
    /// Structure:  https://en.bitcoin.it/wiki/Protocol_specification#tx
    /// Information: http://bitcoin.stackexchange.com/a/20725/8899
    /// </remarks>
    /// </summary>
    public class TxIn
    {
        /// <summary>
        /// The previous output transaction reference, as an OutPoint structure
        /// </summary>
        public OutPoint PreviousOutput { get; set; }

        /// <summary>
        /// The length of the signature script
        /// </summary>
        public int SignatureScriptLenght { get; set; }

        /// <summary>
        /// Computational Script for confirming transaction authorization - part 1
        /// </summary>
        public byte[] SignatureScriptPart1 { get; set; }

        /// <summary>
        /// Computational Script for confirming transaction authorization - part 2
        /// </summary>
        public byte[] SignatureScriptPart2 { get; set; }

        /// <summary>
        /// Transaction version as defined by the sender. Intended for "replacement" of transactions when information is updated before inclusion into a block.
        /// </summary>
        public UInt32 Sequence { get; set; }
    }

    /// <summary>
    /// Structure:  https://en.bitcoin.it/wiki/Protocol_specification#tx
    /// </summary>
    public class OutPoint
    {
        /// <summary>
        /// The hash of the referenced transaction - as we creating a generation transaction - none.
        /// </summary>
        public Sha256Hash Hash { get; set; }

        /// <summary>
        /// The index of the specific output in the transaction. The first output is 0, etc.
        /// </summary>
        public UInt32 Index { get; set; }
    }

    /// <summary>
    /// Outpus for transaction.
    /// <remarks>
    /// Structure:  https://en.bitcoin.it/wiki/Protocol_specification#tx
    /// </remarks>
    /// </summary>
    public class TxOut
    {
        /// <summary>
        /// Transaction Value
        /// </summary>
        public UInt64 Value { get; set; }

        /// <summary>
        /// Length of the pk_script
        /// </summary>
        public int PublicKeyScriptLenght { get; set; }

        /// <summary>
        /// Usually contains the public key as a Bitcoin script setting up conditions to claim this output.
        /// </summary>
        public string PublicKeyScript { get; set; }

    }
}
