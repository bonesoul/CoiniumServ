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
using System.Threading;
using Coinium.Daemon;
using Coinium.Daemon.Exceptions;
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

        public PaymentProcessor(IDaemonClient daemonClient, IStorage storage)
        {
            _daemonClient = daemonClient;
            _storage = storage;
        }

        public void Initialize(IPaymentConfig config)
        {
            _config = config;

            IsEnabled = _config.Enabled;

            if (IsEnabled)
            {
                _timer = new Timer(RunPayments, null, _config.Interval * 1000, Timeout.Infinite); // setup the timer to run payments.  
            }
        }

        private void RunPayments(object state)
        {
            var pendingBlocks = _storage.GetPendingBlocks();
            QueryPendingBlocks(pendingBlocks);

            Log.ForContext<PaymentProcessor>().Information("Payment processor ran.");

            _timer.Change(_config.Interval * 1000, Timeout.Infinite); // reset the payments timer.
        }

        private void QueryPendingBlocks(IEnumerable<IPersistedBlock> blocks)
        {

            foreach (var block in blocks)
            {
                try
                {
                    var test = _daemonClient.GetTransaction(block.TransactionHash + "a");
                }
                catch (DaemonException exception)
                {
                    if (exception.Error.Code == -5)
                        Log.ForContext<PaymentProcessor>().Error("Kicking block {0} as daemon reported invalid generation transaction id: {1}.", block.Height, block.TransactionHash);
                    else
                        Log.ForContext<PaymentProcessor>().Error("Kicking block {0} as daemon reported an unknown error with generation transaction: {1:l} error: {2:l} [{3}].", block.Height, block.TransactionHash, exception.Error.Message, exception.Error.Code);
                    
                    block.Status=PersistedBlockStatus.Kicked; // kic the block.
                }
            }

        }
    }
}
