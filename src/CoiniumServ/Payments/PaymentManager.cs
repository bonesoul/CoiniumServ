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
