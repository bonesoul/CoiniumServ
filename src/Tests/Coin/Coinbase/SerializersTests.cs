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
using System.Linq;
using System.Text;
using Coinium.Coin.Coinbase;
using Coinium.Coin.Daemon;
using Coinium.Coin.Daemon.Responses;
using Coinium.Common.Extensions;
using Coinium.Crypto;
using Coinium.Mining.Jobs;
using Coinium.Server.Stratum.Notifications;
using Coinium.Transactions;
using Coinium.Transactions.Script;
using Gibbed.IO;
using Newtonsoft.Json;
using NSubstitute;
using Should.Fluent;
using Xunit;

/* Sample data

    block template:
    ---------------
    rpcData: {"version":2,"previousblockhash":"1a47638fd58c3b90cc3b2a7f1973dcdf545be4474d2157af28ad6ce7767acb09","transactions":[],"coinbaseaux":{"flags":"062f503253482f"},"coinbasevalue":5000000000,"target":"000000ffff000000000000000000000000000000000000000000000000000000","mintime":1403563551,"mutable":["time","transactions","prevblock"],"noncerange":"00000000ffffffff","sigoplimit":20000,"sizelimit":1000000,"curtime":1403563962,"bits":"1e00ffff","height":313498}
 
    generation transaction:
    --------------- 
    -- scriptSigPart data --
    -> height: 313498 serialized: 039ac804
    -> coinbase: 062f503253482f hex: 062f503253482f
    -> date: 1403563961807 final:1403563961 serialized: 04b9afa853
    -- p1 data --
    txVersion: 1 packed: 01000000
    txInputsCount: 1 varIntBuffer: 01
    txInPrevOutHash: 0 uint256BufferFromHash: 0000000000000000000000000000000000000000000000000000000000000000
    txInPrevOutIndex: 4294967295 packUInt32LE: ffffffff
    scriptSigPart1.length: 17 extraNoncePlaceholder.length:8 scriptSigPart2.length:14 all: 39 varIntBuffer: 27
    scriptSigPart1: 039ac804062f503253482f04b9afa85308
    p1: 01000000010000000000000000000000000000000000000000000000000000000000000000ffffffff27039ac804062f503253482f04b9afa85308
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

    job-params:
    ---------------
    getJobParams: ["1","767acb0928ad6ce74d2157af545be4471973dcdfcc3b2a7fd58c3b901a47638f","01000000010000000000000000000000000000000000000000000000000000000000000000ffffffff27039ac804062f503253482f04b9afa85308","0d2f6e6f64655374726174756d2f000000000280010b27010000001976a914329035234168b8da5af106ceb20560401236849888ac80f0fa02000000001976a9147d576fbfca48b899dc750167dd2a2a6572fff49588ac00000000",[],"00000002","1e00ffff","53a8afba",true]
 
    process-share:
    ---------------
    jobId: 1 previousDifficulty: undefined difficulty: 32 extraNonce1: 70000000 extraNonce2: 00000000 nTime: 53a8afba nonce: 8c6b0c00 ipAddress: 10.0.0.40 port: 3333 workerName: mn4jUMneEBjZuDPEdFuj6BmFPmehmrT2Zc
    nTimeInt: 1403563962
    coinbaseBuffer: 01000000010000000000000000000000000000000000000000000000000000000000000000ffffffff27039ac804062f503253482f04b9afa8530870000000000000000d2f6e6f64655374726174756d2f000000000280010b27010000001976a914329035234168b8da5af106ceb20560401236849888ac80f0fa02000000001976a9147d576fbfca48b899dc750167dd2a2a6572fff49588ac00000000
    coinbaseHash: dcbac3aae04bb6893d22b39426da75473c6d1e23eb3acd701ff682a6a1fecd76
    merkleRoot: 76cdfea1a682f61f70cd3aeb231e6d3c4775da2694b3223d89b64be0aac3badc
    headerBuffer: 0200000009cb7a76e76cad28af57214d47e45b54dfdc73197f2a3bcc903b8cd58f63471adcbac3aae04bb6893d22b39426da75473c6d1e23eb3acd701ff682a6a1fecd76baafa853ffff001e000c6b8c
    headerHash: c271dc00d2389083bf547a905a8d441ee9c710c6a87edfd35d7c8cafbe030000
    headerBigNum: 25846116350681099734171790912699060475203659552966485851836826877981122
    shareDiff: 68.35921036876233 diff1: 2.695953529101131e+67 shareMultiplier: 65536
    blockDiffAdjusted : 256 job.difficulty: 0.00390625
    2014-06-24 01:52:52 [Pool]	[litecoin] (Thread 1) Share accepted at diff 32/68.35921037 by mn4jUMneEBjZuDPEdFuj6BmFPmehmrT2Zc [10.0.0.40]
    emit: {"job":"1","ip":"10.0.0.40","port":3333,"worker":"mn4jUMneEBjZuDPEdFuj6BmFPmehmrT2Zc","height":313498,"blockReward":5000000000,"difficulty":32,"shareDiff":"68.35921037","blockDiff":256,"blockDiffActual":0.00390625}
 */

