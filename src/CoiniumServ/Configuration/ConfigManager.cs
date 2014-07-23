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
        public bool ConfigExists { get { return _globalConfigData != null; } }
        public IWebServerConfig WebServerConfig { get; private set; }
        public ILogConfig LogConfig { get; private set; }

        public List<IPoolConfig> PoolConfigs { get; private set; }

        private const string GlobalConfigFilename = "config/config.json"; // global config filename.
        private const string PoolConfigRoot = "config/pools"; // root of pool configs.
        private const string CoinConfigRoot = "config/coins"; // root of pool configs.

        private readonly Dictionary<string, ICoinConfig> _coinConfigs; // cache for loaded coin configs. 

        private readonly dynamic _globalConfigData; // global config data.
        private dynamic _defaultPoolConfig;
        private readonly IConfigFactory _configFactory;

        private ILogger _logger;

        public ConfigManager(IConfigFactory configFactory)
        {
            _configFactory = configFactory;

            _globalConfigData = JsonConfigReader.Read(GlobalConfigFilename); // read the global config data.
            PoolConfigs = new List<IPoolConfig>();
            _coinConfigs =  new Dictionary<string, ICoinConfig>();

            LogConfig = new LogConfig(_globalConfigData.logging);
            WebServerConfig = new WebServerConfig(_globalConfigData.website);

            // TODO: implement metrics config.
        }

        public void Initialize()
        {
            _logger = Log.ForContext<ConfigManager>();

            LoadPoolConfigs();
        }

        private void LoadPoolConfigs()
        {
            _logger.Verbose("Discovering enabled pool configs..");

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

                if(_defaultPoolConfig != null)
                    data = JsonConfig.Merger.Merge(_defaultPoolConfig, data); // if we do have a default.json config, merge with it.

                PoolConfigs.Add(_configFactory.GetPoolConfig(data, coinConfig));
            }

            _logger.Information("Discovered a total of {0} enabled pool configurations: {1:l}", PoolConfigs.Count,
                PoolConfigs.Select(config => config.Coin.Name).ToList());
        }

        private ICoinConfig GetCoinConfig(string name)
        {
            if (!_coinConfigs.ContainsKey(name))
            {
                var fileName = string.Format("{0}/{1}.json", CoinConfigRoot, name);
                var data = JsonConfigReader.Read(fileName);
                var coinConfig = _configFactory.GetCoinConfig(data);

                _coinConfigs.Add(name, coinConfig);
            }

            return _coinConfigs[name];            
        }
    }
}
