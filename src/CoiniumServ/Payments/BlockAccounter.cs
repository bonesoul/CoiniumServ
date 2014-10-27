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
