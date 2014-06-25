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
using System.Numerics;
using Coinium.Coin.Algorithms;
using Coinium.Coin.Daemon;
using Coinium.Coin.Daemon.Responses;
using Coinium.Common.Extensions;
using Coinium.Crypto;
using Coinium.Mining.Jobs;
using Coinium.Mining.Shares;
using Coinium.Server.Stratum.Notifications;
using Coinium.Transactions;
using Coinium.Transactions.Script;
using Newtonsoft.Json;
using NSubstitute;
using Should.Fluent;
using Xunit;

namespace Tests.Mining.Shares
{
    public class ShareTests
    {
        // object mocks.
        private readonly IDaemonClient _daemonClient;
        private readonly IBlockTemplate _blockTemplate;
        private readonly IExtraNonce _extraNonce;
        private readonly IMerkleTree _merkleTree;
        private readonly IJobManager _jobManager;
        private readonly IHashAlgorithm _hashAlgorithm;
        private readonly ISignatureScript _signatureScript;
        private readonly IOutputs _outputs;
        private readonly IGenerationTransaction _generationTransaction;      
        private readonly IJob _job;

        public ShareTests()
        {
            /*
                -- create-generation start --
                rpcData: {"version":2,"previousblockhash":"1c4eb88e47564cb796b5c6648c74bec51d7215ac12fc4168b14827aac74a8062","transactions":[{"data":"010000000332a82e92f522deee69b09e27858ba9b87585f2a4913ef71018df40909032fdc3000000006a473044022019ca05cb880a04f0d842268b7e75ac6d2695fc544df033e3daeb29239251a8970220031f6336767f2ea617347484e1290ec0bdcc71056ea2d3084e75384905250ec50121030dd394118fb66ca288bff71d8ea762678783b005770f7f9ba4128233191e0847ffffffff086747cbd339b21b950774186091653a7b8f5751b00a906ff6f5561b3a6fcee6010000006b4830450221009ae1ba9a216d313cc592fc2c1ef08f1e0e555a32b6c1b305f685ac882d38356b0220243106bbb5bb76dde142e574cba8f30c1e2f7059e8e9161770396fbd2b50420f0121030dd394118fb66ca288bff71d8ea762678783b005770f7f9ba4128233191e0847ffffffffe2f15804b1e41c36c925c6f64f219b2bdb3c9fbff4c97a4f0e8c7f31d7e6f2af000000006b48304502200be8894fdd7f5c19be248a979c08bbf2395f606e038c3e02c0266474c03699ab022100ff5de87086e487410f5d7b68012655ca6d814f0caeb9ca42d9c425a90f68b3030121030dd394118fb66ca288bff71d8ea762678783b005770f7f9ba4128233191e0847ffffffff02a0f01900000000001976a9141c50209a1dfdf53313d237b75e9aeb553ca1dfda88ac00e1f505000000001976a914cbb9a3e7a7c1651b1006f876f08b40be85b274f588ac00000000","hash":"dc3a80ec6c45aa489453b2c4abf6761eb6656d949e26d01793458c166640e5f3","depends":[],"fee":0,"sigops":2}],"coinbaseaux":{"flags":"062f503253482f"},"coinbasevalue":5000000000,"target":"00000048d4f70000000000000000000000000000000000000000000000000000","mintime":1403691059,"mutable":["time","transactions","prevblock"],"noncerange":"00000000ffffffff","sigoplimit":20000,"sizelimit":1000000,"curtime":1403691825,"bits":"1d48d4f7","height":315152}
             
                -- scriptSigPart data --
                -> height: 315152 serialized: 0310cf04
                -> coinbase: 062f503253482f hex: 062f503253482f
                -> date: 1403691824760 final:1403691824 serialized: 0430a3aa53
                -- p1 data --
                txVersion: 1 packed: 01000000
                txInputsCount: 1 varIntBuffer: 01
                txInPrevOutHash: 0 uint256BufferFromHash: 0000000000000000000000000000000000000000000000000000000000000000
                txInPrevOutIndex: 4294967295 packUInt32LE: ffffffff
                scriptSigPart1.length: 17 extraNoncePlaceholder.length:8 scriptSigPart2.length:14 all: 39 varIntBuffer: 27
                scriptSigPart1: 0310cf04062f503253482f0430a3aa5308
                p1: 01000000010000000000000000000000000000000000000000000000000000000000000000ffffffff270310cf04062f503253482f0430a3aa5308
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

                getJobParams: ["2","c74a8062b14827aa12fc41681d7215ac8c74bec596b5c66447564cb71c4eb88e","01000000010000000000000000000000000000000000000000000000000000000000000000ffffffff270310cf04062f503253482f0430a3aa5308","0d2f6e6f64655374726174756d2f000000000280010b27010000001976a914329035234168b8da5af106ceb20560401236849888ac80f0fa02000000001976a9147d576fbfca48b899dc750167dd2a2a6572fff49588ac00000000",["f3e54066168c459317d0269e946d65b61e76f6abc4b2539448aa456cec803adc"],"00000002","1d48d4f7","53aaa331",true]
             */

            // daemon client
            _daemonClient = Substitute.For<IDaemonClient>();
            _daemonClient.ValidateAddress(Arg.Any<string>()).Returns(new ValidateAddress { IsValid = true });

            // block template
            const string json = "{\"result\":{\"version\":2,\"previousblockhash\":\"1c4eb88e47564cb796b5c6648c74bec51d7215ac12fc4168b14827aac74a8062\",\"transactions\":[{\"data\":\"010000000332a82e92f522deee69b09e27858ba9b87585f2a4913ef71018df40909032fdc3000000006a473044022019ca05cb880a04f0d842268b7e75ac6d2695fc544df033e3daeb29239251a8970220031f6336767f2ea617347484e1290ec0bdcc71056ea2d3084e75384905250ec50121030dd394118fb66ca288bff71d8ea762678783b005770f7f9ba4128233191e0847ffffffff086747cbd339b21b950774186091653a7b8f5751b00a906ff6f5561b3a6fcee6010000006b4830450221009ae1ba9a216d313cc592fc2c1ef08f1e0e555a32b6c1b305f685ac882d38356b0220243106bbb5bb76dde142e574cba8f30c1e2f7059e8e9161770396fbd2b50420f0121030dd394118fb66ca288bff71d8ea762678783b005770f7f9ba4128233191e0847ffffffffe2f15804b1e41c36c925c6f64f219b2bdb3c9fbff4c97a4f0e8c7f31d7e6f2af000000006b48304502200be8894fdd7f5c19be248a979c08bbf2395f606e038c3e02c0266474c03699ab022100ff5de87086e487410f5d7b68012655ca6d814f0caeb9ca42d9c425a90f68b3030121030dd394118fb66ca288bff71d8ea762678783b005770f7f9ba4128233191e0847ffffffff02a0f01900000000001976a9141c50209a1dfdf53313d237b75e9aeb553ca1dfda88ac00e1f505000000001976a914cbb9a3e7a7c1651b1006f876f08b40be85b274f588ac00000000\",\"hash\":\"dc3a80ec6c45aa489453b2c4abf6761eb6656d949e26d01793458c166640e5f3\",\"depends\":[],\"fee\":0,\"sigops\":2}],\"coinbaseaux\":{\"flags\":\"062f503253482f\"},\"coinbasevalue\":5000000000,\"target\":\"00000048d4f70000000000000000000000000000000000000000000000000000\",\"mintime\":1403691059,\"mutable\":[\"time\",\"transactions\",\"prevblock\"],\"noncerange\":\"00000000ffffffff\",\"sigoplimit\":20000,\"sizelimit\":1000000,\"curtime\":1403691825,\"bits\":\"1d48d4f7\",\"height\":315152},\"error\":null,\"id\":1}";
            var blockTemplateObject = JsonConvert.DeserializeObject<DaemonResponse<BlockTemplate>>(json);
            _blockTemplate = blockTemplateObject.Result;

            // extra nonce
            _extraNonce = Substitute.For<ExtraNonce>((UInt32)0);

            // merkle tree
            var hashList = _blockTemplate.Transactions.Select(transaction => transaction.Hash.HexToByteArray()).ToList();
            _merkleTree = Substitute.For<MerkleTree>(hashList);

            // signature script
            _signatureScript = Substitute.For<SignatureScript>(
                _blockTemplate.Height,
                _blockTemplate.CoinBaseAux.Flags,
                1403691824760,
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

            // generation transaction
            _generationTransaction = Substitute.For<GenerationTransaction>(_extraNonce, _daemonClient, _blockTemplate, false);
            _generationTransaction.Inputs.First().SignatureScript = _signatureScript;
            _generationTransaction.Outputs = _outputs;
            _generationTransaction.Create();

            // hash algorithm
            _hashAlgorithm = Substitute.For<Scrypt>();

            // the job.
            _job = new Job(2,_hashAlgorithm, _blockTemplate, _generationTransaction, _merkleTree)
            {
                CleanJobs = true
            };

            // the job manager.
            _jobManager = Substitute.For<IJobManager>();
            _jobManager.ExtraNonce.Current.Returns((UInt32)0x58000000);
            _jobManager.GetJob(2).Returns(_job);
        }

        [Fact]
        public void CandicateShareTest()
        {
            
        }

        [Fact]
        public void NonCandicateShareTest()
        {
            /* 
                handle-submit:
                ---------------
                handleSubmit: {"params":["mn4jUMneEBjZuDPEdFuj6BmFPmehmrT2Zc","2","07000000","53aaa331","87500200"],"id":118,"method":"mining.submit"}
 
                process-share:
                ---------------
                jobId: 2 previousDifficulty: undefined difficulty: 32 extraNonce1: 58000000 extraNonce2: 07000000 nTime: 53aaa331 nonce		: 87500200 ipAddress: 10.0.0.40 port: 3333 workerName: mn4jUMneEBjZuDPEdFuj6BmFPmehmrT2Zc
                nTimeInt: 1403691825
                coinbaseBuffer: 01000000010000000000000000000000000000000000000000000000000000000000000000ffffffff270310cf04062f503253482f0430a3aa530858000000070000000d2f6e6f64655374726174756d2f000000000280010b27010000001976a914329035234168b8da5af106ceb20560401236849888ac80f0fa02000000001976a9147d576fbfca48b899dc750167dd2a2a6572fff49588ac00000000
                coinbaseHash: 76a3f30f9dfdb980bf08a153f097c6456d5e0d290a41f760ce380c4b9c73f5d0
                merkleRoot: 7875fb5effb2f631634523f777090ba1568ec4c4ceee35a9b1c6832d24a23217
                headerBuffer: 0200000062804ac7aa2748b16841fc12ac15721dc5be748c64c6b596b74c56478eb84e1c1732a2242d83c6b1a935eecec4c48e56a10b0977f723456331f6b2ff5efb757831a3aa53f7d4481d00025087
                headerHash: 83011ba79aa48eefd2fccef32ebe190f538c1d188fecf764d2aca2259c060000
                headerBigNum: 45620193236259201373579162993315268294283759910880625924204234612081027
                shareDiff: 38.72890444987063 diff1: 2.695953529101131e+67 shareMultiplier: 65536
                blockDiffAdjusted : 899.811835904 job.difficulty: 0.013730039
                job.target: 1963543975774994773269086777481374456547162842587540503781935641788416
                candicate: false
                blockHashInvalid: 87776a72cedf7467ef78c0dc8a7181340342888e33bd19e6ff48d579299d38c1
                shareDiff / difficulty: 1.2102782640584573
                2014-06-25 13:23:45 [Pool]	[litecoin] (Thread 1) Share accepted at diff 32/38.72890445 by mn4jUMneEBjZuDPEdFuj6BmFPmehmrT2Zc [10.0.0.40]
                emit: {"job":"2","ip":"10.0.0.40","port":3333,"worker":"mn4jUMneEBjZuDPEdFuj6BmFPmehmrT2Zc","height":315152,"blockReward":5000000000,"difficulty":32,"shareDiff":"38.72890445","blockDiff":899.811835904,"blockDiffActual":0.013730039,"blockHashInvalid":"87776a72cedf7467ef78c0dc8a7181340342888e33bd19e6ff48d579299d38c1"} blockHex: undefined
             */

            // submitted share json
            const string shareJson = "{\"params\":[\"mn4jUMneEBjZuDPEdFuj6BmFPmehmrT2Zc\",\"2\",\"07000000\",\"53aaa331\",\"87500200\"],\"id\":118,\"method\":\"mining.submit\"}";
            dynamic shareObject = JsonConvert.DeserializeObject(shareJson);
            dynamic shareData = shareObject.@params;

            string minerName = shareData[0];
            string jobId = shareData[1];
            string extraNonce2 = shareData[2];
            string nTime = shareData[3];
            string nonce = shareData[4];

            minerName.Should().Equal("mn4jUMneEBjZuDPEdFuj6BmFPmehmrT2Zc");
            jobId.Should().Equal("2");
            extraNonce2.Should().Equal("07000000");
            nTime.Should().Equal("53aaa331");
            nonce.Should().Equal("87500200");

            // create the share
            var id = Convert.ToUInt64(jobId, 16);
            var job = _jobManager.GetJob(id);

            var share = new Share(id, job, _jobManager.ExtraNonce.Current, extraNonce2, nTime, nonce);

            // test miner provided nonce and ntime
            share.nTime.Should().Equal((UInt32)0x53aaa331);
            share.Nonce.Should().Equal((UInt32)0x87500200);

            // test job provided extraNonce1 and extraNonce2
            share.ExtraNonce1.Should().Equal((UInt32)0x58000000);
            share.ExtraNonce2.Should().Equal((UInt32)0x07000000);

            // test coinbase
            share.CoinbaseBuffer.ToHexString().Should().Equal("01000000010000000000000000000000000000000000000000000000000000000000000000ffffffff270310cf04062f503253482f0430a3aa530858000000070000000d2f6e6f64655374726174756d2f000000000280010b27010000001976a914329035234168b8da5af106ceb20560401236849888ac80f0fa02000000001976a9147d576fbfca48b899dc750167dd2a2a6572fff49588ac00000000");
            share.CoinbaseHash.Bytes.ToHexString().Should().Equal("76a3f30f9dfdb980bf08a153f097c6456d5e0d290a41f760ce380c4b9c73f5d0");

            // test merkle-root.
            share.MerkleRoot.ToHexString().Should().Equal("7875fb5effb2f631634523f777090ba1568ec4c4ceee35a9b1c6832d24a23217");

            // test the block header
            share.HeaderBuffer.ToHexString().Should().Equal("0200000062804ac7aa2748b16841fc12ac15721dc5be748c64c6b596b74c56478eb84e1c1732a2242d83c6b1a935eecec4c48e56a10b0977f723456331f6b2ff5efb757831a3aa53f7d4481d00025087");

            // test the block hash.
            share.HeaderHash.ToHexString().Should().Equal("83011ba79aa48eefd2fccef32ebe190f538c1d188fecf764d2aca2259c060000");
            share.HeaderValue.Should().Equal(BigInteger.Parse("45620193236259201373579162993315268294283759910880625924204234612081027"));

            // test the difficulty
            share.Difficulty.Should().Equal(549.396764174295);
            share.BlockDiffAdjusted.Should().Equal(256);
            share.Job.Difficulty.Should().Equal(0.00390625);

            // check the share itself.
            share.Valid.Should().Equal(true);
            share.Candicate.Should().Equal(false);
        }
    }
}
