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
using System.Threading;
using CoiniumServ.Blocks;
using CoiniumServ.Pools;
using Serilog;

namespace CoiniumServ.Payments
{
    public class PaymentManager:IPaymentManager
    {
        private readonly Timer _timer;

        private readonly IPoolConfig _poolConfig;

        private readonly IList<IPaymentLabor> _labors;

        private readonly ILogger _logger;

        public PaymentManager(IPoolConfig poolConfig,  IBlockProcessor blockProcessor, IBlockAccounter blockAccounter, IPaymentProcessor paymentProcessor)
        {
            _poolConfig = poolConfig;
            _labors = new List<IPaymentLabor>
            {
                blockProcessor,
                blockAccounter, 
                paymentProcessor
            };

            _logger = Log.ForContext<PaymentManager>().ForContext("Component", poolConfig.Coin.Name);

            if (!_poolConfig.Payments.Enabled) // make sure payments are enabled.
                return;

            // setup the timer to run payment laberos 
            _timer = new Timer(Run, null, _poolConfig.Payments.Interval * 1000, Timeout.Infinite);
        }

        private void Run(object state)
        {
            // loop through each payment labors and execute them.
            foreach (var labor in _labors)
            {
                if (!labor.Active) // make sure labor is active
                    continue;

                labor.Run(); // run the labor.
            }

            _timer.Change(_poolConfig.Payments.Interval * 1000, Timeout.Infinite); // reset the timer.
        }
    }
}
