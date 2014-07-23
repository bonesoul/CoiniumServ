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
using CoiniumServ.Coin.Coinbase;
using CoiniumServ.Cryptology.Algorithms;
using CoiniumServ.Daemon;
using CoiniumServ.Daemon.Responses;
using CoiniumServ.Jobs;
using CoiniumServ.Payments;
using CoiniumServ.Server.Mining.Stratum.Notifications;
using CoiniumServ.Transactions;
using CoiniumServ.Transactions.Script;
using CoiniumServ.Utils.Extensions;
using Newtonsoft.Json;
using NSubstitute;
using Should.Fluent;
using Xunit;

/* 
    Sample data:
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
 
    handleSubmit: {"params":["mn4jUMneEBjZuDPEdFuj6BmFPmehmrT2Zc","2","07000000","53aaa331","44725000"],"id":120,"method":"mining.submit"}

    processShare: jobId: 2 previousDifficulty: undefined difficulty: 32 extraNonce1: 58000000 extraNonce2: 07000000 nTime: 53aaa331 nonce		: 44725000 ipAddress: 10.0.0.40 port: 3333 workerName: mn4jUMneEBjZuDPEdFuj6BmFPmehmrT2Zc
    nTimeInt: 1403691825
    coinbaseBuffer: 01000000010000000000000000000000000000000000000000000000000000000000000000ffffffff270310cf04062f503253482f0430a3aa530858000000070000000d2f6e6f64655374726174756d2f000000000280010b27010000001976a914329035234168b8da5af106ceb20560401236849888ac80f0fa02000000001976a9147d576fbfca48b899dc750167dd2a2a6572fff49588ac00000000
    coinbaseHash: 76a3f30f9dfdb980bf08a153f097c6456d5e0d290a41f760ce380c4b9c73f5d0
    merkleRoot: 7875fb5effb2f631634523f777090ba1568ec4c4ceee35a9b1c6832d24a23217
    headerBuffer: 0200000062804ac7aa2748b16841fc12ac15721dc5be748c64c6b596b74c56478eb84e1c1732a2242d83c6b1a935eecec4c48e56a10b0977f723456331f6b2ff5efb757831a3aa53f7d4481d00507244
    headerHash: da8b1ccf43b05a9fcea33f5ca15b0c3498a89bd764e07c7486620ccb15000000
    headerBigNum: 587542370056468750648693126335067486740543774422516057325819970489306
    shareDiff: 3007.1364975123547 diff1: 2.695953529101131e+67 shareMultiplier: 65536
    blockDiffAdjusted : 899.811835904 job.difficulty: 0.013730039
    job.target: 1963543975774994773269086777481374456547162842587540503781935641788416
    candicate: true
    blockHex: 0200000062804ac7aa2748b16841fc12ac15721dc5be748c64c6b596b74c56478eb84e1c1732a2242d83c6b1a935eecec4c48e56a10b0977f723456331f6b2ff5efb757831a3aa53f7d4481d005072440201000000010000000000000000000000000000000000000000000000000000000000000000ffffffff270310cf04062f503253482f0430a3aa530858000000070000000d2f6e6f64655374726174756d2f000000000280010b27010000001976a914329035234168b8da5af106ceb20560401236849888ac80f0fa02000000001976a9147d576fbfca48b899dc750167dd2a2a6572fff49588ac00000000010000000332a82e92f522deee69b09e27858ba9b87585f2a4913ef71018df40909032fdc3000000006a473044022019ca05cb880a04f0d842268b7e75ac6d2695fc544df033e3daeb29239251a8970220031f6336767f2ea617347484e1290ec0bdcc71056ea2d3084e75384905250ec50121030dd394118fb66ca288bff71d8ea762678783b005770f7f9ba4128233191e0847ffffffff086747cbd339b21b950774186091653a7b8f5751b00a906ff6f5561b3a6fcee6010000006b4830450221009ae1ba9a216d313cc592fc2c1ef08f1e0e555a32b6c1b305f685ac882d38356b0220243106bbb5bb76dde142e574cba8f30c1e2f7059e8e9161770396fbd2b50420f0121030dd394118fb66ca288bff71d8ea762678783b005770f7f9ba4128233191e0847ffffffffe2f15804b1e41c36c925c6f64f219b2bdb3c9fbff4c97a4f0e8c7f31d7e6f2af000000006b48304502200be8894fdd7f5c19be248a979c08bbf2395f606e038c3e02c0266474c03699ab022100ff5de87086e487410f5d7b68012655ca6d814f0caeb9ca42d9c425a90f68b3030121030dd394118fb66ca288bff71d8ea762678783b005770f7f9ba4128233191e0847ffffffff02a0f01900000000001976a9141c50209a1dfdf53313d237b75e9aeb553ca1dfda88ac00e1f505000000001976a914cbb9a3e7a7c1651b1006f876f08b40be85b274f588ac00000000
    blockHash: e242093d92f4c98bfd5dd1f9f6489652d1165f5ce4eed1f28747d2b8e3efd8b6
    emit: {"job":"2","ip":"10.0.0.40","port":3333,"worker":"mn4jUMneEBjZuDPEdFuj6BmFPmehmrT2Zc","height":315152,"blockReward":5000000000,"difficulty":32,"shareDiff":"3007.13649751","blockDiff":899.811835904,"blockDiffActual":0.013730039,"blockHash":"e242093d92f4c98bfd5dd1f9f6489652d1165f5ce4eed1f28747d2b8e3efd8b6"} blockHex: 0200000062804ac7aa2748b16841fc12ac15721dc5be748c64c6b596b74c56478eb84e1c1732a2242d83c6b1a935eecec4c48e56a10b0977f723456331f6b2ff5efb757831a3aa53f7d4481d005072440201000000010000000000000000000000000000000000000000000000000000000000000000ffffffff270310cf04062f503253482f0430a3aa530858000000070000000d2f6e6f64655374726174756d2f000000000280010b27010000001976a914329035234168b8da5af106ceb20560401236849888ac80f0fa02000000001976a9147d576fbfca48b899dc750167dd2a2a6572fff49588ac00000000010000000332a82e92f522deee69b09e27858ba9b87585f2a4913ef71018df40909032fdc3000000006a473044022019ca05cb880a04f0d842268b7e75ac6d2695fc544df033e3daeb29239251a8970220031f6336767f2ea617347484e1290ec0bdcc71056ea2d3084e75384905250ec50121030dd394118fb66ca288bff71d8ea762678783b005770f7f9ba4128233191e0847ffffffff086747cbd339b21b950774186091653a7b8f5751b00a906ff6f5561b3a6fcee6010000006b4830450221009ae1ba9a216d313cc592fc2c1ef08f1e0e555a32b6c1b305f685ac882d38356b0220243106bbb5bb76dde142e574cba8f30c1e2f7059e8e9161770396fbd2b50420f0121030dd394118fb66ca288bff71d8ea762678783b005770f7f9ba4128233191e0847ffffffffe2f15804b1e41c36c925c6f64f219b2bdb3c9fbff4c97a4f0e8c7f31d7e6f2af000000006b48304502200be8894fdd7f5c19be248a979c08bbf2395f606e038c3e02c0266474c03699ab022100ff5de87086e487410f5d7b68012655ca6d814f0caeb9ca42d9c425a90f68b3030121030dd394118fb66ca288bff71d8ea762678783b005770f7f9ba4128233191e0847ffffffff02a0f01900000000001976a9141c50209a1dfdf53313d237b75e9aeb553ca1dfda88ac00e1f505000000001976a914cbb9a3e7a7c1651b1006f876f08b40be85b274f588ac00000000
    2014-06-25 13:24:04 [Pool]	[litecoin] (Thread 1) Submitted Block using submitblock successfully to daemon instance(s)
    2014-06-25 13:24:04 [Pool]	[litecoin] (Thread 1) Block found: e242093d92f4c98bfd5dd1f9f6489652d1165f5ce4eed1f28747d2b8e3efd8b6
 */

