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
using CoiniumServ.Pools.Config;
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

        public TransactionDetail GetPoolOutput(Block block)
        {
            try
            {
                var genTx = _daemonClient.GetTransaction(block.Tx.First()); // query the transaction

                if (genTx == null) // make sure the generation transaction exists
                    return null;

                // check if coin includes output address data in transaction details.
                return genTx.Details.Any(x => x.Address == null)
                    ? genTx.Details.FirstOrDefault(x => x.Account == _poolAccount) // some coins doesn't include address field in outputs, so try to determine using the associated account name.
                    : genTx.Details.FirstOrDefault(x => x.Address == _poolConfig.Wallet.Adress); // if coin includes address field in outputs, just use it.
            }
            catch (RpcException e)
            {
                _logger.Error("Queried transaction does not exist {0:l} - {1:l}", block.Tx.First(), e.Message);
                return null;
            }
        }
    }
}
