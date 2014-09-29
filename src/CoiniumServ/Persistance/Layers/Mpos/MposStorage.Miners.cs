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
using System.Linq;
using CoiniumServ.Mining;
using CoiniumServ.Server.Mining.Stratum;
using Dapper;
using MySql.Data.MySqlClient;

namespace CoiniumServ.Persistance.Layers.Mpos
{
    public partial class MposStorage
    {
        public bool Authenticate(IMiner miner)
        {
            try
            {
                if (!IsEnabled)
                    return false;

                using (var connection = new MySqlConnection(_mySqlProvider.ConnectionString))
                {
                    // query the username against mpos pool_worker table.
                    var result = connection.Query<string>(
                        "SELECT password FROM pool_worker WHERE username = @username",
                        new {username = miner.Username}).FirstOrDefault();

                    // if matching record exists for given miner username, then authenticate the miner.
                    // note: we don't check for password on purpose.
                    return result != null;
                }
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while reading pool_worker table; {0:l}", e.Message);
                return false;
            }
        }

        public void UpdateDifficulty(IStratumMiner miner)
        {
            try
            {
                if (!IsEnabled)
                    return;

                using (var connection = new MySqlConnection(_mySqlProvider.ConnectionString))
                {
                    connection.Execute(
                        "UPDATE pool_worker SET difficulty = @difficulty WHERE username = @username", new
                        {
                            difficulty = miner.Difficulty,
                            username = miner.Username
                        });
                }
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while updating difficulty for miner; {0:l}", e.Message);
            }
        }
    }
}