namespace CoiniumServ.Tests.Coin.Coinbase
{
    public class SerializerTests
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
        private readonly IJob _job;

        public SerializerTests()
        {
            // daemon client
            _daemonClient = Substitute.For<IDaemonClient>();
            _daemonClient.ValidateAddress(Arg.Any<string>()).Returns(new ValidateAddress { IsValid = true });

            // block template
            const string json = "{\"result\":{\"version\":2,\"previousblockhash\":\"1c4eb88e47564cb796b5c6648c74bec51d7215ac12fc4168b14827aac74a8062\",\"transactions\":[{\"data\":\"010000000332a82e92f522deee69b09e27858ba9b87585f2a4913ef71018df40909032fdc3000000006a473044022019ca05cb880a04f0d842268b7e75ac6d2695fc544df033e3daeb29239251a8970220031f6336767f2ea617347484e1290ec0bdcc71056ea2d3084e75384905250ec50121030dd394118fb66ca288bff71d8ea762678783b005770f7f9ba4128233191e0847ffffffff086747cbd339b21b950774186091653a7b8f5751b00a906ff6f5561b3a6fcee6010000006b4830450221009ae1ba9a216d313cc592fc2c1ef08f1e0e555a32b6c1b305f685ac882d38356b0220243106bbb5bb76dde142e574cba8f30c1e2f7059e8e9161770396fbd2b50420f0121030dd394118fb66ca288bff71d8ea762678783b005770f7f9ba4128233191e0847ffffffffe2f15804b1e41c36c925c6f64f219b2bdb3c9fbff4c97a4f0e8c7f31d7e6f2af000000006b48304502200be8894fdd7f5c19be248a979c08bbf2395f606e038c3e02c0266474c03699ab022100ff5de87086e487410f5d7b68012655ca6d814f0caeb9ca42d9c425a90f68b3030121030dd394118fb66ca288bff71d8ea762678783b005770f7f9ba4128233191e0847ffffffff02a0f01900000000001976a9141c50209a1dfdf53313d237b75e9aeb553ca1dfda88ac00e1f505000000001976a914cbb9a3e7a7c1651b1006f876f08b40be85b274f588ac00000000\",\"hash\":\"dc3a80ec6c45aa489453b2c4abf6761eb6656d949e26d01793458c166640e5f3\",\"depends\":[],\"fee\":0,\"sigops\":2}],\"coinbaseaux\":{\"flags\":\"062f503253482f\"},\"coinbasevalue\":5000000000,\"target\":\"00000048d4f70000000000000000000000000000000000000000000000000000\",\"mintime\":1403691059,\"mutable\":[\"time\",\"transactions\",\"prevblock\"],\"noncerange\":\"00000000ffffffff\",\"sigoplimit\":20000,\"sizelimit\":1000000,\"curtime\":1403691825,\"bits\":\"1d48d4f7\",\"height\":315152},\"error\":null,\"id\":1}";
            var @object = JsonConvert.DeserializeObject<DaemonResponse<BlockTemplate>>(json);
            _blockTemplate = @object.Result;

            // extra nonce
            _extraNonce = Substitute.For<ExtraNonce>((UInt32)0);

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

            // sample reward recipient
            var rewardsConfig = Substitute.For<IRewardsConfig>();
            var amount = blockReward * 0.01;
            blockReward -= amount;
            var rewards = new Dictionary<string, float> { { "mrwhWEDnU6dUtHZJ2oBswTpEdbBHgYiMji", (float)amount } };

            rewardsConfig.GetEnumerator().Returns(rewards.GetEnumerator());
            foreach (var pair in rewards)
            {
                _outputs.AddRecipient(pair.Key, pair.Value);
            }

            // sample pool wallet
            var walletConfig = Substitute.For<IWalletConfig>();
            walletConfig.Adress.Returns("mk8JqN1kNWju8o3DXEijiJyn7iqkwktAWq");
            _outputs.AddPoolWallet(walletConfig.Adress, blockReward);
                                   
            // generation transaction.
            _generationTransaction = Substitute.For<GenerationTransaction>(_extraNonce, _daemonClient, _blockTemplate, walletConfig, rewardsConfig, false);
            _generationTransaction.Inputs.First().SignatureScript = _signatureScript;
            _generationTransaction.Outputs = _outputs;
            _generationTransaction.Create();

            // job counter
            _jobCounter = Substitute.For<IJobCounter>();
            _jobCounter.Next().Returns((UInt64)2);

            // hash algorithm
            _hashAlgorithm = Substitute.For<IHashAlgorithm>();

            // create the job
            _job = Substitute.For<Job>(_jobCounter.Next(), _hashAlgorithm, _blockTemplate, _generationTransaction);
        }

