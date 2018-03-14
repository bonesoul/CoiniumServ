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
using System.Linq;
using CoiniumServ.Coin.Config;
using CoiniumServ.Daemon;
using CoiniumServ.Daemon.Responses;
using CoiniumServ.Transactions;
using CoiniumServ.Utils.Extensions;
using NSubstitute;
using Should.Fluent;
using Xunit;

/*
    -- generateOutputTransactions --
    block-reward: 5000000000
    recipient-reward: 50000000 packInt64LE: 80f0fa0200000000
    lenght: 25 varIntBuffer: 19
    script: 76a9147d576fbfca48b899dc750167dd2a2a6572fff49588ac
    pool-reward: 4950000000 packInt64LE: 80010b2701000000
    lenght: 25 varIntBuffer: 19
    script: 76a914329035234168b8da5af106ceb20560401236849888ac
    txOutputBuffers.lenght : 2 varIntBuffer: 02
    outputTransactions: 0280010b27010000001976a914329035234168b8da5af106ceb20560401236849888ac80f0fa02000000001976a9147d576fbfca48b899dc750167dd2a2a6572fff49588ac
 */

namespace CoiniumServ.Tests.Transactions
{
    public class OutputsTests
    {
        // object mocks.
        private readonly IDaemonClient _daemonClient;
        private readonly ICoinConfig _coinConfig;

        public OutputsTests()
        {
            _daemonClient = Substitute.For<IDaemonClient>();
            _daemonClient.ValidateAddress(Arg.Any<string>()).Returns(new ValidateAddress { IsValid = true });
            // _daemonClient.GetAddressInfo(Arg.Any<string>()).Returns(new GetAddressInfo { IsValid = true });

            _coinConfig = Substitute.For<ICoinConfig>();
            _coinConfig.Options.IsProofOfStakeHybrid.Returns(false);
        }

        [Fact]
        public void TestOutputs()
        {
            var outputs = new Outputs(_daemonClient, _coinConfig);

            // setup the outputs based on the sample.
            double blockReward = 5000000000; // the amount rewarded by the block.

            // sample recipient
            const string recipient = "mrwhWEDnU6dUtHZJ2oBswTpEdbBHgYiMji";
            var amount = blockReward * 0.01;
            blockReward -= amount;
            outputs.AddRecipient(recipient, amount);

            // sample pool wallet
            const string poolWallet = "mk8JqN1kNWju8o3DXEijiJyn7iqkwktAWq";
            outputs.AddPoolWallet(poolWallet, blockReward);

            // test the recipient rewards
            outputs.List.Last().Value.Should().Equal((UInt64)0x0000000002faf080); // packInt64LE: 80f0fa0200000000
            outputs.List.Last().PublicKeyScriptLenght.Should().Equal(new byte[] { 0x19 });
            outputs.List.Last().PublicKeyScript.ToHexString().Should().Equal("76a9147d576fbfca48b899dc750167dd2a2a6572fff49588ac");

            // test the pool wallet 
            outputs.List.First().Value.Should().Equal((UInt64)0x00000001270b0180); // packInt64LE: 80010b2701000000
            outputs.List.First().PublicKeyScriptLenght.Should().Equal(new byte[] { 0x19 });
            outputs.List.First().PublicKeyScript.ToHexString().Should().Equal("76a914329035234168b8da5af106ceb20560401236849888ac");

            // test the outputs buffer.
            outputs.GetBuffer().ToHexString().Should().Equal("0280010b27010000001976a914329035234168b8da5af106ceb20560401236849888ac80f0fa02000000001976a9147d576fbfca48b899dc750167dd2a2a6572fff49588ac");
        }
    }
}
