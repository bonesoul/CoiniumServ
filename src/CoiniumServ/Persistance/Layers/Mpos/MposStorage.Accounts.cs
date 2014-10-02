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

namespace CoiniumServ.Persistance.Layers.Mpos
{
    public partial class MposStorage
    {
        public void AddAccount(IAccount account)
        {
            // this function is not supported as this functionality is handled by mpos itself.
			throw new NotSupportedException();
        }

        public IAccount GetAccountByUsername(string username)
        {
            try
            {
                if (!IsEnabled)
                    return null;

                using (var connection = new MySqlConnection(_mySqlProvider.ConnectionString))
                {
                    return connection.Query<Account>(
                        @"SELECT a.id, w.username, a.coin_address as address FROM pool_worker as w
                            INNER JOIN accounts as a ON a.id=w.account_id
                            WHERE w.username = @username",
                        new {username}).Single();
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
            throw new NotImplementedException();
        }

        public IAccount GetAccountById(int id)
        {
            // TODO: implement me!
            throw new NotImplementedException();
        }
    }
}
