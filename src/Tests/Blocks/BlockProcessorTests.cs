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
using CoiniumServ.Blocks;
using CoiniumServ.Daemon;
using CoiniumServ.Daemon.Responses;
using CoiniumServ.Persistance.Blocks;
using CoiniumServ.Persistance.Layers;
using CoiniumServ.Pools;
using ExposedObject;
using NSubstitute;
using Should.Fluent;
using Xunit;

namespace CoiniumServ.Tests.Blocks
{
    public class BlockProcessorTests
    {
        // object mocks.
        private readonly IPoolConfig _poolConfig;
        private readonly IDaemonClient _daemonClient;
        private readonly IStorageLayer _storageLayer;

        public BlockProcessorTests()
        {
            // create mockup objects required for tests.            
            _daemonClient = Substitute.For<IDaemonClient>();
            _poolConfig = Substitute.For<IPoolConfig>();
            _storageLayer = Substitute.For<IStorageLayer>();
        }

        [Fact]
        private void QueryBlockTest_WithInvalidBlockHash_ShouldBeOrphaned()
        {
            // create a test case where coin daemon reports block hash as invalid.
            var block = new PersistedBlock(1, false, false, "INVALID_HASH", "TX_HASH", 0, 0, DateTime.Now);
            _daemonClient.GetBlock(Arg.Any<string>()).Returns(info => null);            
            
            // query the block.
            var exposed = Exposed.From(new BlockProcessor(_poolConfig, _daemonClient, _storageLayer));
            exposed.QueryBlock(block);

            // block should be marked as orphaned.
            block.Status.Should().Equal(BlockStatus.Orphaned);
        }

        [Fact]
        private void QueryBlockTest_WithNegativeConfirmations_ShouldBeOrphaned()
        {
            // create a test case where coin daemon returns block-info with negative confirmations.
            var block = new PersistedBlock(1, false, false, "BLOCK_HASH", "TX_HASH", 0, 0, DateTime.Now);
            _daemonClient.GetBlock(Arg.Any<string>()).Returns(info => new Block { Confirmations = -1 });            

            // query the block.
            var exposed = Exposed.From(new BlockProcessor(_poolConfig, _daemonClient, _storageLayer));
            exposed.QueryBlock(block);

            // block should be marked as orphaned.
            block.Status.Should().Equal(BlockStatus.Orphaned);
        }

        [Fact]
        private void QueryBlockTest_WithInvalidTransactionHash_ShouldBeOrphaned()
        {
            // create a test case where coin reports a different tx-hash then the one in block.
            var block = new PersistedBlock(1, false, false, "BLOCK_HASH", "TX_HASH", 0, 0, DateTime.Now);
            _daemonClient.GetBlock(Arg.Any<string>()).Returns(info => new Block {Tx = new List<string>() {"DIFFERENT"}});

            // query the block.
            var exposed = Exposed.From(new BlockProcessor(_poolConfig, _daemonClient, _storageLayer));
            exposed.QueryBlock(block);

            // block should be marked as orphaned.
            block.Status.Should().Equal(BlockStatus.Orphaned);
        }

        [Fact]
        private void QueryBlockTest_WithNonExistingGenerationTransaction_ShouldBeOrphaned()
        {
            // create a test case where coin reports generation transaction hash as invalid.
            var block = new PersistedBlock(1, false, false, "BLOCK_HASH", "TX_HASH", 0, 0, DateTime.Now);
            _daemonClient.GetBlock(Arg.Any<string>()).Returns(info => new Block { Tx = new List<string>() { "TX_HASH" } });
            _daemonClient.GetTransaction(Arg.Any<string>()).Returns(info => null);            

            // query the block.
            var exposed = Exposed.From(new BlockProcessor(_poolConfig, _daemonClient, _storageLayer));
            exposed.QueryBlock(block);

            // block should be marked as orphaned.
            block.Status.Should().Equal(BlockStatus.Orphaned);
        }

