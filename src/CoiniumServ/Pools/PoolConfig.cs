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

using System;
using CoiniumServ.Banning;
using CoiniumServ.Coin.Config;
using CoiniumServ.Daemon.Config;
using CoiniumServ.Jobs.Manager;
using CoiniumServ.Mining;
using CoiniumServ.Payments.Config;
using CoiniumServ.Persistance;
using CoiniumServ.Persistance.Layers.Mpos;
using CoiniumServ.Server.Mining.Getwork;
using CoiniumServ.Server.Mining.Stratum;
using Serilog;

namespace CoiniumServ.Pools
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
        public IMetaConfig Meta { get; private set; }
        public IWalletConfig Wallet { get; private set; }
        public IRewardsConfig Rewards { get; private set; }
        public IPaymentConfig Payments { get; private set; }
        public IMinerConfig Miner { get; private set; }
        public IJobConfig Job { get; private set; }
        public IStratumServerConfig Stratum { get; private set; }
        public IBanConfig Banning { get; private set; }
        public IStorageConfig Storage { get; private set; }
        public IGetworkServerConfig Getwork { get; private set; }

        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PoolConfig"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="coinConfig"></param>
        public PoolConfig(dynamic config, ICoinConfig coinConfig)
        {
            try
            {
                _logger = Log.ForContext<PoolConfig>().ForContext("Component", coinConfig.Name);

                Enabled = config.enabled ? config.enabled : false;

                if (Enabled == false) // if the configuration is not enabled
                    return; // just skip reading rest of the parameters.

                // load the sections.
                Coin = coinConfig; // assign the coin config.
                Daemon = new DaemonConfig(config.daemon);
                Meta = new MetaConfig(config.meta);
                Wallet = new WalletConfig(config.wallet);
                Rewards = new RewardsConfig(config.rewards);
                Payments = new PaymentConfig(config.payments);
                Miner = new MinerConfig(config.miner);
                Job = new JobConfig(config.job);
                Stratum = new StratumServerConfig(config.stratum);
                Banning = new BanConfig(config.banning);
                Storage = new StorageConfig(config.storage);
                Getwork = new GetworkServerConfig(config.getwork);

                // process extra checks
                if (Storage.Layer is MposStorageConfig)
                {
                    if (Payments.Enabled)
                    {
                        Payments.Disable();
                        _logger.Information("Disabled payment processor as it can not be enabled when MPOS mode is on");
                    }
                }

                Valid = true;
            }
            catch (Exception e)
            {
                Valid = false;
                _logger.Error(e, "Error loading pool configuration");
            }
        }
    }
}
