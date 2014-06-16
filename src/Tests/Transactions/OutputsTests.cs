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
using Coinium.Transactions;
using NSubstitute;
using Should.Fluent;
using Xunit;

namespace Tests.Transactions
{
    public class OutputsTests
    {
        [Fact]
        public void TestOutputs()
        {
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

            // init mockup objects
            var daemonClient = Substitute.For<IDaemonClient>();
            daemonClient.ValidateAddress(Arg.Any<string>()).Returns(new ValidateAddress { IsValid = true });

            // setup the outputs based on the sample.
            var outputs = new Outputs(daemonClient);
            double blockReward = 5000000000; // the amount rewarded by the block.

            // sample recipient
            const string recipient = "mrwhWEDnU6dUtHZJ2oBswTpEdbBHgYiMji";
            var amount = blockReward * 0.01;
            blockReward -= amount;
            outputs.AddRecipient(recipient, amount);

            // sample pool wallet
            const string poolWallet = "mk8JqN1kNWju8o3DXEijiJyn7iqkwktAWq";
            outputs.AddPool(poolWallet, blockReward);

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
