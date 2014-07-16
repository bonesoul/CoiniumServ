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
using System.IO;
using Coinium.Coin.Config;
using Coinium.Daemon.Config;
using Coinium.Payments;
using Coinium.Server.Mining.Stratum.Config;
using Coinium.Server.Mining.Vanilla.Config;

namespace Coinium.Mining.Pools.Config
{
    /// <summary>
    /// Reads and serves the configuration for a pool.
    /// </summary>
    public class PoolConfig : IPoolConfig
    {
        public bool Valid { get; private set; }

        public bool Enabled { get; private set; }
        public IWalletConfig Wallet { get; private set; }

        public ICoinConfig Coin { get; private set; }

        public IStratumServerConfig Stratum { get; private set; }

        public IVanillaServerConfig Vanilla { get; private set; }

        /// <summary>
        /// Gets the daemon configuration.
        /// </summary>
        public IDaemonConfig Daemon { get; private set; }

        public IRewardsConfig Rewards { get; private set; }

        public IPaymentConfig Payments { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PoolConfig"/> class.
        /// </summary>
        /// <param name="coinConfigFactory">The coin configuration factory.</param>
        /// <param name="config">The configuration.</param>
        public PoolConfig(ICoinConfigFactory coinConfigFactory, dynamic config)
        {
            if (config == null)
            {
                Valid = false;
                return;
            }

            Enabled = config.enabled ? config.enabled : false;

            if (Enabled == false)
                return;

            Wallet = new WalletConfig(config.wallet);
            Rewards = new RewardsConfig(config.rewards);

            var coinName = Path.GetFileNameWithoutExtension(config.coin);
            Coin = coinConfigFactory.GetConfig(coinName);

            if (Coin == null)
            {
                Valid = false;
                return;
            }

            Stratum = new StratumServerConfig(config.stratum);
            Vanilla = new VanillaServerConfig(config.vanilla);

            if (Stratum == null && Vanilla == null)
            {
                Valid = false;
                return;
            }

            Daemon = new DaemonConfig(config.daemon);

            if (Daemon == null)
            {
                Valid = false;
                return;
            }

            Payments = new PaymentConfig(config.payments);
            if (Payments == null)
            {
                Valid = false;
                return;
            }

            Valid = true;
        }
    }
}
