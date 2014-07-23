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
using CoiniumServ.Banning;
using CoiniumServ.Coin.Config;
using CoiniumServ.Daemon.Config;
using CoiniumServ.Jobs.Manager;
using CoiniumServ.Miners;
using CoiniumServ.Payments;
using CoiniumServ.Persistance;
using CoiniumServ.Persistance.Redis;
using CoiniumServ.Server.Mining.Stratum.Config;
using CoiniumServ.Server.Mining.Vanilla.Config;
using Serilog;

namespace CoiniumServ.Pools.Config
{
    /// <summary>
    /// Reads and serves the configuration for a pool.
    /// </summary>
    public class PoolConfig : IPoolConfig
    {
        public bool Valid { get; private set; }
        public bool Enabled { get; private set; }
        public ICoinConfig Coin { get; private set; }
        public IDaemonConfig Daemon { get; private set; }
        public IWalletConfig Wallet { get; private set; }
        public IRewardsConfig Rewards { get; private set; }
        public IPaymentConfig Payments { get; private set; }
        public IMinerConfig Miner { get; private set; }
        public IJobConfig Job { get; private set; }
        public IStratumServerConfig Stratum { get; private set; }
        public IBanConfig Banning { get; private set; }
        public IStorageConfig Storage { get; private set; }
        public IVanillaServerConfig Vanilla { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PoolConfig"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="coinConfig"></param>
        public PoolConfig(dynamic config, ICoinConfig coinConfig)
        {
            try
            {
                Enabled = config.enabled ? config.enabled : false;

                if (Enabled == false) // if the configuration is not enabled
                    return; // just skip reading rest of the parameters.

                Coin = coinConfig;
                Daemon = new DaemonConfig(config.daemon);
                Wallet = new WalletConfig(config.wallet);
                Rewards = new RewardsConfig(config.rewards);
                Payments = new PaymentConfig(config.payments);
                Miner = new MinerConfig(config.miner);
                Job = new JobConfig(config.job);
                Stratum = new StratumServerConfig(config.stratum);
                Banning = new BanConfig(config.banning);
                Storage = new RedisConfig(config.storage.redis);
                Vanilla = new VanillaServerConfig(config.vanilla);

                Valid = true;
            }
            catch (Exception e)
            {
                Valid = false;
                Log.Logger.ForContext<PoolConfig>().Error(e, "Error loading pool configuration");
            }
        }
    }
}
