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
using CoiniumServ.Payments;
using CoiniumServ.Server.Web.Models.Pool;
using Dapper;
using MySql.Data.MySqlClient;

namespace CoiniumServ.Persistance.Layers.Hybrid
{
    public partial class HybridStorage
    {
        public void AddPayment(IPayment payment)
        {
            try
            {
                if (!IsEnabled)
                    return;

                using (var connection = new MySqlConnection(_mySqlProvider.ConnectionString))
                {
                    connection.Execute(
                        @"INSERT INTO Payment(Block, AccountId, Amount, CreatedAt) VALUES(@blockId, @accountId, @amount, @createdAt)",
                        new
                        {
                            blockId = payment.BlockId,
                            accountId = payment.AccountId,
                            amount = payment.Amount,
                            createdAt = DateTime.Now
                        });
                }
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while committing payment; {0:l}", e.Message);
            }
        }

        public void UpdatePayment(IPayment payment)
        {
            try
            {
                if (!IsEnabled)
                    return;

                using (var connection = new MySqlConnection(_mySqlProvider.ConnectionString))
                {
                    connection.Execute(
                        @"UPDATE Payment SET Completed = @completed WHERE Id = @id",
                        new
                        {
                            completed = payment.Completed,
                            id = payment.Id
                        });
                }
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while updating payment; {0:l}", e.Message);
            }
        }

        public IList<IPayment> GetPendingPayments()
        {
            var payouts = new List<IPayment>();

            try
            {
                if (!IsEnabled)
                    return payouts;

                using (var connection = new MySqlConnection(_mySqlProvider.ConnectionString))
                {
                    var results = connection.Query<Payment>(
                        @"SELECT Id, Block, AccountId, Amount, Completed FROM Payment Where Completed = false ORDER BY Id ASC");

                    payouts.AddRange(results);
                }
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while getting pending payments: {0:l}", e.Message);
            }

            return payouts;
        }

        public IList<IDetailedPayment> GetPaymentsForBlock(uint height)
        {
            var payouts = new List<IDetailedPayment>();

            try
            {
                if (!IsEnabled)
                    return payouts;

                using (var connection = new MySqlConnection(_mySqlProvider.ConnectionString))
                {
                    var results = connection.Query<DetailedPayment>(
                        @"SELECT p.Id as PaymentId, t.Id as TransactionId, p.AccountId, p.Block, p.Amount as Amount, 
                            t.Amount as SentAmount, t.Currency, t.TxId as TxHash, p.CreatedAt as PaymentDate, t.CreatedAt as TransactionDate, p.Completed 
                            FROM Payment p LEFT OUTER JOIN Transaction t On p.Id = t.PaymentId Where Block = @height",
                        new {height});

                    payouts.AddRange(results);
                }
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while getting payments: {0:l}", e.Message);
            }

            return payouts;
        }

        public void AddTransaction(ITransaction transaction)
        {
            try
            {
                if (!IsEnabled)
                    return;

                using (var connection = new MySqlConnection(_mySqlProvider.ConnectionString))
                {
                    connection.Execute(
                        @"INSERT INTO Transaction(AccountId, PaymentId, Amount, Currency, TxId, CreatedAt) 
                            VALUES(@accountId, @paymentId, @amount, @currency, @txId, @createdAt)",
                        new
                        {
                            accountId = transaction.Account.Id,
                            paymentId = transaction.Payment.Id,
                            amount = transaction.Payment.Amount,
                            currency = transaction.Currency,
                            txId = transaction.TxId,
                            createdAt = transaction.CreatedAt
                        });
                }
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while committing transaction; {0:l}", e.Message);
            }
        }
    }
}