        [Fact]
        public void SerializeCoinbaseTest()
        {
            const UInt32 extraNonce1 = 0x58000000;
            const UInt32 extraNonce2 = 0x07000000;

            var coinbase = Serializers.SerializeCoinbase(_job, extraNonce1, extraNonce2);
            coinbase.ToHexString().Should().Equal("01000000010000000000000000000000000000000000000000000000000000000000000000ffffffff270310cf04062f503253482f0430a3aa530858000000070000000d2f6e6f64655374726174756d2f000000000280010b27010000001976a914329035234168b8da5af106ceb20560401236849888ac80f0fa02000000001976a9147d576fbfca48b899dc750167dd2a2a6572fff49588ac00000000");
        }

        [Fact]
        public void SerializeHeaderTest()
        {
            const UInt32 extraNonce1 = 0x58000000;
            const UInt32 extraNonce2 = 0x07000000;
            const UInt32 nTime = 0x53aaa331;
            const UInt32 nonce = 0x44725000;

            // create the coinbase.
            var coinbase = Serializers.SerializeCoinbase(_job, extraNonce1, extraNonce2);
            var coinbaseHash = CoiniumServ.Coin.Coinbase.Utils.HashCoinbase(coinbase);

            // create the merkle root.
            var merkleRoot = _job.MerkleTree.WithFirst(coinbaseHash).ReverseBuffer();

            // create the header
            var header = Serializers.SerializeHeader(_job, merkleRoot, nTime, nonce);

            // test the header.
            header.ToHexString().Should().Equal("0200000062804ac7aa2748b16841fc12ac15721dc5be748c64c6b596b74c56478eb84e1c1732a2242d83c6b1a935eecec4c48e56a10b0977f723456331f6b2ff5efb757831a3aa53f7d4481d00507244");
        }

