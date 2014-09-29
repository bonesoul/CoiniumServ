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
