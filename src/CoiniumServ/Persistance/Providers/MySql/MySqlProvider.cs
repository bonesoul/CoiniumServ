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

using CoiniumServ.Pools;
using MySql.Data.MySqlClient;
using Serilog;

namespace CoiniumServ.Persistance.Providers.MySql
{
    // TODO: dapper doesn't manage connections life-cyle - check this pattern: http://stackoverflow.com/questions/23023534/managing-connection-with-non-buffered-queries-in-dapper

    public class MySqlProvider : IMySqlProvider
    {
        public bool IsConnected { get; private set; }
        public MySqlConnection Connection { get; private set; }

        private readonly ILogger _logger;

        public MySqlProvider(PoolConfig poolConfig)
        {
            _logger = Log.ForContext<MySqlProvider>().ForContext("Component", poolConfig.Coin.Name);

            Initialize();
        }
        private void Initialize()
        {
            Connection = new MySqlConnection("Server=10.0.0.13;Port=3306;Database=mpos;Uid=root;Password=123456");
            Connection.Open();
        }
    }
}
