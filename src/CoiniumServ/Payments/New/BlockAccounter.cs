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
using System.Diagnostics;
using System.Linq;
using System.Threading;
using CoiniumServ.Factories;
using CoiniumServ.Persistance.Layers;
using CoiniumServ.Pools;
using Serilog;

namespace CoiniumServ.Payments.New
{
    public class BlockAccounter : IBlockAccounter
    {
        private Timer _timer;

        private readonly Stopwatch _stopWatch = new Stopwatch();

        private readonly IPoolConfig _poolConfig;

        private readonly IObjectFactory _objectFactory;

        private readonly IStorageLayer _storageLayer;

        private readonly ILogger _logger;

        public BlockAccounter(IPoolConfig poolConfig, IObjectFactory objectFactory, IStorageLayer storageLayer)
        {
            _poolConfig = poolConfig;
            _objectFactory = objectFactory;
            _storageLayer = storageLayer;

            if (!_poolConfig.Payments.Enabled) // make sure payments are enabled.
                return;

            _logger = Log.ForContext<BlockAccounter>().ForContext("Component", poolConfig.Coin.Name);

            // setup the timer to run calculations.  
            _timer = new Timer(Run, null, _poolConfig.Payments.Interval * 1000, Timeout.Infinite);
        }

        private void Run(object state)
        {
            _stopWatch.Start();

            // find that blocks that were confirmed but still unpaid.
            var unpaidBlocks = _storageLayer.GetAllUnpaidBlocks(); 

            // create the awaiting payment objects.
            var rounds = unpaidBlocks.Select(block => _objectFactory.GetPaymentRound(block, _storageLayer)).ToList();

            // loop through rounds
            foreach (var round in rounds)
            {
                _storageLayer.CommitPayoutsForRound(round); // commit awaiting payments for the round.
                _storageLayer.UpdateBlock(round.Block); // set the block for the round as accounted.
            }

            _logger.Information("Accounted {0} confirmed rounds, took {1:0.000} seconds", rounds.Count, (float) _stopWatch.ElapsedMilliseconds/1000);

            _stopWatch.Reset();

            _timer.Change(_poolConfig.Payments.Interval * 1000, Timeout.Infinite); // reset the timer.
        }
    }
}
