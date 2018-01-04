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
using CoiniumServ.Blocks;
using CoiniumServ.Daemon;
using CoiniumServ.Daemon.Errors;
using CoiniumServ.Daemon.Exceptions;
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
            // test case: coin daemon reports block hash as invalid.
            var block = new PersistedBlock(1, false, false, false, "INVALID_HASH", "TX_HASH", 0, 0, DateTime.Now);
            _daemonClient.GetBlock("INVALID_HASH").Returns(x =>
            {
                throw new RpcErrorException(new RpcErrorResponse
                {
                    Error = new RpcError
                    {
                        Code = -5 // 'block not found'.
                    }
                });
            });

            // query the block.
            var exposed = Exposed.From(new BlockProcessor(_poolConfig, _daemonClient, _storageLayer));
            exposed.QueryBlock(block);

            // block should be marked as orphaned.
            block.Status.Should().Equal(BlockStatus.Orphaned);
        }

        [Fact]
        private void QueryBlockTest_WithNegativeConfirmations_ShouldBeOrphaned()
        {
            // test case: coin daemon returns block-info with negative confirmations.
            var block = new PersistedBlock(1, false, false, false, "BLOCK_HASH", "TX_HASH", 0, 0, DateTime.Now);
            _daemonClient.GetBlock("BLOCK_HASH").Returns(info => new Block { Confirmations = -1 });            

            // query the block.
            var exposed = Exposed.From(new BlockProcessor(_poolConfig, _daemonClient, _storageLayer));
            exposed.QueryBlock(block);

            // block should be marked as orphaned.
            block.Status.Should().Equal(BlockStatus.Orphaned);
        }

        [Fact]
        private void QueryBlockTest_WithInvalidTransactionHash_ShouldBeOrphaned()
        {
            // test case: coin daemon reports a different tx-hash then the one in block.
            var block = new PersistedBlock(1, false, false, false, "BLOCK_HASH", "TX_HASH", 0, 0, DateTime.Now);
            _daemonClient.GetBlock("BLOCK_HASH").Returns(info => new Block { Tx = new List<string> { "DIFFERENT" } });

            // query the block.
            var exposed = Exposed.From(new BlockProcessor(_poolConfig, _daemonClient, _storageLayer));
            exposed.QueryBlock(block);

            // block should be marked as orphaned.
            block.Status.Should().Equal(BlockStatus.Orphaned);
        }

        [Fact]
        private void QueryBlockTest_WithNonExistingGenerationTransaction_ShouldBeOrphaned()
        {
            // test case: coin reports generation transaction hash as invalid.
            var block = new PersistedBlock(1, false, false, false, "BLOCK_HASH", "TX_HASH", 0, 0, DateTime.Now);
            _daemonClient.GetBlock("BLOCK_HASH").Returns(info => new Block { Tx = new List<string> { "TX_HASH" } });

            _daemonClient.GetTransaction("TX_HASH").Returns(x =>
            {
                throw new RpcErrorException(new RpcErrorResponse
                {
                    Error = new RpcError
                    {
                        Code = -5 // 'Invalid or non-wallet transaction id'
                    }
                });
            });          

            // query the block.
            var exposed = Exposed.From(new BlockProcessor(_poolConfig, _daemonClient, _storageLayer));
            exposed.QueryBlock(block);

            // block should be marked as orphaned.
            block.Status.Should().Equal(BlockStatus.Orphaned);
        }

        [Fact]
        private void QueryBlockTest_WithNonExistingPoolOutput_ShouldBeOrphaned()
        {
            // test case: generation transaction doesn't contain an output for the pool.
            var block = new PersistedBlock(1, false, false, false, "BLOCK_HASH", "TX_HASH", 0, 0, DateTime.Now);
            _daemonClient.GetBlock("BLOCK_HASH").Returns(info => new Block { Tx = new List<string> { "TX_HASH" } });
            _daemonClient.GetTransaction("TX_HASH").Returns(info => new Transaction());

            // query the block.
            var exposed = Exposed.From(new BlockProcessor(_poolConfig, _daemonClient, _storageLayer));
            exposed.QueryBlock(block);

            // block should be marked as orphaned.
            block.Status.Should().Equal(BlockStatus.Orphaned);
        }

        [Fact]
        private void QueryBlockTest_WithIncorrectPoolOutputAddress_ShouldBeOrphaned()
        {
            // test case: generation transaction output doesn't match pool output address.
            var block = new PersistedBlock(1, false, false, false, "BLOCK_HASH", "TX_HASH", 0, 0, DateTime.Now);
            _poolConfig.Wallet.Adress.Returns("POOL_ADDRESS");

            _daemonClient.GetBlock("BLOCK_HASH").Returns(info => new Block { Tx = new List<string> { "TX_HASH" } });
            _daemonClient.GetTransaction("TX_HASH").Returns(info => new Transaction
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
            // test case: generation transaction output doesn't match pool output account.
            var block = new PersistedBlock(1, false, false, false, "BLOCK_HASH", "TX_HASH", 0, 0, DateTime.Now);

            _daemonClient.GetBlock("BLOCK_HASH").Returns(info => new Block { Tx = new List<string> { "TX_HASH" } });
            _daemonClient.GetTransaction("TX_HASH").Returns(info => new Transaction
            {
                Details = new List<TransactionDetail> { new TransactionDetail { Account = "DIFFERENT_ACCOUNT" } }
            });

            // query the block.
            var exposed = Exposed.From(new BlockProcessor(_poolConfig, _daemonClient, _storageLayer));
            exposed._poolAccount = "POOL_ACCOUNT";          
            exposed.QueryBlock(block);

            // block should be marked as orphaned.
            block.Status.Should().Equal(BlockStatus.Orphaned);
        }

        [Fact]
        private void QueryBlockTest_ShouldSetReward()
        {
            // test case: set block reward based on pool output value.
            var block = new PersistedBlock(1, false, false, false, "BLOCK_HASH", "TX_HASH", 0, 0, DateTime.Now);
            _poolConfig.Wallet.Adress.Returns("POOL_ADDRESS");

            _daemonClient.GetBlock("BLOCK_HASH").Returns(info => new Block { Tx = new List<string> { "TX_HASH" } });
            _daemonClient.GetTransaction("TX_HASH").Returns(info => new Transaction
            {
                Details = new List<TransactionDetail> { new TransactionDetail { Address = "POOL_ADDRESS", Amount = 999 } }
            });

            // query the block.
            var exposed = Exposed.From(new BlockProcessor(_poolConfig, _daemonClient, _storageLayer));
            exposed.QueryBlock(block);

            // block reward should be set to 999
            block.Reward.Should().Equal((decimal)999);
        }

        [Fact]
        private void QueryBlockTest_WithImmaturePoolOutputCategory_ShouldStayPending()
        {
            // test case: we supply a pending block which should stay as pending as pool output category is still 'immature'.
            var block = new PersistedBlock(1, false, false, false, "BLOCK_HASH", "TX_HASH", 0, 0, DateTime.Now);
            _poolConfig.Wallet.Adress.Returns("POOL_ADDRESS");

            _daemonClient.GetBlock("BLOCK_HASH").Returns(info => new Block { Tx = new List<string> { "TX_HASH" } });
            _daemonClient.GetTransaction("TX_HASH").Returns(info => new Transaction
            {
                Details = new List<TransactionDetail> { new TransactionDetail { Address = "POOL_ADDRESS", Category = "immature"} }
            });

            // query the block.
            var exposed = Exposed.From(new BlockProcessor(_poolConfig, _daemonClient, _storageLayer));
            exposed.QueryBlock(block);

            // block should still stay as pending
            block.Status.Should().Equal(BlockStatus.Pending);
        }

        [Fact]
        private void QueryBlockTest_WithGeneratePoolOutputCategory_ShouldGetConfirmed()
        {
            // test case: we supply a pending block which should stay as pending as pool output category is still 'immature'.
            var block = new PersistedBlock(1, false, false, false, "BLOCK_HASH", "TX_HASH", 0, 0, DateTime.Now);
            _poolConfig.Wallet.Adress.Returns("POOL_ADDRESS");

            _daemonClient.GetBlock("BLOCK_HASH").Returns(info => new Block { Tx = new List<string> { "TX_HASH" } });
            _daemonClient.GetTransaction("TX_HASH").Returns(info => new Transaction
            {
                Details = new List<TransactionDetail> { new TransactionDetail { Address = "POOL_ADDRESS", Category = "generate" } }
            });

            // query the block.
            var exposed = Exposed.From(new BlockProcessor(_poolConfig, _daemonClient, _storageLayer));
            exposed.QueryBlock(block);

            // block should still stay as pending
            block.Status.Should().Equal(BlockStatus.Confirmed);
        }

        [Fact]
        private void QueryBlockTest_WithOprhanedPoolOutputCategory_ShouldGetOrphaned()
        {
            // test case: we supply a pending block which should stay as pending as pool output category is still 'immature'.
            var block = new PersistedBlock(1, false, false, false, "BLOCK_HASH", "TX_HASH", 0, 0, DateTime.Now);
            _poolConfig.Wallet.Adress.Returns("POOL_ADDRESS");

            _daemonClient.GetBlock("BLOCK_HASH").Returns(info => new Block { Tx = new List<string> { "TX_HASH" } });
            _daemonClient.GetTransaction("TX_HASH").Returns(info => new Transaction
            {
                Details = new List<TransactionDetail> { new TransactionDetail { Address = "POOL_ADDRESS", Category = "orphan" } }
            });

            // query the block.
            var exposed = Exposed.From(new BlockProcessor(_poolConfig, _daemonClient, _storageLayer));
            exposed.QueryBlock(block);

            // block should still stay as pending
            block.Status.Should().Equal(BlockStatus.Orphaned);
        }
    }
}
