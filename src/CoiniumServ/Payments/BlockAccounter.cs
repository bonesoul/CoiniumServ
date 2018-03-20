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
using CoiniumServ.Accounts;
using CoiniumServ.Container;
using CoiniumServ.Persistance.Layers;
using CoiniumServ.Pools;
using Serilog;

namespace CoiniumServ.Payments
{
    public class BlockAccounter : IBlockAccounter
    {
        public bool Active { get; private set; }

        private readonly Stopwatch _stopWatch = new Stopwatch();

        private readonly IObjectFactory _objectFactory;

        private readonly IStorageLayer _storageLayer;

        private readonly IAccountManager _accountManager;

        private readonly ILogger _logger;

        public BlockAccounter(IPoolConfig poolConfig, IObjectFactory objectFactory, IStorageLayer storageLayer, IAccountManager accountManager)
        {
            _objectFactory = objectFactory;
            _storageLayer = storageLayer;
            _accountManager = accountManager;
            _logger = Log.ForContext<BlockAccounter>().ForContext("Component", poolConfig.Coin.Name);

            if (!poolConfig.Payments.Enabled) // make sure payments are enabled.
                return;

            Active = true;
        }

        public void Run()
        {
            _stopWatch.Start();

            // find that blocks that were confirmed but still unpaid.
            var unpaidBlocks = _storageLayer.GetUnpaidBlocks();

            // create the payouts.
            var rounds = unpaidBlocks.Select(block => _objectFactory.GetPaymentRound(block, _storageLayer, _accountManager)).ToList();

            // loop through rounds
            foreach (var round in rounds)
            {
                // commit awaiting payments for the round.
                foreach (var payment in round.Payments)
                {
                    _storageLayer.AddPayment(payment);
                }

                _storageLayer.RemoveShares(round); // remove the shares for the round.
                _storageLayer.UpdateBlock(round.Block); // set the block for the round as accounted.
            }

            if (rounds.Count > 0)
                _logger.Information("Accounted {0} confirmed rounds, took {1:0.000} seconds", rounds.Count, (float)_stopWatch.ElapsedMilliseconds / 1000);
            else
                _logger.Information("No pending blocks waiting to get accounted found");

            _stopWatch.Reset();
        }
    }
}
