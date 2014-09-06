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
using System.Linq;
using CoiniumServ.Daemon;
using CoiniumServ.Daemon.Exceptions;
using CoiniumServ.Daemon.Responses;
using CoiniumServ.Pools;
using Serilog;

namespace CoiniumServ.Blocks
{
    public class BlockProcessor:IBlockProcessor
    {
        private readonly IDaemonClient _daemonClient;

        private readonly IPoolConfig _poolConfig;

        private readonly ILogger _logger;

        private string _poolAccount;

        public BlockProcessor(IPoolConfig poolConfig, IDaemonClient daemonClient)
        {
            _poolConfig = poolConfig;
            _daemonClient = daemonClient;
            _logger = Log.ForContext<BlockProcessor>().ForContext("Component", poolConfig.Coin.Name);

            FindPoolAccount();
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

        public Block GetBlock(string blockHash)
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
            if (transaction == null)
                return null;

            return transaction.GetPoolOutput(_poolConfig.Wallet.Adress, _poolAccount);
        }
    }
}