        [Fact]
        private void QueryBlockTest_WithNonExistingPoolOutput_ShouldBeOrphaned()
        {
            // create a test case where generation transaction doesn't contain an output for the pool.
            var block = new PersistedBlock(1, false, false, "BLOCK_HASH", "TX_HASH", 0, 0, DateTime.Now);
            _daemonClient.GetBlock(Arg.Any<string>()).Returns(info => new Block { Tx = new List<string>() { "TX_HASH" } });
            _daemonClient.GetTransaction(Arg.Any<string>()).Returns(info => new Transaction());

            // query the block.
            var exposed = Exposed.From(new BlockProcessor(_poolConfig, _daemonClient, _storageLayer));
            exposed.QueryBlock(block);

            // block should be marked as orphaned.
            block.Status.Should().Equal(BlockStatus.Orphaned);
        }

        [Fact]
        private void QueryBlockTest_WithIncorrectPoolOutputAddress_ShouldBeOrphaned()
        {
            // create a test case where generation transaction output doesn't match pool output address.
            var block = new PersistedBlock(1, false, false, "BLOCK_HASH", "TX_HASH", 0, 0, DateTime.Now);
            _daemonClient.GetBlock(Arg.Any<string>()).Returns(info => new Block { Tx = new List<string>() { "TX_HASH" } });
            _poolConfig.Wallet.Adress.Returns("POOL_ADDRESS");
            _daemonClient.GetTransaction(Arg.Any<string>()).Returns(info => new Transaction
            {
                Details = new List<TransactionDetail> { new TransactionDetail { Address = "DIFFERENT_ADDRESS" } }
            });

            // query the block.
            var exposed = Exposed.From(new BlockProcessor(_poolConfig, _daemonClient, _storageLayer));
            exposed.QueryBlock(block);

            // block should be marked as orphaned.
            block.Status.Should().Equal(BlockStatus.Orphaned);
        }

        [Fact]
        private void QueryBlockTest_WithIncorrectPoolOutputAccount_ShouldBeOrphaned()
        {
            // create a test case where generation transaction output doesn't match pool output account.
            var block = new PersistedBlock(1, false, false, "BLOCK_HASH", "TX_HASH", 0, 0, DateTime.Now);            
            _daemonClient.GetBlock(Arg.Any<string>()).Returns(info => new Block { Tx = new List<string>() { "TX_HASH" } });
            _daemonClient.GetTransaction(Arg.Any<string>()).Returns(info => new Transaction
            {
                Details = new List<TransactionDetail> { new TransactionDetail { Account = "DIFFERENT_ACCOUNT" } }
            });
            var exposed = Exposed.From(new BlockProcessor(_poolConfig, _daemonClient, _storageLayer));
            exposed._poolAccount = "POOL_ACCOUNT";

            // query the block.           
            exposed.QueryBlock(block);

            // block should be marked as orphaned.
            block.Status.Should().Equal(BlockStatus.Orphaned);
        }

        private void QueryBlockTest_WithPoolOutputAddress_ShouldEqual()
        {
            // create a test case and find pool output by address
            var block = new PersistedBlock(1, false, false, "BLOCK_HASH", "TX_HASH", 0, 0, DateTime.Now);
            _daemonClient.GetBlock(Arg.Any<string>()).Returns(info => new Block { Tx = new List<string>() { "TX_HASH" } });
            _poolConfig.Wallet.Adress.Returns("POOL_ADDRESS");
            var output = new TransactionDetail {Address = "POOL_ADDRESS"};
            _daemonClient.GetTransaction(Arg.Any<string>()).Returns(info => new Transaction
            {
                Details = new List<TransactionDetail> { output }
            });

            // query the pool output.
            var blockProcessor = new BlockProcessor(_poolConfig, _daemonClient, _storageLayer);
            //var transaction = _daemonClient.GetTransaction("")
            //blockProcessor.GetPoolOutput()

        }

        private void QueryBlockTest_SetReward()
        {
            // create a test case and set block reward based on pool output value.
        }
    }
}
