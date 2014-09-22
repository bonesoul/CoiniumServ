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
using System.Diagnostics;
using System.Linq;
using System.Threading;
using CoiniumServ.Daemon;
using CoiniumServ.Daemon.Exceptions;
using CoiniumServ.Daemon.Responses;
using CoiniumServ.Persistance.Blocks;
using CoiniumServ.Persistance.Layers;
using CoiniumServ.Pools;
using Serilog;

namespace CoiniumServ.Blocks
{
    public class BlockProcessor:IBlockProcessor
    {
        private readonly IPoolConfig _poolConfig;

        private readonly IDaemonClient _daemonClient;

        private readonly IStorageLayer _storageLayer;

        private readonly Timer _timer;

        private readonly Stopwatch _stopWatch = new Stopwatch();

        private readonly ILogger _logger;

        private string _poolAccount;

        public BlockProcessor(IPoolConfig poolConfig, IDaemonClient daemonClient, IStorageLayer storageLayer)
        {
            _poolConfig = poolConfig;
            _daemonClient = daemonClient;
            _storageLayer = storageLayer;
            _logger = Log.ForContext<BlockProcessor>().ForContext("Component", poolConfig.Coin.Name);

            FindPoolAccount();

            // setup the timer to run calculations.  
            _timer = new Timer(Run, null, _poolConfig.Payments.Interval * 1000, Timeout.Infinite);            
        }

        private void Run(object state)
        {
            _stopWatch.Start();

            var pendingBlocks = _storageLayer.GetPendingBlocks();
            var pendingCount = 0;
            var confirmedCount = 0;
            var orphanedCount = 0;

            foreach (var block in pendingBlocks)
            {
                pendingCount++;

                QueryBlock(block); // query the block status

                switch (block.Status)
                {
                    case BlockStatus.Pending:
                        break;
                    case BlockStatus.Orphaned:
                        _storageLayer.MoveOrphanedShares(block); // move existing shares for contributed miners to current round.
                        _storageLayer.UpdateBlock(block); // update block in our persistance layer.
                        orphanedCount++;             
                        break;
                    case BlockStatus.Confirmed:
                        _storageLayer.UpdateBlock(block); // update block in our persistance layer.
                        confirmedCount++;
                        break;
                }              
            }

            if(pendingCount > 0)
                _logger.Information("Queried {0} pending blocks; {1} got confirmed, {2} got orphaned; took {3:0.000} seconds", pendingCount, confirmedCount, orphanedCount, (float)_stopWatch.ElapsedMilliseconds / 1000);
            else
                _logger.Information("No pending blocks found");

            _stopWatch.Reset();

            _timer.Change(_poolConfig.Payments.Interval * 1000, Timeout.Infinite); // reset the timer.
        }

        private void QueryBlock(IPersistedBlock block)
        {
            var blockInfo = GetBlockInfo(block.BlockHash); // query the block.

            if (blockInfo == null || blockInfo.Confirmations == -1) // make sure the block exists and is accepted.
            {
                block.Status = BlockStatus.Orphaned;
                return;
            }

            // calculate our expected generation transactions's hash
            var expectedTxHash = block.TransactionHash; // expected transaction hash
            var genTxHash = blockInfo.Tx.First(); // read the hash of very first (generation transaction) of the block

            // make sure our calculated and reported generation tx hashes match.
            if (expectedTxHash != genTxHash)
            {
                block.Status = BlockStatus.Orphaned;
                return;
            }

            // get the generation transaction.
            var genTx = GetGenerationTransaction(blockInfo);

            // make sure we were able to read the generation transaction
            if (genTx == null)
            {
                block.Status = BlockStatus.Orphaned;
                return;
            }

            // get the output transaction that targets pools central wallet.
            var poolOutput = GetPoolOutput(genTx);

            // make sure we have a valid reference to poolOutput
            if (poolOutput == null)
            {
                block.Status = BlockStatus.Orphaned;
                return;
            }

            // set the reward of the block to miners.
            block.Reward = (decimal) poolOutput.Amount;

            // find the block status
            switch (poolOutput.Category)
            {
                case "immature":
                    block.Status = BlockStatus.Pending;
                    break;
                case "orphan":
                    block.Status = BlockStatus.Orphaned;
                    break;
                case "generate":
                    block.Status = BlockStatus.Confirmed;
                    break;
            }
        }

        private void FindPoolAccount()
        {
            try
            {
                _poolAccount = _daemonClient.GetAccount(_poolConfig.Wallet.Adress);
            }
            catch (RpcException e)
            {
                _logger.Error("Error getting account for pool central wallet address: {0:l} - {1:l}", _poolConfig.Wallet.Adress, e.Message);
            }
        }

        public Block GetBlockInfo(string blockHash)
        {
            try
            {
                var block = _daemonClient.GetBlock(blockHash);
                return block;
            }
            catch (RpcException e)
            {
                _logger.Error("Queried block does not exist {0:l} - {1:l}", blockHash, e.Message);
                return null;
            }
        }

        public Transaction GetGenerationTransaction(Block block)
        {
            try
            {
                return _daemonClient.GetTransaction(block.Tx.First()); // query the transaction
            }
            catch (RpcException e)
            {
                _logger.Error("Queried transaction does not exist {0:l} - {1:l}", block.Tx.First(), e.Message);
                return null;
            }
        }

        public TransactionDetail GetPoolOutput(Transaction transaction)
        {
            return transaction == null ? null : transaction.GetPoolOutput(_poolConfig.Wallet.Adress, _poolAccount);
        }
    }
}
