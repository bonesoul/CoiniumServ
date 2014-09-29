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
using System.Collections.Generic;
using Dapper;
using MySql.Data.MySqlClient;

namespace CoiniumServ.Persistance.Layers.Mpos
{
    public partial class MposStorage
    {
        public IDictionary<string, double> GetHashrateData(int since)
        {
            var hashrateData = new Dictionary<string, double>();

            try
            {
                if (!IsEnabled)
                    return hashrateData;

                using (var connection = new MySqlConnection(_mySqlProvider.ConnectionString))
                {
                    var results = connection.Query(
                            @"SELECT username, sum(difficulty) AS shares FROM shares WHERE our_result='Y' GROUP BY username");

                    foreach (var row in results)
                    {
                        hashrateData.Add(row.username, row.shares);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while getting share data: {0:l}", e.Message);
            }

            return hashrateData;
        }

        public void DeleteExpiredHashrateData(int until)
        {
            // MPOS doesn't use another hashrate data structure except the shares, which are already handled by MPOS.
        }
    }
}
