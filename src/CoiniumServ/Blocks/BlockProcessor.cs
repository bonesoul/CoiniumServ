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

using System.Diagnostics;
using System.Linq;
using CoiniumServ.Daemon;
using CoiniumServ.Daemon.Errors;
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
        public bool Active { get; private set; }

        private readonly IPoolConfig _poolConfig;

        private readonly IDaemonClient _daemonClient;

        private readonly IStorageLayer _storageLayer;

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

            Active = true;
        }

        public void Run()
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

            if (pendingCount > 0)
                _logger.Information("Queried {0} pending blocks; {1} got confirmed, {2} got orphaned; took {3:0.000} seconds", pendingCount, confirmedCount, orphanedCount, (float)_stopWatch.ElapsedMilliseconds / 1000);
            else
                _logger.Information("No pending blocks found");

            _stopWatch.Reset();
        }

        private void QueryBlock(IPersistedBlock block)
        {
            var blockInfo = GetBlockInfo(block); // query the block.

            if (blockInfo == null) // make sure we got a valid blockInfo response before continuing on 
                return; // in case we have a null, status will be already decided by GetBlockInfo() function.

            if (blockInfo.Confirmations == -1) // check if block is orphaned already.
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
            var genTx = GetGenerationTx(block);

            if (genTx == null) // make sure we were able to read the generation transaction
                return; // in case we have a null, status will be already decided by GetGenerationTx() function.

            // get the output transaction that targets pools central wallet.
            var poolOutput = genTx.GetPoolOutput(_poolConfig.Wallet.Adress, _poolAccount);

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

        private Block GetBlockInfo(IPersistedBlock block)
        {
            try
            {
                return _daemonClient.GetBlock(block.BlockHash); // query the block.
            }
            catch (RpcTimeoutException)
            {
                // on rpc-timeout exception, let block stay in pending status so we can query it again later.
                block.Status = BlockStatus.Pending;
                return null;
            }
            catch (RpcConnectionException)
            {
                // on rpc-connection exception, let block stay in pending status so we can query it again later.
                block.Status = BlockStatus.Pending;
                return null;
            }
            catch (RpcErrorException e)
            {
                // block not found
                if (e.Code == (int)RpcErrorCode.RPC_INVALID_ADDRESS_OR_KEY)
                    block.Status = BlockStatus.Orphaned; // orphan the block if block does not exist.

                // if we got an error that we do not handle
                else
                {
                    block.Status = BlockStatus.Pending; // let the block in pending status so we can query it again later.
                    _logger.Error("Unhandled rpc-error: {0:l}", e.Message);
                }

                return null;
            }
            catch (RpcException e)
            {
                block.Status = BlockStatus.Pending; // and let block stay in pending status so we can query it again later.
                _logger.Error("Unhandled rpc-exception: {0:l}", e.Message);
                return null;
            }
        }

        private Transaction GetGenerationTx(IPersistedBlock block)
        {
            try
            {
                return _daemonClient.GetTransaction(block.TransactionHash); // query the transaction
            }
            catch (RpcTimeoutException)
            {
                // on rpc-timeout exception, let block stay in pending status so we can query it again later.
                block.Status = BlockStatus.Pending;
                return null;
            }
            catch (RpcConnectionException)
            {
                // on rpc-connection exception, let block stay in pending status so we can query it again later.
                block.Status = BlockStatus.Pending;
                return null;
            }
            catch (RpcErrorException e)
            {
                // Invalid or non-wallet transaction id
                if (e.Code == (int) RpcErrorCode.RPC_INVALID_ADDRESS_OR_KEY)
                    block.Status = BlockStatus.Orphaned; // orphan the block if block does not exist.

                // if we got an error that we do not handle
                else
                {                    
                    block.Status = BlockStatus.Pending; // let the block in pending status so we can query it again later.
                    _logger.Error("Unhandled rpc-error: {0:l}", e.Message);
                }

                return null;
            }
            catch (RpcException e)
            {
                block.Status = BlockStatus.Pending; // and let block stay in pending status so we can query it again later.
                _logger.Error("Unhandled rpc-exception: {0:l}", e.Message);
                return null;
            }
        }

        private void FindPoolAccount()
        {
            try
            {
                _poolAccount = !_poolConfig.Coin.Options.UseDefaultAccount // if UseDefaultAccount is not set
                    ? _daemonClient.GetAccount(_poolConfig.Wallet.Adress) // find the account of the our pool address.
                    : ""; // use the default account.
            }
            catch (RpcException e)
            {
                _logger.Error("Error getting account for pool central wallet address: {0:l} - {1:l}", _poolConfig.Wallet.Adress, e.Message);
            }
        }
    }
}
