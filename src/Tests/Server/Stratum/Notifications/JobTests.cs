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
using Coinium.Common.Extensions;
using Coinium.Crypto;
using Coinium.Mining.Jobs;
using Coinium.Server.Stratum.Notifications;
using Coinium.Transactions;
using Coinium.Transactions.Script;
using Should.Fluent;
using Xunit;
using Newtonsoft.Json;
using NSubstitute;

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
    getJobParams: 
    [
        "1",
        "54561e9e91f7d344768b6752c301b7557716776419febc959db64f1922a9174d",
        "01000000010000000000000000000000000000000000000000000000000000000000000000ffffffff2703c5a804062f503253482f0465e69e5308",
        "0d2f6e6f64655374726174756d2f000000000280010b27010000001976a914329035234168b8da5af106ceb20560401236849888ac80f0fa02000000001976a9147d576fbfca48b899dc750167dd2a2a6572fff49588ac00000000",
        [],
        "00000002",
        "1d2bd7c3",
        "539ee666",
        true
    ]
 */

namespace Tests.Server.Stratum.Notifications
{
    public class JobTests
    {
        // object mocks.
        private readonly IDaemonClient _daemonClient;
        private readonly IBlockTemplate _blockTemplate;
        private readonly IExtraNonce _extraNonce;
        private readonly IMerkleTree _merkleTree;
        private readonly ISignatureScript _signatureScript;
        private readonly IOutputs _outputs;

        public JobTests()
        {
            // daemon client
            _daemonClient = Substitute.For<IDaemonClient>();
            _daemonClient.ValidateAddress(Arg.Any<string>()).Returns(new ValidateAddress { IsValid = true });

            // block template
            const string json = "{\"result\":{\"version\":2,\"previousblockhash\":\"22a9174d9db64f1919febc9577167764c301b755768b675291f7d34454561e9e\",\"transactions\":[],\"coinbaseaux\":{\"flags\":\"062f503253482f\"},\"coinbasevalue\":5000000000,\"target\":\"0000002bd7c30000000000000000000000000000000000000000000000000000\",\"mintime\":1402922277,\"mutable\":[\"time\",\"transactions\",\"prevblock\"],\"noncerange\":\"00000000ffffffff\",\"sigoplimit\":20000,\"sizelimit\":1000000,\"curtime\":1402922598,\"bits\":\"1d2bd7c3\",\"height\":305349},\"error\":null,\"id\":1}";
            var @object = JsonConvert.DeserializeObject<DaemonResponse<BlockTemplate>>(json);
            _blockTemplate = @object.Result;

            // extra nonce
            _extraNonce = new ExtraNonce(0);

            // merkle tree
            var hashList = _blockTemplate.Transactions.Select(transaction => transaction.Hash.HexToByteArray()).ToList();
            _merkleTree = new MerkleTree(hashList);

            // signature script
            _signatureScript = new SignatureScript(
                _blockTemplate.Height,
                _blockTemplate.CoinBaseAux.Flags,
                1402922597281,
                (byte) _extraNonce.ExtraNoncePlaceholder.Length,
                "/nodeStratum/");

            // outputs
            _outputs = new Outputs(_daemonClient);
            double blockReward = 5000000000; // the amount rewarded by the block.

            // sample recipient
            const string recipient = "mrwhWEDnU6dUtHZJ2oBswTpEdbBHgYiMji";
            var amount = blockReward * 0.01;
            blockReward -= amount;
            _outputs.AddRecipient(recipient, amount);

            // sample pool wallet
            const string poolWallet = "mk8JqN1kNWju8o3DXEijiJyn7iqkwktAWq";
            _outputs.AddPool(poolWallet, blockReward);
        }

        [Fact]
        public void TestJob()
        {
            var jobCounter = Substitute.For<JobCounter>();

            // create the transaction.
            var generationTransaction = new GenerationTransaction(_extraNonce, _daemonClient, _blockTemplate);
            generationTransaction.Inputs.First().SignatureScript = _signatureScript;
            generationTransaction.Outputs = _outputs;
            generationTransaction.Create();             

            // test the output.
            var job = new Job(jobCounter.Next(), _blockTemplate, generationTransaction, _merkleTree)
            {
                CleanJobs = true
            };

            // test previous block hash reversed.
            job.Id.Should().Equal((UInt64)1);
            job.PreviousBlockHashReversed.Should().Equal("54561e9e91f7d344768b6752c301b7557716776419febc959db64f1922a9174d");

            // test the Coinbase (generation transaction).
            job.CoinbaseInitial.Should().Equal("01000000010000000000000000000000000000000000000000000000000000000000000000ffffffff2703c5a804062f503253482f0465e69e5308");
            job.CoinbaseFinal.Should().Equal("0d2f6e6f64655374726174756d2f000000000280010b27010000001976a914329035234168b8da5af106ceb20560401236849888ac80f0fa02000000001976a9147d576fbfca48b899dc750167dd2a2a6572fff49588ac00000000");

            // test the merkle branch
            job.MerkleTree.Branches.Count.Should().Equal(0);

            // test the version.
            job.Version.Should().Equal("00000002");

            // test the bits (encoded network difficulty)
            job.NetworkDifficulty.Should().Equal("1d2bd7c3");

            // test the current time
            job.nTime.Should().Equal("539ee666");

            // test the clean jobs flag
            job.CleanJobs.Should().Equal(true);

            // test the json
            var jobJson = JsonConvert.SerializeObject(job);
            jobJson.Should().Equal("[\"1\",\"54561e9e91f7d344768b6752c301b7557716776419febc959db64f1922a9174d\",\"01000000010000000000000000000000000000000000000000000000000000000000000000ffffffff2703c5a804062f503253482f0465e69e5308\",\"0d2f6e6f64655374726174756d2f000000000280010b27010000001976a914329035234168b8da5af106ceb20560401236849888ac80f0fa02000000001976a9147d576fbfca48b899dc750167dd2a2a6572fff49588ac00000000\",[],\"00000002\",\"1d2bd7c3\",\"539ee666\",true]");
        }
    }
}
