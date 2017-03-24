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
