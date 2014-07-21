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
using System.IO;
using System.Linq;
using CoiniumServ.Coin.Config;
using CoiniumServ.Factories;
using CoiniumServ.Mining.Pools.Config;
using CoiniumServ.Persistance.Redis;
using CoiniumServ.Server.Web;
using CoiniumServ.Utils.Helpers.IO;
using CoiniumServ.Utils.Logging;
using Serilog;

namespace CoiniumServ.Utils.Configuration
{
    public class ConfigManager:IConfigManager
    {
        public bool ConfigExists { get { return _globalConfigData != null; } }
        public IRedisConfig RedisConfig { get; private set; }
        public IWebServerConfig WebServerConfig { get; private set; }
        public ILogConfig LogConfig { get; private set; }

        public List<IPoolConfig> PoolConfigs { get; private set; }

        private const string GlobalConfigFilename = "config/config.json"; // global config filename.
        private const string PoolConfigRoot = "config/pools"; // root of pool configs.
        private const string CoinConfigRoot = "config/coins"; // root of pool configs.

        private readonly Dictionary<string, ICoinConfig> _coinConfigs; // cache for loaded coin configs. 

        private readonly dynamic _globalConfigData; // global config data.
        private readonly IConfigFactory _configFactory;
        private ILogger _logger;

        public ConfigManager(IConfigFactory configFactory)
        {
            _configFactory = configFactory;

            _globalConfigData = JsonConfigReader.Read(GlobalConfigFilename); // read the global config data.
            PoolConfigs = new List<IPoolConfig>();
            _coinConfigs =  new Dictionary<string, ICoinConfig>();

            LogConfig = new LogConfig(_globalConfigData.logging);
            RedisConfig = new RedisConfig(_globalConfigData.storage.redis);
            WebServerConfig = new WebServerConfig(_globalConfigData.website);            
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

                if (!data.enabled) // skip pools that are not enabled.
                    continue;

                var coinName = Path.GetFileNameWithoutExtension(data.coin);
                var coinConfig = GetCoinConfig(coinName);

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
