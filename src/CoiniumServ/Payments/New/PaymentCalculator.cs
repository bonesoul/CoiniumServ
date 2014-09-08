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

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CoiniumServ.Factories;
using CoiniumServ.Persistance.Layers;
using CoiniumServ.Pools;
using Serilog;

namespace CoiniumServ.Payments.New
{
    public class PaymentCalculator : IPaymentCalculator
    {
        private Timer _timer;

        private readonly IPoolConfig _poolConfig;

        private readonly IObjectFactory _objectFactory;

        private readonly IStorageLayer _storageLayer;

        private readonly ILogger _logger;

        public PaymentCalculator(IPoolConfig poolConfig, IObjectFactory objectFactory, IStorageLayer storageLayer)
        {
            _poolConfig = poolConfig;
            _objectFactory = objectFactory;
            _storageLayer = storageLayer;

            _logger = Log.ForContext<PaymentCalculator>().ForContext("Component", poolConfig.Coin.Name);

            // setup the timer to run calculations.  
            //_timer = new Timer(Run, null, _poolConfig.Payments.Interval * 1000, Timeout.Infinite);

            Run(null);
        }

        private void Run(object state)
        {
            if (!_poolConfig.Payments.Enabled)
                return;

            // TODO: in our new payment-subsystem, block confirmations are no longer handled by the payment-processor but it should be done via block-processor.

            // find that blocks that were confirmed but still unpaid.
            var unpaidBlocks = _storageLayer.GetAllUnpaidBlocks(); 

            // create the awaiting payment objects.
            var awaitingPayments = unpaidBlocks.Select(block => _objectFactory.GetAwaitingPayment(block, _storageLayer)).ToList();
        }
    }
}
