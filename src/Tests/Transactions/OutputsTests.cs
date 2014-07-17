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
using System.Linq;
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

        public OutputsTests()
        {
            _daemonClient = Substitute.For<IDaemonClient>();
            _daemonClient.ValidateAddress(Arg.Any<string>()).Returns(new ValidateAddress { IsValid = true });
        }

        [Fact]
        public void TestOutputs()
        {
            var outputs = new Outputs(_daemonClient);

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