        [Fact]
        public void SerializeBlockTest()
        {
            const UInt32 extraNonce1 = 0x58000000;
            const UInt32 extraNonce2 = 0x07000000;
            const UInt32 nTime = 0x53aaa331;
            const UInt32 nonce = 0x44725000;

            // create the coinbase.
            var coinbase = Serializers.SerializeCoinbase(_job, extraNonce1, extraNonce2);
            var coinbaseHash = CoiniumServ.Coin.Coinbase.Utils.HashCoinbase(coinbase);

            // create the merkle root.
            var merkleRoot = _job.MerkleTree.WithFirst(coinbaseHash).ReverseBuffer();

            // create the header
            var header = Serializers.SerializeHeader(_job, merkleRoot, nTime, nonce);

            // create the block hex
            var blockHex = Serializers.SerializeBlock(_job, header, coinbase);

            // test the block hex
            blockHex.ToHexString().Should().Equal("0200000062804ac7aa2748b16841fc12ac15721dc5be748c64c6b596b74c56478eb84e1c1732a2242d83c6b1a935eecec4c48e56a10b0977f723456331f6b2ff5efb757831a3aa53f7d4481d005072440201000000010000000000000000000000000000000000000000000000000000000000000000ffffffff270310cf04062f503253482f0430a3aa530858000000070000000d2f6e6f64655374726174756d2f000000000280010b27010000001976a914329035234168b8da5af106ceb20560401236849888ac80f0fa02000000001976a9147d576fbfca48b899dc750167dd2a2a6572fff49588ac00000000010000000332a82e92f522deee69b09e27858ba9b87585f2a4913ef71018df40909032fdc3000000006a473044022019ca05cb880a04f0d842268b7e75ac6d2695fc544df033e3daeb29239251a8970220031f6336767f2ea617347484e1290ec0bdcc71056ea2d3084e75384905250ec50121030dd394118fb66ca288bff71d8ea762678783b005770f7f9ba4128233191e0847ffffffff086747cbd339b21b950774186091653a7b8f5751b00a906ff6f5561b3a6fcee6010000006b4830450221009ae1ba9a216d313cc592fc2c1ef08f1e0e555a32b6c1b305f685ac882d38356b0220243106bbb5bb76dde142e574cba8f30c1e2f7059e8e9161770396fbd2b50420f0121030dd394118fb66ca288bff71d8ea762678783b005770f7f9ba4128233191e0847ffffffffe2f15804b1e41c36c925c6f64f219b2bdb3c9fbff4c97a4f0e8c7f31d7e6f2af000000006b48304502200be8894fdd7f5c19be248a979c08bbf2395f606e038c3e02c0266474c03699ab022100ff5de87086e487410f5d7b68012655ca6d814f0caeb9ca42d9c425a90f68b3030121030dd394118fb66ca288bff71d8ea762678783b005770f7f9ba4128233191e0847ffffffff02a0f01900000000001976a9141c50209a1dfdf53313d237b75e9aeb553ca1dfda88ac00e1f505000000001976a914cbb9a3e7a7c1651b1006f876f08b40be85b274f588ac00000000");
        }

