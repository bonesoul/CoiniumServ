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
using System.Collections.Generic;
using System.Linq;
using CoiniumServ.Payments;
using CoiniumServ.Persistance.Query;
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

        public IList<IPaymentDetails> GetPaymentsForBlock(uint height)
        {
            var payouts = new List<IPaymentDetails>();

            try
            {
                if (!IsEnabled)
                    return payouts;

                using (var connection = new MySqlConnection(_mySqlProvider.ConnectionString))
                {
                    var results = connection.Query<PaymentDetails>(
                        @"SELECT p.Id as PaymentId, t.Id as TransactionId, p.AccountId, a.Address, p.Block, p.Amount as Amount, 
                            t.Amount as SentAmount, t.Currency, t.TxHash, p.CreatedAt as PaymentDate, t.CreatedAt as TransactionDate, p.Completed 
                            FROM Payment p 
                                INNER JOIN Account as a ON p.AccountId = a.Id
                                LEFT OUTER JOIN Transaction t On p.Id = t.PaymentId Where Block = @height",
                        new { height });

                    payouts.AddRange(results);
                }
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while getting payments: {0:l}", e.Message);
            }

            return payouts;
        }

        public IList<IPaymentDetails> GetPaymentsForAccount(int id, IPaginationQuery paginationQuery)
        {
            var payouts = new List<IPaymentDetails>();

            try
            {
                if (!IsEnabled)
                    return payouts;

                using (var connection = new MySqlConnection(_mySqlProvider.ConnectionString))
                {
                    var results = connection.Query<PaymentDetails>(
                        @"SELECT p.Id as PaymentId, t.Id as TransactionId, p.AccountId, a.Address, p.Block, p.Amount as Amount, 
                            t.Amount as SentAmount, t.Currency, t.TxHash, p.CreatedAt as PaymentDate, t.CreatedAt as TransactionDate, p.Completed 
                            FROM Payment p 
                                INNER JOIN Account as a ON p.AccountId = a.Id
                                LEFT OUTER JOIN Transaction t On p.Id = t.PaymentId Where a.Id = @id
                            ORDER BY p.Id DESC LIMIT @offset, @count",
                        new
                        {
                            id,
                            offset = paginationQuery.Offset,
                            count = paginationQuery.Count
                        });

                    payouts.AddRange(results);
                }
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while getting payments: {0:l}", e.Message);
            }

            return payouts;
        }

        public IPaymentDetails GetPaymentDetailsByTransactionId(uint id)
        {
            try
            {
                if (!IsEnabled)
                    return null;

                using (var connection = new MySqlConnection(_mySqlProvider.ConnectionString))
                {
                    return connection.Query<PaymentDetails>(
                        @"SELECT p.Id as PaymentId, t.Id as TransactionId, p.AccountId, a.Address, p.Block, p.Amount as Amount, 
                            t.Amount as SentAmount, t.Currency, t.TxHash, p.CreatedAt as PaymentDate, t.CreatedAt as TransactionDate, p.Completed 
                            FROM Payment p 
                                INNER JOIN Account as a ON p.AccountId = a.Id
                                LEFT OUTER JOIN Transaction t On p.Id = t.PaymentId 
                                Where t.Id = @id",
                        new {id}).Single();
                }
            }
            catch (InvalidOperationException) // fires when no result is found.
            {
                return null;
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while gettin transaction: {0:l}", e.Message);
                return null;
            }
        }

        public IPaymentDetails GetPaymentDetailsByPaymentId(uint id)
        {
            try
            {
                if (!IsEnabled)
                    return null;

                using (var connection = new MySqlConnection(_mySqlProvider.ConnectionString))
                {
                    return connection.Query<PaymentDetails>(
                        @"SELECT p.Id as PaymentId, t.Id as TransactionId, p.AccountId, a.Address, p.Block, p.Amount as Amount, 
                            t.Amount as SentAmount, t.Currency, t.TxHash, p.CreatedAt as PaymentDate, t.CreatedAt as TransactionDate, p.Completed 
                            FROM Payment p 
                                INNER JOIN Account as a ON p.AccountId = a.Id
                                LEFT OUTER JOIN Transaction t On p.Id = t.PaymentId 
                                Where p.Id = @id",
                        new { id }).Single();
                }
            }
            catch (InvalidOperationException) // fires when no result is found.
            {
                return null;
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while gettin transaction: {0:l}", e.Message);
                return null;
            }
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
                        @"INSERT INTO Transaction(AccountId, PaymentId, Amount, Currency, TxHash, CreatedAt) 
                            VALUES(@accountId, @paymentId, @amount, @currency, @txHash, @createdAt)",
                        new
                        {
                            accountId = transaction.Account.Id,
                            paymentId = transaction.Payment.Id,
                            amount = transaction.Payment.Amount,
                            currency = transaction.Currency,
                            txHash = transaction.TxHash,
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
