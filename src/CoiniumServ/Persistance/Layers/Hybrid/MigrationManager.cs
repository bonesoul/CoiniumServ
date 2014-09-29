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

using System.Reflection;
using CoiniumServ.Persistance.Providers.MySql;
using CoiniumServ.Pools;
using FluentMigrator;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Initialization;
using MySql.Data.MySqlClient;
using Serilog;

namespace CoiniumServ.Persistance.Layers.Hybrid
{
    public class MigrationManager:IMigrationManager
    {
        private readonly IMySqlProvider _provider;

        private readonly ILogger _logger;

        public MigrationManager(IMySqlProvider provider, IPoolConfig poolConfig)
        {
            _provider = provider;
            _logger = Log.ForContext<MigrationManager>().ForContext("Component", poolConfig.Coin.Name);

            Check();
        }

        private void Check()
        {
            try
            {
                var announcer = new TextWriterAnnouncer(WriteLog);
                var assembly = Assembly.GetExecutingAssembly();
                var migrationContext = new RunnerContext(announcer);

                var options = new MigrationOptions {PreviewOnly = false, Timeout = 60};
                var factory = new FluentMigrator.Runner.Processors.MySql.MySqlProcessorFactory();
                var processor = factory.Create(_provider.ConnectionString, announcer, options);
                var runner = new MigrationRunner(assembly, migrationContext, processor);

                runner.MigrateUp(true);
            }
            catch (InvalidMigrationException e)
            {
                _logger.Error("An invalid migration exception occured; {0:l}", e.Message);
            }
            catch (MySqlException e)
            {
                _logger.Error("An exception occured while running migration manager; {0:l}", e.Message);
            }
        }

        private void WriteLog(string s)
        {
            if (!string.IsNullOrWhiteSpace(s)) // filter out empty lines
                _logger.Debug(s);
        }
    }

    public class MigrationOptions : IMigrationProcessorOptions
    {
        public bool PreviewOnly { get; set; }
        public string ProviderSwitches { get; set; }
        public int Timeout { get; set; }
    }
}