        /// <summary>
        /// Tests <see cref="Serializers.VarInt"/>.
        /// </summary>
        [Fact]
        public void VarIntegerTest()
        {
            // < 0xfd test
            var varInt = Serializers.VarInt(0xfc);
            var expected = new byte[] { 0xfc };
            varInt.Should().Equal(expected);

            // < 0xffff
            varInt = Serializers.VarInt(0xfffe);
            expected = new byte[] { 0xfd, 0xfe, 0xff };
            varInt.Should().Equal(expected);

            // 0xffffffff
            varInt = Serializers.VarInt(0xfffffffe);
            expected = new byte[] { 0xfe, 0xfe, 0xff, 0xff, 0xff };
            varInt.Should().Equal(expected);
        }


        /// <summary>
        /// Tests <see cref="Serializers.SerializeNumber(int)"/>.
        /// </summary>
        [Fact]
        public void SerializeNumberTest()
        {
            // =< 16 test
            var buffer = Serializers.SerializeNumber(16);
            var expected = new byte[] { 0x01, 0x10 };
            buffer.Should().Equal(expected);

            // > 16 test
            buffer = Serializers.SerializeNumber(10000);
            expected = new byte[] { 0x02, 0x10, 0x27 };
            buffer.Should().Equal(expected);
        }

        /// <summary>
        /// Tests <see cref="Serializers.SerializeString"/>.
        /// </summary>
        [Fact]
        public void SerializeStringTest()
        {
            // < 253 test
            var serialized = Serializers.SerializeString("https://github.com/CoiniumServ/CoiniumServ");
            serialized.ToHexString().Should().Equal("2a68747470733a2f2f6769746875622e636f6d2f436f696e69756d536572762f436f696e69756d53657276");

            // >= 253 & <65536 (0x10000) test
            serialized = Serializers.SerializeString(@"Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.");
            serialized.ToHexString().Should().Equal("fd3e024c6f72656d20497073756d2069732073696d706c792064756d6d792074657874206f6620746865207072696e74696e6720616e64207479706573657474696e6720696e6475737472792e204c6f72656d20497073756d20686173206265656e2074686520696e6475737472792773207374616e646172642064756d6d79207465787420657665722073696e6365207468652031353030732c207768656e20616e20756e6b6e6f776e207072696e74657220746f6f6b20612067616c6c6579206f66207479706520616e6420736372616d626c656420697420746f206d616b65206120747970652073706563696d656e20626f6f6b2e20497420686173207375727669766564206e6f74206f6e6c7920666976652063656e7475726965732c2062757420616c736f20746865206c65617020696e746f20656c656374726f6e6963207479706573657474696e672c2072656d61696e696e6720657373656e7469616c6c7920756e6368616e6765642e2049742077617320706f70756c61726973656420696e207468652031393630732077697468207468652072656c65617365206f66204c657472617365742073686565747320636f6e7461696e696e67204c6f72656d20497073756d2070617373616765732c20616e64206d6f726520726563656e746c792077697468206465736b746f70207075626c697368696e6720736f667477617265206c696b6520416c64757320506167654d616b657220696e636c7564696e672076657273696f6e73206f66204c6f72656d20497073756d2e");
        }
    }
}
