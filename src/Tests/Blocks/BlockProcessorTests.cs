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
        public void QueryBlockTest_WithInvalidBlockHash_ShouldBeOrphaned()
        {
            // create a test case where coin daemon reports block hash as invalid.
            _daemonClient.GetBlock(Arg.Any<string>()).Returns(info => null);
            var block = new PersistedBlock(1, false, false, "INVALID_HASH", "TX_HASH", 0, 0, DateTime.Now);
            
            // query the block.
            var exposed = Exposed.From(new BlockProcessor(_poolConfig, _daemonClient, _storageLayer));
            exposed.QueryBlock(block);

            // block should be marked as orphaned.
            block.Status.Should().Equal(BlockStatus.Orphaned);
        }

        [Fact]
        public void QueryBlockTest_WithNegativeConfirmations_ShouldBeOrphaned()
        {
            // create a test case where coin daemon returns block-info with negative confirmations.
            _daemonClient.GetBlock(Arg.Any<string>()).Returns(info => new Block { Confirmations = -1 });
            var block = new PersistedBlock(1, false, false, "BLOCK_HASH", "TX_HASH", 0, 0, DateTime.Now);

            // query the block.
            var exposed = Exposed.From(new BlockProcessor(_poolConfig, _daemonClient, _storageLayer));
            exposed.QueryBlock(block);

            // block should be marked as orphaned.
            block.Status.Should().Equal(BlockStatus.Orphaned);
        }

        [Fact]
        public void QueryBlockTest_WithDifferentTransactionHash_ShouldBeOrphaned()
        {
            // create a test case where coin reports a different tx-hash then the one in block.
            _daemonClient.GetBlock(Arg.Any<string>()).Returns(info => new Block {Tx = new List<string>() {"DIFFERENT"}});
            var block = new PersistedBlock(1, false, false, "BLOCK_HASH", "TX_HASH", 0, 0, DateTime.Now);

            // query the block.
            var exposed = Exposed.From(new BlockProcessor(_poolConfig, _daemonClient, _storageLayer));
            exposed.QueryBlock(block);

            // block should be marked as orphaned.
            block.Status.Should().Equal(BlockStatus.Orphaned);
        }

        [Fact]
        public void QueryBlockTest_WithNonExistingGenerationTransaction_ShouldBeOrphaned()
        {
            // create a test case where coin reports generation transaction hash as invalid.
            _daemonClient.GetBlock(Arg.Any<string>()).Returns(info => new Block { Tx = new List<string>() { "TX_HASH" } });
            _daemonClient.GetTransaction(Arg.Any<string>()).Returns(info => null);
            var block = new PersistedBlock(1, false, false, "BLOCK_HASH", "TX_HASH", 0, 0, DateTime.Now);

            // query the block.
            var exposed = Exposed.From(new BlockProcessor(_poolConfig, _daemonClient, _storageLayer));
            exposed.QueryBlock(block);

            // block should be marked as orphaned.
            block.Status.Should().Equal(BlockStatus.Orphaned);
        }
    }
}
