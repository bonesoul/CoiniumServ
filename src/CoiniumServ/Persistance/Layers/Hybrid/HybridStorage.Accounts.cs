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
using CoiniumServ.Accounts;
using Dapper;
using MySql.Data.MySqlClient;

namespace CoiniumServ.Persistance.Layers.Hybrid
{
    public partial class HybridStorage
    {
        public void AddAccount(IAccount account)
        {
            try
            {
                if (!IsEnabled)
                    return;

                using (var connection = new MySqlConnection(_mySqlProvider.ConnectionString))
                {
                    connection.Execute(
                        @"INSERT INTO Account(Username, Address, CreatedAt) VALUES (@username, @address, @createdAt)",
                        new
                        {
                            username = account.Username,
                            address = account.Address,
                            createdAt = DateTime.Now
                        });
                }
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while creating account; {0:l}", e.Message);
            }
        }

        public IAccount GetAccountByUsername(string username)
        {
            try
            {
                if (!IsEnabled)
                    return null;

                using (var connection = new MySqlConnection(_mySqlProvider.ConnectionString))
                {
                    return connection.Query<Account>("SELECT Id, Username, Address FROM Account WHERE Username = @username",
                        new { username }).Single();
                }
            }
            catch (InvalidOperationException) // fires when no result is found.
            {
                return null;
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while getting account; {0:l}", e.Message);
                return null;
            }
        }

        public IAccount GetAccountByAddress(string address)
        {
            try
            {
                if (!IsEnabled)
                    return null;

                using (var connection = new MySqlConnection(_mySqlProvider.ConnectionString))
                {
                    return connection.Query<Account>("SELECT Id, Username, Address FROM Account WHERE Address = @address",
                        new { address }).Single();
                }
            }
            catch (InvalidOperationException) // fires when no result is found.
            {
                return null;
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while getting account; {0:l}", e.Message);
                return null;
            }
        }

        public IAccount GetAccountById(int id)
        {
            try
            {
                if (!IsEnabled)
                    return null;

                using (var connection = new MySqlConnection(_mySqlProvider.ConnectionString))
                {
                    return connection.Query<Account>("SELECT Id, Username, Address FROM Account WHERE Id = @id",
                        new { id }).Single();
                }
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while getting account; {0:l}", e.Message);
                return null;
            }
        }
    }
}
