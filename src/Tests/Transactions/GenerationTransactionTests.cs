#region License
// 
//     MIT License
//
//     CoiniumServ - Crypto Currency Mining Pool Server Software
//     Copyright (C) 2013 - 2017, CoiniumServ Project
//     Hüseyin Uslu, shalafiraistlin at gmail dot com
//     https://github.com/bonesoul/CoiniumServ
// 
//     Permission is hereby granted, free of charge, to any person obtaining a copy
//     of this software and associated documentation files (the "Software"), to deal
//     in the Software without restriction, including without limitation the rights
//     to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//     copies of the Software, and to permit persons to whom the Software is
//     furnished to do so, subject to the following conditions:
//     
//     The above copyright notice and this permission notice shall be included in all
//     copies or substantial portions of the Software.
//     
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//     FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//     LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//     OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//     SOFTWARE.
// 
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using CoiniumServ.Coin.Config;
using CoiniumServ.Daemon;
using CoiniumServ.Daemon.Responses;
using CoiniumServ.Jobs;
using CoiniumServ.Payments.Config;
using CoiniumServ.Pools;
using CoiniumServ.Transactions;
using CoiniumServ.Transactions.Script;
using CoiniumServ.Utils.Extensions;
using Newtonsoft.Json;
using NSubstitute;
using Should.Fluent;
using Xunit;

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

namespace CoiniumServ.Tests.Transactions
{
    // TODO: tests below target non-POS coins with no txMessage support. Implement tests for those specific conditions too.

    public class GenerationTransactionTests
    {
        // object mocks.
        private readonly IDaemonClient _daemonClient;
        private readonly IBlockTemplate _blockTemplate;
        private readonly IExtraNonce _extraNonce;
        private readonly ISignatureScript _signatureScript;
        private readonly IOutputs _outputs;
        private readonly IPoolConfig _poolConfig;

        public GenerationTransactionTests()
        {
            // daemon client
            _daemonClient = Substitute.For<IDaemonClient>();
            _daemonClient.ValidateAddress(Arg.Any<string>()).Returns(new ValidateAddress { IsValid = true });
            // _daemonClient.GetAddressInfo(Arg.Any<string>()).Returns(new GetAddressInfo { IsValid = true });

            // block template
            const string json = "{\"result\":{\"version\":2,\"previousblockhash\":\"e9bbcc9b46ed98fd4850f2d21e85566defdefad3453460caabc7a635fc5a1261\",\"transactions\":[],\"coinbaseaux\":{\"flags\":\"062f503253482f\"},\"coinbasevalue\":5000000000,\"target\":\"0000004701b20000000000000000000000000000000000000000000000000000\",\"mintime\":1402660580,\"mutable\":[\"time\",\"transactions\",\"prevblock\"],\"noncerange\":\"00000000ffffffff\",\"sigoplimit\":20000,\"sizelimit\":1000000,\"curtime\":1402661060,\"bits\":\"1d4701b2\",\"height\":302526},\"error\":null,\"id\":1}";
            var @object = JsonConvert.DeserializeObject<DaemonResponse<BlockTemplate>>(json);
            _blockTemplate = @object.Result;

            // extra nonce
            _extraNonce = new ExtraNonce(0);

            // signature script
            _signatureScript = new SignatureScript(
                _blockTemplate.Height,
                _blockTemplate.CoinBaseAux.Flags,
                1402661059432,
                (byte)_extraNonce.ExtraNoncePlaceholder.Length,
                "/nodeStratum/");

            // pool config
            _poolConfig = Substitute.For<IPoolConfig>();

            // create coin config.
            var coinConfig = Substitute.For<ICoinConfig>();
            coinConfig.Options.TxMessageSupported.Returns(false);
            coinConfig.Options.IsProofOfStakeHybrid.Returns(false);
            _poolConfig.Coin.Returns(coinConfig);

            // use the same output data within our sample data.
            _outputs = Substitute.For<Outputs>(_daemonClient, coinConfig);
            double blockReward = 5000000000; // the amount rewarded by the block.

            // create rewards config.
            var rewardsConfig = Substitute.For<IRewardsConfig>();
            _poolConfig.Rewards.Returns(rewardsConfig);            

            // create sample reward
            var amount = blockReward * 0.01;
            blockReward -= amount;
            var rewards = new Dictionary<string, float> { { "mrwhWEDnU6dUtHZJ2oBswTpEdbBHgYiMji", (float)amount } };

            rewardsConfig.GetEnumerator().Returns(rewards.GetEnumerator());
            foreach (var pair in rewards)
            {
                _outputs.AddRecipient(pair.Key, pair.Value);
            }

            // create wallet config.
            var walletConfig = Substitute.For<IWalletConfig>();
            _poolConfig.Wallet.Returns(walletConfig);

            // create sample pool central wallet output.
            walletConfig.Adress.Returns("mk8JqN1kNWju8o3DXEijiJyn7iqkwktAWq");
            _outputs.AddPoolWallet(walletConfig.Adress, blockReward);            
        }

