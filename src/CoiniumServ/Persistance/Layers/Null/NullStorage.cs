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

using System.Collections.Generic;
using CoiniumServ.Accounts;
using CoiniumServ.Mining;
using CoiniumServ.Payments;
using CoiniumServ.Persistance.Blocks;
using CoiniumServ.Persistance.Query;
using CoiniumServ.Server.Mining.Stratum;
using CoiniumServ.Shares;

namespace CoiniumServ.Persistance.Layers.Null
{
    public class NullStorage : IStorageLayer
    {
        public bool IsEnabled { get; private set; }

        public void AddShare(IShare share)
        {
            return;
        }

        public void MoveCurrentShares(int height)
        {
            return;
        }

        public void MoveOrphanedShares(IPersistedBlock block)
        {
            return;
        }

        public void RemoveShares(IPaymentRound round)
        {
            return;
        }

        public Dictionary<string, double> GetCurrentShares()
        {
            return new Dictionary<string, double>();
        }

        public Dictionary<string, double> GetShares(IPersistedBlock block)
        {
            return new Dictionary<string, double>();
        }

        public void AddBlock(IShare share)
        {
            return;
        }

        public void UpdateBlock(IPersistedBlock block)
        {
            return;
        }

        public IPersistedBlock GetBlock(uint height)
        {
            throw new System.NotImplementedException();
        }

        public IList<IPersistedBlock> GetBlocks(IPaginationQuery paginationQuery)
        {
            throw new System.NotImplementedException();
        }

        public IList<IPersistedBlock> GetPaidBlocks(IPaginationQuery paginationQuery)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IPersistedBlock> GetUnpaidBlocks()
        {
            return new List<IPersistedBlock>();
        }

        public IEnumerable<IPersistedBlock> GetPendingBlocks()
        {
            return new List<IPersistedBlock>();
        }

        public IDictionary<string, int> GetTotalBlocks()
        {
            return new Dictionary<string, int>();
        }

        public void AddPayment(IPayment payment)
        {
            return;
        }

        public void UpdatePayment(IPayment payment)
        {
            return;
        }

        public IList<IPayment> GetPendingPayments()
        {
            return new List<IPayment>();
        }

        public IList<IPaymentDetails> GetPaymentsForBlock(uint height)
        {
            return new List<IPaymentDetails>();
        }

        public IList<IPaymentDetails> GetPaymentsForAccount(int id, IPaginationQuery paginationQuery)
        {
            return new List<IPaymentDetails>();
        }

        public IPaymentDetails GetPaymentDetailsByTransactionId(uint id)
        {
            return null;
        }

        public IPaymentDetails GetPaymentDetailsByPaymentId(uint id)
        {
            return null;
        }

        public void AddTransaction(ITransaction transaction)
        {
            return;
        }

        public void AddAccount(IAccount account)
        {
            return;
        }

        public IAccount GetAccountByUsername(string username)
        {
            return null;
        }

        public IAccount GetAccountByAddress(string address)
        {
            throw new System.NotImplementedException();
        }

        public IAccount GetAccountById(int id)
        {
            return null;
        }

        public bool Authenticate(IMiner miner)
        {
            // empty storage layer is only used when no valid storage-layer configuration is available.
            // just authenticate all requests as basically we can't validate nor pay miners actually.
            return true;
        }

        public void UpdateDifficulty(IStratumMiner miner)
        {
            return;
        }

        public IDictionary<string, double> GetHashrateData(int since)
        {
            return new Dictionary<string, double>();
        }

        public void DeleteExpiredHashrateData(int until)
        {
            return;
        }
    }
}
