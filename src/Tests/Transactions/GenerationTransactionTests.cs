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
using System.Linq;
using Coinium.Coin.Daemon;
using Coinium.Coin.Daemon.Responses;
using Coinium.Common.Extensions;
using Coinium.Mining.Jobs;
using Coinium.Transactions;
using Coinium.Transactions.Coinbase;
using Gibbed.IO;
using Newtonsoft.Json;
using NSubstitute;
using Should.Fluent;
using Xunit;

namespace Tests.Transactions
{
    public class GenerationTransactionTests
    {
        [Fact]
        public void CreateGenerationTransactionTest()
        {
            /* expected data
                -- create-generation start --
                rpcData: {"version":2,"previousblockhash":"f5f50aa8da33bde3805fe2a56b5f5ab82a2c0ce8597ef97a0abd8348d33ef1b6","transactions":[],"coinbaseaux":{"flags":"062f503253482f"},"coinbasevalue":5000000000,"target":"00000fffff000000000000000000000000000000000000000000000000000000","mintime":1402264399,"mutable":["time","transactions","prevblock"],"noncerange":"00000000ffffffff","sigoplimit":20000,"sizelimit":1000000,"curtime":1402265776,"bits":"1e0fffff","height":294740}
                -- scriptSigPart data --
                -> height: 294740 serialized: 03547f04
                -> coinbase: 062f503253482f hex: 062f503253482f
                -> date: 1402265775319 final:1402265775 serialized: 04afe09453
                scriptSigPart1: 03547f04062f503253482f04afe0945308
                scriptSigPart2: /nodeStratum/ serialized: 0d2f6e6f64655374726174756d2f
                -- p1 data --
                txVersion: 1 packed: 01000000*/

            // block template json
            const string json = "{\"result\":{\"version\":2,\"previousblockhash\":\"f5f50aa8da33bde3805fe2a56b5f5ab82a2c0ce8597ef97a0abd8348d33ef1b6\",\"transactions\":[],\"coinbaseaux\":{\"flags\":\"062f503253482f\"},\"coinbasevalue\":5000000000,\"target\":\"00000fffff000000000000000000000000000000000000000000000000000000\",\"mintime\":1402264399,\"mutable\":[\"time\",\"transactions\",\"prevblock\"],\"noncerange\":\"00000000ffffffff\",\"sigoplimit\":20000,\"sizelimit\":1000000,\"curtime\":1402265776,\"bits\":\"1e0fffff\",\"height\":294740},\"error\":null,\"id\":1}";

            // now init blocktemplate from our json.
            var @object = JsonConvert.DeserializeObject<DaemonResponse<BlockTemplate>>(json);
            var blockTemplate = @object.Result;

            // init mockup objects
            var daemonClient = Substitute.For<IDaemonClient>();
            var addressValidation = new ValidateAddress
            {
                IsValid = true
            };
            daemonClient.ValidateAddress("n3Mvrshbf4fMoHzWZkDVbhhx4BLZCcU9oY").Returns(addressValidation);
            daemonClient.ValidateAddress("myxWybbhUkGzGF7yaf2QVNx3hh3HWTya5t").Returns(addressValidation);

            // start testing internal structures.

            var extraNonce = new ExtraNonce(0);

            // test serialized block height.
            var serializedBlockHeight = CoinbaseUtils.SerializeNumber(blockTemplate.Height);
            serializedBlockHeight.Should().Equal(new byte[] {0x03, 0x54, 0x7f, 0x04});

            // test coinbase flags
            blockTemplate.CoinBaseAux.Flags.Should().Equal("062f503253482f");

            // test the exact date for the case.
            const long unixDateTime = 1402265775319;
            const long finalUnixDateTime = unixDateTime/1000 | 0;
            finalUnixDateTime.Should().Equal((Int64)1402265775);
            var serializedUnixTime = CoinbaseUtils.SerializeNumber(finalUnixDateTime);
            serializedUnixTime.Should().Equal(new byte[] { 0x04, 0xaf, 0xe0, 0x94, 0x53 });

            // test sig-script part 1
            var txIn = new TxIn();
            txIn.SignatureScriptPart1 = txIn.SignatureScriptPart1.Concat(serializedBlockHeight).ToArray();
            txIn.SignatureScriptPart1 = txIn.SignatureScriptPart1.Concat(blockTemplate.CoinBaseAux.Flags.HexToByteArray()).ToArray();
            txIn.SignatureScriptPart1 = txIn.SignatureScriptPart1.Concat(serializedUnixTime).ToArray();
            txIn.SignatureScriptPart1 = txIn.SignatureScriptPart1.Concat(new[] { (byte)extraNonce.ExtraNoncePlaceholder.Length }).ToArray();
            txIn.SignatureScriptPart1.Should().Equal(new byte[] {0x03, 0x54, 0x7f, 0x04, 0x06, 0x2f, 0x50, 0x32, 0x53, 0x48, 0x2f, 0x04, 0xaf, 0xe0, 0x94, 0x53, 0x08});

            txIn.SignatureScriptPart2 = CoinbaseUtils.SerializeString("/nodeStratum/");
            txIn.SignatureScriptPart2.Should().Equal(new byte[] {0x0d,0x2f,0x6e,0x6f,0x64,0x65,0x53,0x74,0x72,0x61,0x74,0x75,0x6d,0x2f});

            // test the part 1
            const uint txVersion = 1;

            // create the first part.

            byte[] part1;
            using (var stream = new MemoryStream())
            {
                stream.WriteValueU32(txVersion.LittleEndian()); // write version

                // for proof-of-stake coins we need here timestamp - https://github.com/zone117x/node-stratum-pool/blob/b24151729d77e0439e092fe3a1cdbba71ca5d12e/lib/transactions.js#L210

                // write transaction input.
                //stream.WriteBytes(CoinbaseUtils.VarInt(this.InputsCount));
                //stream.WriteBytes(this.Inputs[0].PreviousOutput.Hash.Bytes);
                //stream.WriteValueU32(this.Inputs[0].PreviousOutput.Index.LittleEndian());

                //// write signnature script lenght
                //var signatureScriptLenght = (UInt32)(input.SignatureScriptPart1.Length + extraNonce.ExtraNoncePlaceholder.Length + input.SignatureScriptPart2.Length);
                //stream.WriteBytes(CoinbaseUtils.VarInt(signatureScriptLenght).ToArray());

                //stream.WriteBytes(input.SignatureScriptPart1);

                part1 = stream.ToArray();
            }

            part1.Take(4).Should().Equal(new Byte[] {0x01, 0x00, 0x00, 0x00});
        }
    }
}
