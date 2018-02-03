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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CoiniumServ.Coin.Config;
using CoiniumServ.Logging;
using CoiniumServ.Mining.Software;
using CoiniumServ.Pools;
using CoiniumServ.Server.Stack;
using CoiniumServ.Server.Web.Config;
using CoiniumServ.Statistics;
using CoiniumServ.Utils.Helpers;
using CoiniumServ.Utils.Platform;
using Serilog;

namespace CoiniumServ.Configuration
{
    public class ConfigManager:IConfigManager
    {
        public IStackConfig StackConfig { get; private set; }
        
        public IStatisticsConfig StatisticsConfig { get; private set; }
        
        public IWebServerConfig WebServerConfig { get; private set; }       

        public ILogConfig LogConfig { get; private set; }
        
        public List<IPoolConfig> PoolConfigs { get; private set; }

        public ISoftwareRepositoryConfig SoftwareRepositoryConfig { get; private set; }

        private const string GlobalConfigFilename = "config/config.json"; // global config filename.
        private const string SoftwareManagerConfigFilename = "config/software.json"; // software manager config filename.
        private const string PoolConfigRoot = "config/pools"; // root of pool configs.
        private const string CoinConfigRoot = "config/coins"; // root of pool configs.

        private dynamic _defaultPoolConfig;

        private readonly IConfigFactory _configFactory;
        private readonly IJsonConfigReader _jsonConfigReader;
        private readonly ILogManager _logManager;

        private readonly ILogger _logger;

        public ConfigManager(IConfigFactory configFactory, IJsonConfigReader jsonConfigReader, ILogManager logManager)
        {
            _configFactory = configFactory;
            _jsonConfigReader = jsonConfigReader;
            _logManager = logManager;
            _logger = Log.ForContext<ConfigManager>();

            LoadGlobalConfig(); // read the global config.
            LoadSoftwareManagerConfig(); // load software manager config file.
            LoadDefaultPoolConfig(); // load default pool config if exists.
            LoadPoolConfigs(); // load the per-pool config files.
        }

        private void LoadGlobalConfig()
        {
            var data = _jsonConfigReader.Read(GlobalConfigFilename); // read the global config data.

            // make sure we were able to load global config.
            if (data == null)
            {
                // gracefully exit
                _logger.Error("Couldn't read config/config.json! Make sure you rename config/config-example.json as config/config.json.");
                Environment.Exit(-1);
            }

            // load log config.
            LogConfig = new LogConfig(data.logging); // read the log config first, so rest of the config loaders can use log subsystem.
            _logManager.EmitConfiguration(LogConfig); // assign the log configuration to log manager.

            // print a version banner.
            _logger.Information("CoiniumServ {0:l} {1:l} warming-up..", VersionInfo.CodeName, Assembly.GetAssembly(typeof(Program)).GetName().Version);
            PlatformManager.PrintPlatformBanner();

            // load rest of the configs.
            StackConfig = new StackConfig(data.stack);
            StatisticsConfig = new StatisticsConfig(data.statistics);
            WebServerConfig = new WebServerConfig(data.website);
        }

        private void LoadSoftwareManagerConfig()
        {
            var data = _jsonConfigReader.Read(SoftwareManagerConfigFilename); // read the config data.
            SoftwareRepositoryConfig = new SoftwareRepositoryConfig(_configFactory, data);
        }

        private void LoadDefaultPoolConfig()
        {
            var data = _jsonConfigReader.Read(string.Format("{0}/default.json", PoolConfigRoot));
            _defaultPoolConfig = data ?? null; // set the default config data.
        }

        private void LoadPoolConfigs()
        {
            PoolConfigs = new List<IPoolConfig>(); // list of pool configurations.
            var files = FileHelpers.GetFilesByExtension(PoolConfigRoot, ".json");

            foreach (var file in files)
            {
                var filename = Path.GetFileNameWithoutExtension(file); // read the filename.
                if (!string.IsNullOrEmpty(filename) && filename.Equals("default", StringComparison.OrdinalIgnoreCase)) // if it's the default.json,
                    continue; // just skip it.

                var data = _jsonConfigReader.Read(file); // read the pool config json.

                if (data == null) // make sure we have loaded json data.
                    continue;

                if (!data.enabled) // skip pools that are not enabled.
                    continue;

                var coinName = Path.GetFileNameWithoutExtension(data.coin); // get the coin-name assigned to pool.
                var coinConfig = GetCoinConfig(coinName); // get the coin config.

                if (coinConfig == null) // make sure a configuration file for referenced coin exists.
                {
                    _logger.Error("Referenced coin configuration file coins/{0:l}.json doesn't exist, skipping pool configuration: pools/{1:l}.json", coinName, filename);
                    continue;
                }

                if (!coinConfig.Valid) // make sure the configuration for referenced coin is valid.
                {
                    _logger.Error("coins/{0:l}.json doesnt't contain a valid coin configuration, skipping pool configuration: pools/{1:l}.json", coinName, filename);
                    continue;
                }

                if (_defaultPoolConfig != null) // if we do have a default.json config
                    data = JsonConfig.Merger.Merge(data, _defaultPoolConfig); // merge with it.

                PoolConfigs.Add(_configFactory.GetPoolConfig(data, coinConfig));
            }

            _logger.Information("Discovered a total of {0} enabled pool configurations: {1:l}", PoolConfigs.Count,
                PoolConfigs.Select(config => config.Coin.Name).ToList());
        }

        public ICoinConfig GetCoinConfig(string name)
        {
            var fileName = string.Format("{0}/{1}.json", CoinConfigRoot, name);
            var data = _jsonConfigReader.Read(fileName);

            if (data == null) // make sure we were able to read the coin configuration file.
                return null;

            var coinConfig = _configFactory.GetCoinConfig(data);

            return coinConfig;
        }
    }
}
