#region License
// 
//     CoiniumServ - Crypto Currency Mining Pool Server Software
//     Copyright (C) 2013 - 2014, CoiniumServ Project - http://www.coinium.org
//     http://www.coiniumserv.com - https://github.com/CoiniumServ/CoiniumServ
// 
//     This software is dual-licensed: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
//    
//     For the terms of this license, see licenses/gpl_v3.txt.
// 
//     Alternatively, you can license this software under a commercial
//     license or white-label it as set out in licenses/commercial.txt.
// 
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using CoiniumServ.Cryptology.Algorithms;
using CoiniumServ.Daemon;
using CoiniumServ.Daemon.Responses;
using CoiniumServ.Jobs;
using CoiniumServ.Payments;
using CoiniumServ.Server.Mining.Stratum.Notifications;
using CoiniumServ.Transactions;
using CoiniumServ.Transactions.Script;
using Newtonsoft.Json;
using NSubstitute;
using Should.Fluent;
using Xunit;

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

namespace CoiniumServ.Tests.Server.Stratum.Notifications
{
    public class JobTests
    {
        // object mocks.
        private readonly IDaemonClient _daemonClient;
        private readonly IBlockTemplate _blockTemplate;
        private readonly IExtraNonce _extraNonce;
        private readonly ISignatureScript _signatureScript;
        private readonly IOutputs _outputs;
        private readonly IJobCounter _jobCounter;
        private readonly IHashAlgorithm _hashAlgorithm;
        private readonly IGenerationTransaction _generationTransaction;

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

            // signature script
            _signatureScript = new SignatureScript(
                _blockTemplate.Height,
                _blockTemplate.CoinBaseAux.Flags,
                1402922597281,
                (byte) _extraNonce.ExtraNoncePlaceholder.Length,
                "/nodeStratum/");

            // outputs
            _outputs = Substitute.For<Outputs>(_daemonClient);
            double blockReward = 5000000000; // the amount rewarded by the block.

            // sample reward recipient
            var rewardsConfig = Substitute.For<IRewardsConfig>();
            var amount = blockReward * 0.01;
            blockReward -= amount;
            var rewards = new Dictionary<string, float> { {"mrwhWEDnU6dUtHZJ2oBswTpEdbBHgYiMji", (float) amount} };

            rewardsConfig.GetEnumerator().Returns(rewards.GetEnumerator());
            foreach (var pair in rewards)
            {
                _outputs.AddRecipient(pair.Key, pair.Value);
            }

            // sample pool wallet
            var walletConfig = Substitute.For<IWalletConfig>();
            walletConfig.Adress.Returns("mk8JqN1kNWju8o3DXEijiJyn7iqkwktAWq");
            _outputs.AddPoolWallet(walletConfig.Adress, blockReward);

            // job counter
            _jobCounter = Substitute.For<JobCounter>();

            // generation transaction.
            _generationTransaction = new GenerationTransaction(_extraNonce, _daemonClient, _blockTemplate, walletConfig, rewardsConfig);
            _generationTransaction.Inputs.First().SignatureScript = _signatureScript;
            _generationTransaction.Outputs = _outputs;
            _generationTransaction.Create();     

            // hash algorithm
            _hashAlgorithm = Substitute.For<Scrypt>();
        }

        [Fact]
        public void TestJob()
        {
            // test the output.
            var job = new Job(_jobCounter.Next(), _hashAlgorithm, _blockTemplate, _generationTransaction)
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
            job.EncodedDifficulty.Should().Equal("1d2bd7c3");

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
