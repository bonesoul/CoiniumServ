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
using System.Threading;
using CoiniumServ.Pools;
using Serilog;

namespace CoiniumServ.Payments.New
{
    public class NewPaymentProcessor:INewPaymentProcessor
    {
        private Timer _timer;

        private readonly Stopwatch _stopWatch = new Stopwatch();

        private readonly IPoolConfig _poolConfig;

        private readonly ILogger _logger;

        public NewPaymentProcessor(IPoolConfig poolConfig)
        {
            _poolConfig = poolConfig;
            _logger = Log.ForContext<BlockAccounter>().ForContext("Component", poolConfig.Coin.Name);

            if (!_poolConfig.Payments.Enabled) // make sure payments are enabled.
                return;

            // setup the timer to run calculations.  
            //_timer = new Timer(Run, null, _poolConfig.Payments.Interval * 1000, Timeout.Infinite);
            Run(null);
        }

        private void Run(object state)
        {
        }
    }
}
