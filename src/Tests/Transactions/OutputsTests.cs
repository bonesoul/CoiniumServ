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
using Coinium.Coin.Daemon;
using Coinium.Coin.Daemon.Responses;
using Coinium.Transactions;
using Nancy.ViewEngines.Razor;
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
            outputs.List.Last().PublicKeyScript.Should().Equal(new byte[]
            {
                0x76, 0xa9, 0x14, 0x7d, 0x57, 0x6f, 0xbf, 0xca, 0x48, 0xb8, 0x99, 0xdc, 0x75, 0x01, 0x67, 0xdd,
                0x2a, 0x2a, 0x65, 0x72, 0xff, 0xf4, 0x95, 0x88, 0xac
            });

            // test the pool wallet 
            outputs.List.First().Value.Should().Equal((UInt64)0x00000001270b0180); // packInt64LE: 80010b2701000000
            outputs.List.First().PublicKeyScriptLenght.Should().Equal(new byte[] { 0x19 });
            outputs.List.First().PublicKeyScript.Should().Equal(new byte[]
            {
                0x76, 0xa9, 0x14, 0x32, 0x90, 0x35, 0x23, 0x41, 0x68, 0xb8, 0xda, 0x5a, 0xf1, 0x06, 0xce, 0xb2,
                0x05, 0x60, 0x40, 0x12, 0x36, 0x84, 0x98, 0x88, 0xac
            });

            // test the outputs buffer.
            outputs.GetBuffer().Should().Equal(new byte[]
            {
                0x02, 0x80, 0x01, 0x0b, 0x27, 0x01, 0x00, 0x00, 0x00, 0x19, 0x76, 0xa9, 0x14, 0x32, 0x90, 0x35, 
                0x23, 0x41, 0x68, 0xb8, 0xda, 0x5a, 0xf1, 0x06, 0xce, 0xb2, 0x05, 0x60, 0x40, 0x12, 0x36, 0x84, 
                0x98, 0x88, 0xac, 0x80, 0xf0, 0xfa, 0x02, 0x00, 0x00, 0x00, 0x00, 0x19, 0x76, 0xa9, 0x14, 0x7d,
                0x57, 0x6f, 0xbf, 0xca, 0x48, 0xb8, 0x99, 0xdc, 0x75, 0x01, 0x67, 0xdd, 0x2a, 0x2a, 0x65, 0x72,
                0xff, 0xf4, 0x95, 0x88, 0xac
            });

        }
    }
}
