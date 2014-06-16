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
using Coinium.Coin.Daemon;
using Coinium.Coin.Daemon.Responses;
using Coinium.Crypto;
using Coinium.Mining.Jobs;
using Coinium.Server.Stratum.Notifications;
using Coinium.Transactions;
using Should.Fluent;
using Xunit;
using Newtonsoft.Json;
using NSubstitute;


namespace Tests.Server.Stratum.Notifications
{
    public class JobTests
    {
        [Fact]
        public void TestJob()
        {
            /* sample data
                previousblockhash: 22a9174d9db64f1919febc9577167764c301b755768b675291f7d34454561e9e previousblockhashreversed: 54561e9e91f7d344768b6752c301b7557716776419febc959db64f1922a9174d
                -- create-generation start --
                rpcData: {"version":2,"previousblockhash":"22a9174d9db64f1919febc9577167764c301b755768b675291f7d34454561e9e","transactions":[],"coinbaseaux":{"flags":"062f503253482f"},"coinbasevalue":5000000000,"target":"0000002bd7c30000000000000000000000000000000000000000000000000000","mintime":1402922277,"mutable":["time","transactions","prevblock"],"noncerange":"00000000ffffffff","sigoplimit":20000,"sizelimit":1000000,"curtime":1402922598,"bits":"1d2bd7c3","height":305349}
                -- scriptSigPart data --
                -> height: 305349 serialized: 03c5a804
                -> coinbase: 062f503253482f hex: 062f503253482f
                -> date: 1402922597281 final:1402922597 serialized: 0465e69e53
                -- p1 data --
                txVersion: 1 packed: 01000000
                txInputsCount: 1 varIntBuffer: 01
                txInPrevOutHash: 0 uint256BufferFromHash: 0000000000000000000000000000000000000000000000000000000000000000
                txInPrevOutIndex: 4294967295 packUInt32LE: ffffffff
                scriptSigPart1.length: 17 extraNoncePlaceholder.length:8 scriptSigPart2.length:14 all: 39 varIntBuffer: 27
                scriptSigPart1: 03c5a804062f503253482f0465e69e5308
                p1: 01000000010000000000000000000000000000000000000000000000000000000000000000ffffffff2703c5a804062f503253482f0465e69e5308
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
                getJobParams: ["1","54561e9e91f7d344768b6752c301b7557716776419febc959db64f1922a9174d","01000000010000000000000000000000000000000000000000000000000000000000000000ffffffff2703c5a804062f503253482f0465e69e5308","0d2f6e6f64655374726174756d2f000000000280010b27010000001976a914329035234168b8da5af106ceb20560401236849888ac80f0fa02000000001976a9147d576fbfca48b899dc750167dd2a2a6572fff49588ac00000000",[],"00000002","1d2bd7c3","539ee666",true]
             */

            // block template json
            const string json = "{\"result\":{\"version\":2,\"previousblockhash\":\"ea79cc9640f6c7d471c441a8de8cc737179943f00a9f43535a4a8b1cdbf1c815\",\"transactions\":[],\"coinbaseaux\":{\"flags\":\"062f503253482f\"},\"coinbasevalue\":5000000000,\"target\":\"0000002bd7c30000000000000000000000000000000000000000000000000000\",\"mintime\":1402920882,\"mutable\":[\"time\",\"transactions\",\"prevblock\"],\"noncerange\":\"00000000ffffffff\",\"sigoplimit\":20000,\"sizelimit\":1000000,\"curtime\":1402921730,\"bits\":\"1d2bd7c3\",\"height\":305339},\"error\":null,\"id\":1}";

            // now init blocktemplate from our json.
            var @object = JsonConvert.DeserializeObject<DaemonResponse<BlockTemplate>>(json);
            var blockTemplate = @object.Result;

            // set up mock objects.
            var jobCounter = Substitute.For<JobCounter>();
            var extraNonce = Substitute.For<IExtraNonce>();
            var merkleTree = Substitute.For<MerkleTree>(new List<byte[]>());

            var daemonClient = Substitute.For<IDaemonClient>();
            daemonClient.ValidateAddress(Arg.Any<string>()).Returns(new ValidateAddress { IsValid = true });

            // create the transaction.
            var transaction = Substitute.For<GenerationTransaction>(extraNonce, daemonClient, blockTemplate, false);
            transaction.Create();             

            // test the output.
            var job = new Job(jobCounter.Next(), blockTemplate, transaction, merkleTree);

            job.Id.Should().Equal((UInt64)1);
            job.PreviousBlockHashReversed.Should().Equal("dbf1c8155a4a8b1c0a9f4353179943f0de8cc73771c441a840f6c7d4ea79cc96");
        }
    }
}