        [Fact]
        public void CreateGenerationTransactionTest()
        {
            // create the test object.
            var generationTransaction = new GenerationTransaction(_extraNonce, _daemonClient, _blockTemplate, _poolConfig);

            // use the exactly same input script data within our sample data.
            generationTransaction.Inputs.First().SignatureScript = _signatureScript;
            generationTransaction.Outputs = _outputs;

            // create the transaction
            generationTransaction.Create();

            // test version.
            generationTransaction.Version.Should().Equal((UInt32)2);
            generationTransaction.Initial.Take(4).ToHexString().Should().Equal("02000000");

            // test inputs count.
            generationTransaction.InputsCount.Should().Equal((UInt32)1);
            generationTransaction.Initial.Skip(4).Take(1).Should().Equal(new byte[] { 0x01 }); 

            // test the input previous-output hash
            generationTransaction.Initial.Skip(5).Take(32).ToHexString().Should().Equal("0000000000000000000000000000000000000000000000000000000000000000");

            // test the input previous-output index
            generationTransaction.Inputs.First().PreviousOutput.Index.Should().Equal(0xffffffff);
            generationTransaction.Initial.Skip(37).Take(4).ToHexString().Should().Equal("ffffffff");

            // test the lenghts byte
            generationTransaction.Inputs.First().SignatureScript.Initial.Length.Should().Equal(17);
            _extraNonce.ExtraNoncePlaceholder.Length.Should().Equal(8);
            generationTransaction.Inputs.First().SignatureScript.Final.Length.Should().Equal(14);
            generationTransaction.Initial.Skip(41).Take(1).ToHexString().Should().Equal("27");

            // test the signature script initial
            generationTransaction.Initial.Skip(42).Take(17).ToHexString().Should().Equal("03be9d04062f503253482f04c3e89a5308");
            
            // test the signature script final
            generationTransaction.Final.Take(14).ToHexString().Should().Equal("0d2f6e6f64655374726174756d2f");

            // test the inputs sequence
            generationTransaction.Inputs.First().Sequence.Should().Equal((UInt32)0x00);
            generationTransaction.Final.Skip(14).Take(4).ToHexString().Should().Equal("00000000");

            // test the outputs buffer
            generationTransaction.Final.Skip(18).Take(69).ToHexString().Should().Equal("0280010b27010000001976a914329035234168b8da5af106ceb20560401236849888ac80f0fa02000000001976a9147d576fbfca48b899dc750167dd2a2a6572fff49588ac");

            // test the lock time
            generationTransaction.LockTime.Should().Equal((UInt32)0x00);
            generationTransaction.Final.Skip(87).Take(4).ToHexString().Should().Equal("00000000");

            // test the generation transaction initial part.
            generationTransaction.Initial.ToHexString().Should().Equal("02000000010000000000000000000000000000000000000000000000000000000000000000ffffffff2703be9d04062f503253482f04c3e89a5308");

            // test the generation transaction final part.
            generationTransaction.Final.ToHexString().Should().Equal("0d2f6e6f64655374726174756d2f000000000280010b27010000001976a914329035234168b8da5af106ceb20560401236849888ac80f0fa02000000001976a9147d576fbfca48b899dc750167dd2a2a6572fff49588ac00000000");
        }
    }
}
