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
using CoiniumServ.Pools;
using MySql.Data.MySqlClient;
using Serilog;

namespace CoiniumServ.Persistance.Providers.MySql
{
    // TODO: dapper doesn't manage connections life-cyle - check this pattern: http://stackoverflow.com/questions/23023534/managing-connection-with-non-buffered-queries-in-dapper

    public class MySqlProvider : IMySqlProvider
    {
        public string ConnectionString { get; private set; }

        private readonly IMySqlProviderConfig _config;

        private readonly ILogger _logger;

        public MySqlProvider(IPoolConfig poolConfig, IMySqlProviderConfig config)
        {
            _logger = Log.ForContext<MySqlProvider>().ForContext("Component", poolConfig.Coin.Name);

            _config = config;

            Initialize();
        }
        private void Initialize()
        {
            try
            {
                ConnectionString = string.Format("Server={0};Port={1};Uid={2};Password={3};Database={4};",
                    _config.Host, _config.Port, _config.Username, _config.Password, _config.Database);

                using (var connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();
                    _logger.Information("Mysql storage initialized: {0:l}:{1}, database: {2:l}.", _config.Host, _config.Port, _config.Database);
                }                
            }
            catch (Exception e)
            {
                _logger.Error("Mysql storage initialization failed: {0:l}:{1}, database: {2:l} - {3:l}", _config.Host, _config.Port, _config.Database, e.Message);
            }
        }
    }
}
