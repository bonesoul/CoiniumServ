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
using CoiniumServ.Daemon.Exceptions;
using CoiniumServ.Daemon.Responses;
using CoiniumServ.Pools.Config;
using Serilog;

namespace CoiniumServ.Blocks
{
    public class BlockProcessor:IBlockProcessor
    {
        private readonly IDaemonClient _daemonClient;

        private readonly IPoolConfig _poolConfig;

        private readonly ILogger _logger;

        public BlockProcessor(IPoolConfig poolConfig, IDaemonClient daemonClient)
        {
            _poolConfig = poolConfig;
            _daemonClient = daemonClient;
            _logger = Log.ForContext<BlockProcessor>().ForContext("Component", poolConfig.Coin.Name);
        }

        public bool GetBlockDetails(string blockHash, out Block block, out Transaction generationTransaction)
        {
            block = null;
            generationTransaction = null;

            try
            {
                // query the block.
                block = _daemonClient.GetBlock(blockHash);

                if (block.Confirmations == -1) // check if block is just reported being orphan by coin daemon.
                    return false;

                // read the very first (generation transaction) of the block
                var generationTx = block.Tx.First();

                // also make sure the transaction includes our pool wallet address.
                generationTransaction = _daemonClient.GetTransaction(generationTx);

                return true;
            }
            catch (RpcException e)
            {
                _logger.Error("Queried block does not exist {0:l}, {1:l}", blockHash, e.Message);
                return false;
            }
        }

        public bool CheckGenTxHash(Block block, string expectedTxHash)
        {
            // get the generation transaction for the block.
            var genTx = block.Tx.First();
          
            if (expectedTxHash == genTx)
                return true;

            Log.Debug("Queried block {0:l} doesn't seem to belong us as reported generation transaction hash [{1:l}] doesn't match our expected one [{2:l}]", block.Hash, genTx, expectedTxHash);
            return false;
        }

        public bool ContainsPoolOutput(Transaction transaction)
        {
            // check if the transaction includes output for the configured central pool wallet address.
            var gotPoolOutput = transaction.Details.Any(x => x.Address == _poolConfig.Wallet.Adress);

            if (gotPoolOutput) // if we got the correct pool output
                return true; // then the block seems to belong us.

            Log.Debug("Queried block doesn't seem to belong us as generation transaction doesn't contain an output for pool's central wallet address: {0:}", _poolConfig.Wallet.Adress);
            return false;            
        }
    }
}
