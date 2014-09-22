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
using System.IO;
using CoiniumServ.Coin.Config;
using CoiniumServ.Configuration;
using Serilog;

namespace CoiniumServ.Daemon.Config
{
    public class StandaloneDaemonConfig:IStandaloneDaemonConfig
    {
        public bool Valid { get; private set; }
        public bool Enabled { get; private set; }
        public ICoinConfig Coin { get; private set; }
        public IDaemonConfig Daemon { get; private set; }

        private readonly ILogger _logger;

        public StandaloneDaemonConfig(IConfigManager configManager, dynamic config)
        {
            try
            {
                _logger = Log.ForContext<StandaloneDaemonConfig>();

                Enabled = config.enabled ? config.enabled : false;

                if (Enabled == false) // if the configuration is not enabled
                    return; // just skip reading rest of the parameters.
                
                // load the sections.

                var coinName = Path.GetFileNameWithoutExtension(config.coin); // get the coin-name assigned to pool.
                var coinConfig = configManager.GetCoinConfig(coinName); // get the coin config.

                if (coinConfig == null) // make sure a configuration file for referenced coin exists.
                {
                    _logger.Error("Referenced coin configuration file coins/{0:l}.json doesn't exist, skipping stand-alone daemon configuration", coinName);
                    Valid = false;
                    return;
                }

                if (!coinConfig.Valid) // make sure the configuration for referenced coin is valid.
                {
                    _logger.Error("coins/{0:l}.json doesnt't contain a valid coin configuration, skipping stand-alone daemon configuration", coinName);
                    Valid = false;
                    return;
                }

                Coin = coinConfig; // assign the coin config.
                Daemon = new DaemonConfig(config.daemon);

                Valid = true;
            }
            catch (Exception e)
            {
                Valid = false;
                _logger.Error(e, "Error loading wallet configuration");
            }
        }
    }
}
