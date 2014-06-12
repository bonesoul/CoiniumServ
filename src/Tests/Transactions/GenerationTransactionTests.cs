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
using Coinium.Common.Helpers.Time;
using Coinium.Crypto;
using Coinium.Mining.Jobs;
using Coinium.Transactions;
using Coinium.Transactions.Coinbase;
using Coinium.Transactions.Script;
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
            /*  sample data
            -- create-generation start --
            rpcData: {"version":2,"previousblockhash":"8e5eb2399fcaae485bad3d80265345c6b37ccf0141fecf2a88c5d89b56a2ca86","transactions":[],"coinbaseaux":{"flags":"062f503253482f"},"coinbasevalue":5000000000,"target":"000000f399000000000000000000000000000000000000000000000000000000","mintime":1402568989,"mutable":["time","transactions","prevblock"],"noncerange":"00000000ffffffff","sigoplimit":20000,"sizelimit":1000000,"curtime":1402569149,"bits":"1e00f399","height":300661}
            -- scriptSigPart data --
            -> height: 300661 serialized: 03759604
            -> coinbase: 062f503253482f hex: 062f503253482f
            -> date: 1402569148396 final:1402569148 serialized: 04bc819953
            scriptSigPart1: 03759604062f503253482f04bc81995308
            scriptSigPart2: /nodeStratum/ serialized: 0d2f6e6f64655374726174756d2f
            -- p1 data --
            txVersion: 1 packed: 01000000
            txInputsCount: 1 varIntBuffer: 01
            txInPrevOutHash: 0 uint256BufferFromHash: 0000000000000000000000000000000000000000000000000000000000000000
            txInPrevOutIndex: 4294967295 packUInt32LE: ffffffff
            scriptSigPart1.length: 17 extraNoncePlaceholder.length:8 scriptSigPart2.length:14 all: 39 varIntBuffer: 27
            scriptSigPart1: 03759604062f503253482f04bc81995308
            p1: 01000000010000000000000000000000000000000000000000000000000000000000000000ffffffff2703759604062f503253482f04bc81995308 */

            // block template json
            const string json = "{\"result\":{\"version\":2,\"previousblockhash\":\"8e5eb2399fcaae485bad3d80265345c6b37ccf0141fecf2a88c5d89b56a2ca86\",\"transactions\":[],\"coinbaseaux\":{\"flags\":\"062f503253482f\"},\"coinbasevalue\":5000000000,\"target\":\"000000f399000000000000000000000000000000000000000000000000000000\",\"mintime\":1402568989,\"mutable\":[\"time\",\"transactions\",\"prevblock\"],\"noncerange\":\"00000000ffffffff\",\"sigoplimit\":20000,\"sizelimit\":1000000,\"curtime\":1402569149,\"bits\":\"1e00f399\",\"height\":300661},\"error\":null,\"id\":1}";

            // now init blocktemplate from our json.
            var @object = JsonConvert.DeserializeObject<DaemonResponse<BlockTemplate>>(json);
            var blockTemplate = @object.Result;

            // init mockup objects
            var daemonClient = Substitute.For<IDaemonClient>();
            var addressValidation = new ValidateAddress { IsValid = true };
            daemonClient.ValidateAddress("n3Mvrshbf4fMoHzWZkDVbhhx4BLZCcU9oY").Returns(addressValidation);
            daemonClient.ValidateAddress("myxWybbhUkGzGF7yaf2QVNx3hh3HWTya5t").Returns(addressValidation);

            var extraNonce = new ExtraNonce(0);

            // create the test object.
            var generationTransaction = new GenerationTransaction(extraNonce, daemonClient, blockTemplate);

            // use the exactly same inputscript data within our sample data.
            generationTransaction.Inputs.First().SignatureScript = new SignatureScript(
                blockTemplate.Height,
                blockTemplate.CoinBaseAux.Flags,
                1402569148396,
                (byte) extraNonce.ExtraNoncePlaceholder.Length,
                "/nodeStratum/");

            generationTransaction.Create();

            // test version.
            generationTransaction.Version.Should().Equal((UInt32)1);
            generationTransaction.Initial.Take(4).Should().Equal(new byte[] { 0x01, 0x00, 0x00, 0x00 }); 

            // test inputs count.
            generationTransaction.InputsCount.Should().Equal((UInt32)1);
            generationTransaction.Initial.Skip(4).Take(1).Should().Equal(new byte[] { 0x01 }); 

            // test the input previous-output hash
            generationTransaction.Initial.Skip(5)
                .Take(32)
                .Should()
                .Equal(new byte[]
                {
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
                });

            generationTransaction.Inputs.First().PreviousOutput.Index.Should().Equal(0xffffffff);
            generationTransaction.Initial.Skip(37).Take(4).Should().Equal(new byte[] {0xff, 0xff, 0xff, 0xff});

            // test the lenghts byte
            generationTransaction.Inputs.First().SignatureScript.Initial.Length.Should().Equal(17);
            extraNonce.ExtraNoncePlaceholder.Length.Should().Equal(8);
            generationTransaction.Inputs.First().SignatureScript.Final.Length.Should().Equal(14);
            generationTransaction.Initial.Skip(41).Take(1).Should().Equal(new byte[] { 0x27 });

            // test the signature script
            generationTransaction.Initial.Skip(42).Take(17).Should().Equal(new byte[]
                {
                    0x03, 0x75, 0x96, 0x04, 0x06, 0x2f, 0x50, 0x32, 0x53, 0x48, 0x2f, 0x04, 0xbc, 0x81, 0x99, 0x53,
                    0x08
                });

            // test the generation transactions initial part.
            generationTransaction.Initial.Should().Equal(new byte[]
                {
                    0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0xff, 0xff, 0x27, 0x03, 0x75, 0x96,
                    0x04, 0x06, 0x2f, 0x50, 0x32, 0x53, 0x48, 0x2f, 0x04, 0xbc, 0x81, 0x99, 0x53, 0x08
                });
        }
    }
}
