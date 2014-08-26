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
using CoiniumServ.Daemon;
using CoiniumServ.Daemon.Exceptions;
using CoiniumServ.Miners;
using CoiniumServ.Payments;
using CoiniumServ.Persistance.Blocks;
using CoiniumServ.Persistance.Providers;
using CoiniumServ.Pools;
using CoiniumServ.Server.Mining.Stratum;
using CoiniumServ.Shares;
using Serilog;

namespace CoiniumServ.Persistance.Layers.Hybrid
{
    public class HybridStorageLayer : IStorageLayer
    {
        public bool SupportsShareStorage { get { return true; } }
        public bool SupportsBlockStorage { get { return true; } }
        public bool SupportsPaymentsStorage { get { return true; } }

        private readonly IDaemonClient _daemonClient;

        private readonly ILogger _logger;

        public HybridStorageLayer(IEnumerable<IStorageProvider> providers, IDaemonClient daemonClient, PoolConfig poolConfig)
        {
            _daemonClient = daemonClient;
            _logger = Log.ForContext<HybridStorageLayer>().ForContext("Component", poolConfig.Coin.Name);
        }

        public void AddShare(IShare share)
        {
            throw new NotImplementedException();
        }

        public void RemoveShares(IPaymentRound round)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, double> GetCurrentShares()
        {
            throw new NotImplementedException();
        }

        public Dictionary<uint, Dictionary<string, double>> GetShares(IList<IPaymentRound> rounds)
        {
            throw new NotImplementedException();
        }

        public void AddBlock(IShare share)
        {
            throw new NotImplementedException();
        }

        public void UpdateBlock(IPaymentRound round)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, int> GetTotalBlocks()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPersistedBlock> GetBlocks(BlockStatus status)
        {
            throw new NotImplementedException();
        }

        public bool Authenticate(IMiner miner)
        {
            // within current implementation of hybrid storage layer, we don't have users registered to a pool but they
            // just mine with supplying a valid coin wallet address as username. So we just need to make sure the username
            // is valid address against the coin network.
            try
            {
                return _daemonClient.ValidateAddress(miner.Username).IsValid; // if so validate it against coin daemon as an address.
            }
            catch (RpcException)
            {
                return false;
            }
        }

        public void UpdateDifficulty(IStratumMiner miner)
        {
            // with-in our current hybrid-storage-layer, we don't need to write difficulty to persistance layer.
            return;
        }
    }
}
