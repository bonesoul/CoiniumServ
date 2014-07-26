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
using System.IO;
using System.Linq;
using CoiniumServ.Coin.Config;
using CoiniumServ.Factories;
using CoiniumServ.Logging;
using CoiniumServ.Pools.Config;
using CoiniumServ.Server.Web;
using CoiniumServ.Utils.Helpers.IO;
using Serilog;

namespace CoiniumServ.Configuration
{
    public class ConfigManager:IConfigManager
    {
        public IWebServerConfig WebServerConfig { get; private set; }
        public ILogConfig LogConfig { get; private set; }
        public IStackConfig StackConfig { get; private set; }
        public List<IPoolConfig> PoolConfigs { get; private set; }

        private const string GlobalConfigFilename = "config/config.json"; // global config filename.
        private const string PoolConfigRoot = "config/pools"; // root of pool configs.
        private const string CoinConfigRoot = "config/coins"; // root of pool configs.

        private dynamic _defaultPoolConfig;

        private readonly IConfigFactory _configFactory;

        private ILogger _logger;

        public ConfigManager(IConfigFactory configFactory)
        {
            _configFactory = configFactory;

            PoolConfigs = new List<IPoolConfig>(); // list of pool configurations.

            ReadGlobalConfig(); // read the global config.
        }

        private void ReadGlobalConfig()
        {
            var data = JsonConfigReader.Read(GlobalConfigFilename); // read the global config data.

            if (data == null) // make sure it exists, else gracefully exists
            {                
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Couldn't read config/config.json! Make sure you rename config/config-example.json as config/config.json.");
                Console.ResetColor();

                Environment.Exit(-1);
            }

            StackConfig = new StackConfig(data.stack);
            WebServerConfig = new WebServerConfig(data.website);
            LogConfig = new LogConfig(data.logging);
        }

        public void Initialize()
        {
            LoadPoolConfigs();
        }

        private void LoadPoolConfigs()
        {
            _logger = Log.ForContext<ConfigManager>();
            _logger.Debug("Discovering enabled pool configs..");

            var files = FileHelpers.GetFilesByExtensionRecursive(PoolConfigRoot, ".json");

            foreach (var file in files)
            {
                var data = JsonConfigReader.Read(file);

                // check if we have a default.json pool config.
                var filename = Path.GetFileNameWithoutExtension(file);
                if (!string.IsNullOrEmpty(filename) && filename.Equals("default", StringComparison.OrdinalIgnoreCase)) 
                {
                    _defaultPoolConfig = data;
                    continue; // don't add the default.json to list of pools and yet again do not load the coinconfig data for it.
                }

                if (!data.enabled) // skip pools that are not enabled.
                    continue;

                var coinName = Path.GetFileNameWithoutExtension(data.coin);
                var coinConfig = GetCoinConfig(coinName);

                if (!coinConfig.Valid)
                {
                    _logger.Error("coins/{0:l}.json doesnt't contain a valid configuration, skipping pool configuration: pools/{1:l}.json", coinName, filename);
                    continue;
                }

                if(_defaultPoolConfig != null)
                    data = JsonConfig.Merger.Merge(data, _defaultPoolConfig); // if we do have a default.json config, merge with it.

                PoolConfigs.Add(_configFactory.GetPoolConfig(data, coinConfig));
            }

            _logger.Information("Discovered a total of {0} enabled pool configurations: {1:l}", PoolConfigs.Count,
                PoolConfigs.Select(config => config.Coin.Name).ToList());
        }

        private ICoinConfig GetCoinConfig(string name)
        {
            var fileName = string.Format("{0}/{1}.json", CoinConfigRoot, name);
            var data = JsonConfigReader.Read(fileName);
            var coinConfig = _configFactory.GetCoinConfig(data);

            return coinConfig;
        }
    }
}
