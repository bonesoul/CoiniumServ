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
using System.Linq;
using Coinium.Coin.Daemon;
using Coinium.Coin.Daemon.Responses;
using Coinium.Mining.Jobs;
using Coinium.Transactions;
using Coinium.Transactions.Script;
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
                rpcData: {"version":2,"previousblockhash":"e9bbcc9b46ed98fd4850f2d21e85566defdefad3453460caabc7a635fc5a1261","transactions":[],"coinbaseaux":{"flags":"062f503253482f"},"coinbasevalue":5000000000,"target":"0000004701b20000000000000000000000000000000000000000000000000000","mintime":1402660580,"mutable":["time","transactions","prevblock"],"noncerange":"00000000ffffffff","sigoplimit":20000,"sizelimit":1000000,"curtime":1402661060,"bits":"1d4701b2","height":302526}
                -- scriptSigPart data --
                -> height: 302526 serialized: 03be9d04
                -> coinbase: 062f503253482f hex: 062f503253482f
                -> date: 1402661059432 final:1402661059 serialized: 04c3e89a53
                -- p1 data --
                txVersion: 1 packed: 01000000
                txInputsCount: 1 varIntBuffer: 01
                txInPrevOutHash: 0 uint256BufferFromHash: 0000000000000000000000000000000000000000000000000000000000000000
                txInPrevOutIndex: 4294967295 packUInt32LE: ffffffff
                scriptSigPart1.length: 17 extraNoncePlaceholder.length:8 scriptSigPart2.length:14 all: 39 varIntBuffer: 27
                scriptSigPart1: 03be9d04062f503253482f04c3e89a5308
                p1: 01000000010000000000000000000000000000000000000000000000000000000000000000ffffffff2703be9d04062f503253482f04c3e89a5308
                -- generateOutputTransactions --
                block-reward: 5000000000
                recipient-reward: 50000000 packInt64LE: 80f0fa0200000000
                lenght: 25 varIntBuffer: 19
                script: 76a9147d576fbfca48b899dc750167dd2a2a6572fff49588ac
                pool-reward: 4950000000 packInt64LE: 80010b2701000000
                lenght: 25 varIntBuffer: 19
                script: 76a914329035234168b8da5af106ceb20560401236849888ac
                txOutputBuffers.lenght : 2 varIntBuffer: 02
                -- p2 --
                scriptSigPart2: 0d2f6e6f64655374726174756d2f
                txInSequence: 0 packUInt32LE: 00000000
                outputTransactions: 0280010b27010000001976a914329035234168b8da5af106ceb20560401236849888ac80f0fa02000000001976a9147d576fbfca48b899dc750167dd2a2a6572fff49588ac
                txLockTime: 0 packUInt32LE: 00000000
                txComment: 
                p2: 0d2f6e6f64655374726174756d2f000000000280010b27010000001976a914329035234168b8da5af106ceb20560401236849888ac80f0fa02000000001976a9147d576fbfca48b899dc750167dd2a2a6572fff49588ac00000000
            */

            // block template json
            const string json = "{\"result\":{\"version\":2,\"previousblockhash\":\"e9bbcc9b46ed98fd4850f2d21e85566defdefad3453460caabc7a635fc5a1261\",\"transactions\":[],\"coinbaseaux\":{\"flags\":\"062f503253482f\"},\"coinbasevalue\":5000000000,\"target\":\"0000004701b20000000000000000000000000000000000000000000000000000\",\"mintime\":1402660580,\"mutable\":[\"time\",\"transactions\",\"prevblock\"],\"noncerange\":\"00000000ffffffff\",\"sigoplimit\":20000,\"sizelimit\":1000000,\"curtime\":1402661060,\"bits\":\"1d4701b2\",\"height\":302526},\"error\":null,\"id\":1}";

            // now init blocktemplate from our json.
            var @object = JsonConvert.DeserializeObject<DaemonResponse<BlockTemplate>>(json);
            var blockTemplate = @object.Result;

            // init mockup objects
            var daemonClient = Substitute.For<IDaemonClient>();
            daemonClient.ValidateAddress(Arg.Any<string>()).Returns(new ValidateAddress {IsValid = true});

            var extraNonce = new ExtraNonce(0);

            // create the test object.
            var generationTransaction = new GenerationTransaction(extraNonce, daemonClient, blockTemplate);

            // use the exactly same input script data within our sample data.
            generationTransaction.Inputs.First().SignatureScript = new SignatureScript(
                blockTemplate.Height,
                blockTemplate.CoinBaseAux.Flags,
                1402661059432,
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

            // test the signature script initial
            generationTransaction.Initial.Skip(42).Take(17).Should().Equal(new byte[]
            {
                0x03, 0xbe, 0x9d, 0x04, 0x06, 0x2f, 0x50, 0x32, 0x53, 0x48, 0x2f, 0x04, 0xc3, 0xe8, 0x9a, 0x53,
                0x08                    
            });

            // test the generation transactions initial part.
            generationTransaction.Initial.Should().Equal(new byte[]
            {
                0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0xff, 0xff, 0x27, 0x03, 0xbe, 0x9d, 
                0x04, 0x06, 0x2f, 0x50, 0x32, 0x53, 0x48, 0x2f, 0x04, 0xc3, 0xe8, 0x9a, 0x53, 0x08
            });

            // test the signature script final
            generationTransaction.Final.Take(14).Should().Equal(new byte[]
            {
                0x0d, 0x2f, 0x6e, 0x6f, 0x64, 0x65, 0x53, 0x74, 0x72, 0x61, 0x74, 0x75, 0x6d, 0x2f
            });
        }
    }
}
