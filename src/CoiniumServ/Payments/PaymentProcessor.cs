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
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Coinium.Coin.Address.Exceptions;
using Coinium.Daemon;
using Coinium.Daemon.Exceptions;
using Coinium.Daemon.Responses;
using Coinium.Mining.Pools;
using Coinium.Persistance;
using Serilog;

namespace Coinium.Payments
{
    public class PaymentProcessor : IPaymentProcessor
    {
        public bool IsEnabled { get; private set; }

        private readonly IDaemonClient _daemonClient;
        private readonly IStorage _storage;
        private IPaymentConfig _config;
        private Timer _timer;

        private readonly object _paymentsLock = new object();
        private readonly Stopwatch _stopWatch = new Stopwatch();

        private const string PoolWallet = "n3Mvrshbf4fMoHzWZkDVbhhx4BLZCcU9oY";

        public PaymentProcessor(IDaemonClient daemonClient, IStorage storage)
        {
            _daemonClient = daemonClient;
            _storage = storage;
        }

        public void Initialize(IPaymentConfig config)
        {
            _config = config;

            IsEnabled = _config.Enabled;

            if (!IsEnabled) 
                return;

            // validate the pool wallet.
            var validationResult = _daemonClient.ValidateAddress(PoolWallet);

            if (!validationResult.IsValid || !validationResult.IsMine) // make sure the pool central wallet address is valid and belongs to the daemon we are connected to.
            {
                Log.ForContext<PaymentProcessor>().Error("Halted as daemon we are connected to does not own the pool address: {0:l}.",PoolWallet);
                return;
            }

            // determine the satoshis in the coin.
            var balanceResult = _daemonClient.GetBalance();


            // setup the timer to run payments.  
            _timer = new Timer(RunPayments, null, _config.Interval * 1000, Timeout.Infinite);
        }

        private void RunPayments(object state)
        {
            lock (_paymentsLock)
            {
                _stopWatch.Start();

                var pendingBlocks = _storage.GetPendingBlocks();
                QueryPendingBlocks(pendingBlocks);

                Log.ForContext<PaymentProcessor>().Information("Payments processed - took {0:0.00} seconds.", (float)_stopWatch.ElapsedMilliseconds/1000);
                _stopWatch.Reset();

                _timer.Change(_config.Interval*1000, Timeout.Infinite); // reset the payments timer.
            }
        }

        private void QueryPendingBlocks(IEnumerable<IPersistedBlock> blocks)
        {
            foreach (var block in blocks)
            {
                try
                {
                    var transaction = _daemonClient.GetTransaction(block.TransactionHash);
                    
                    // get the output transaction that targets pools central wallet.
                    var poolOutput = transaction.Details.FirstOrDefault(output => output.Address == PoolWallet);

                    // make sure output for the pool central wallet exists
                    if (poolOutput == null)
                    {
                        Log.ForContext<PaymentProcessor>().Error("Kicking block {0} as transaction doesn't contain output for the pool's central wallet.", block.Height);
                        block.Status = PersistedBlockStatus.Kicked; // kick the block.
                    }
                    else
                    {
                        // possible categories:
                        // src/rpcwallet.cpp:961:  entry.push_back(Pair("category", "send"));
                        // src/rpcwallet.cpp:986:  entry.push_back(Pair("category", "orphan"));
                        // src/rpcwallet.cpp:988:  entry.push_back(Pair("category", "immature"));
                        // src/rpcwallet.cpp:990:  entry.push_back(Pair("category", "generate"));
                        // src/rpcwallet.cpp:993:  entry.push_back(Pair("category", "receive"));
                        // src/rpcwallet.cpp:1011: entry.push_back(Pair("category", "move"));

                        switch (poolOutput.Category)
                        {
                            case "immature":
                                block.Status = PersistedBlockStatus.Pending;
                                break;
                            case "orphan":
                                block.Status = PersistedBlockStatus.Orphan;
                                break;
                            case "generate":
                                block.Status = PersistedBlockStatus.Confirmed;
                                break;
                            default: // send, recieve, move - TODO: we shouldn't be seing these categories! Implement an error message and kick it may be?
                                block.Status = PersistedBlockStatus.Pending;
                                break;
                        }

                        // get the amount.
                        var amount = poolOutput.Amount;
                    }
                }
                catch (DaemonException exception)
                {
                    if (exception.Error.Code == -5)
                        Log.ForContext<PaymentProcessor>().Error("Kicking block {0} as daemon reported invalid generation transaction id: {1}.", block.Height, block.TransactionHash);
                    else
                        Log.ForContext<PaymentProcessor>().Error("Kicking block {0} as daemon reported an unknown error with generation transaction: {1:l} error: {2:l} [{3}].", block.Height, block.TransactionHash, exception.Error.Message, exception.Error.Code);

                    block.Status = PersistedBlockStatus.Kicked; // kick the block.
                }
            }
        }
    }
}
