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