namespace Tests.Coin.Coinbase
{
    public class SerializerTests
    {
        // object mocks.
        private readonly IDaemonClient _daemonClient;
        private readonly IBlockTemplate _blockTemplate;
        private readonly IExtraNonce _extraNonce;
        private readonly IMerkleTree _merkleTree;
        private readonly ISignatureScript _signatureScript;
        private readonly IOutputs _outputs;
        private readonly IJobCounter _jobCounter;
        private readonly IGenerationTransaction _generationTransaction;
        private readonly IJob _job;

        public SerializerTests()
        {
            // daemon client
            _daemonClient = Substitute.For<IDaemonClient>();
            _daemonClient.ValidateAddress(Arg.Any<string>()).Returns(new ValidateAddress { IsValid = true });

            // block template
            const string json = "{\"result\":{\"version\":2,\"previousblockhash\":\"1a47638fd58c3b90cc3b2a7f1973dcdf545be4474d2157af28ad6ce7767acb09\",\"transactions\":[],\"coinbaseaux\":{\"flags\":\"062f503253482f\"},\"coinbasevalue\":5000000000,\"target\":\"000000ffff000000000000000000000000000000000000000000000000000000\",\"mintime\":1403563551,\"mutable\":[\"time\",\"transactions\",\"prevblock\"],\"noncerange\":\"00000000ffffffff\",\"sigoplimit\":20000,\"sizelimit\":1000000,\"curtime\":1403563962,\"bits\":\"1e00ffff\",\"height\":313498},\"error\":null,\"id\":1}";
            var @object = JsonConvert.DeserializeObject<DaemonResponse<BlockTemplate>>(json);
            _blockTemplate = @object.Result;

            // extra nonce
            _extraNonce = Substitute.For<ExtraNonce>((UInt32)0);

            // merkle tree
            var hashList = _blockTemplate.Transactions.Select(transaction => transaction.Hash.HexToByteArray()).ToList();
            _merkleTree = Substitute.For<MerkleTree>(hashList);                

            // signature script
            _signatureScript = Substitute.For<SignatureScript>(
                _blockTemplate.Height,
                _blockTemplate.CoinBaseAux.Flags,
                1403563961807,
                (byte)_extraNonce.ExtraNoncePlaceholder.Length,
                "/nodeStratum/");

            // outputs
            _outputs = Substitute.For<Outputs>(_daemonClient);
            double blockReward = 5000000000; // the amount rewarded by the block.

            // sample recipient
            const string recipient = "mrwhWEDnU6dUtHZJ2oBswTpEdbBHgYiMji";
            var amount = blockReward * 0.01;
            blockReward -= amount;
            _outputs.AddRecipient(recipient, amount);

            // sample pool wallet
            const string poolWallet = "mk8JqN1kNWju8o3DXEijiJyn7iqkwktAWq";
            _outputs.AddPool(poolWallet, blockReward);

            // generation transaction.
            _generationTransaction = Substitute.For<GenerationTransaction>(_extraNonce, _daemonClient, _blockTemplate, false);
            _generationTransaction.Inputs.First().SignatureScript = _signatureScript;
            _generationTransaction.Outputs = _outputs;
            _generationTransaction.Create();

            // job counter
            _jobCounter = Substitute.For<JobCounter>();

            // create the job
            _job = new Job(_jobCounter.Next(), _blockTemplate, _generationTransaction, _merkleTree)
            {
                CleanJobs = true
            };
        }

        [Fact]
        public void CoinbaseSerializerTest()
        {
            const UInt32 extraNonce1 = 0x70000000;
            const UInt32 extraNonce2 = 0x00000000;
            var coinbase = Serializers.SerializeCoinbase(_job, extraNonce1, extraNonce2);

            coinbase.ToHexString().Should().Equal("01000000010000000000000000000000000000000000000000000000000000000000000000ffffffff27039ac804062f503253482f04b9afa8530870000000000000000d2f6e6f64655374726174756d2f000000000280010b27010000001976a914329035234168b8da5af106ceb20560401236849888ac80f0fa02000000001976a9147d576fbfca48b899dc750167dd2a2a6572fff49588ac00000000");
        }
    }
}
