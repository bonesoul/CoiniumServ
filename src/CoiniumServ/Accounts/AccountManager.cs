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
using CoiniumServ.Payments;
using CoiniumServ.Persistance.Layers;
using CoiniumServ.Persistance.Layers.Hybrid;
using CoiniumServ.Persistance.Query;
using CoiniumServ.Pools;
using Serilog;

namespace CoiniumServ.Accounts
{
    public class AccountManager:IAccountManager
    {
        private readonly IStorageLayer _storageLayer;

        private readonly ILogger _logger;

        public AccountManager(IStorageLayer storageLayer, IPoolConfig poolConfig)
        {
            _storageLayer = storageLayer;
            _logger = Log.ForContext<HybridStorage>().ForContext("Component", poolConfig.Coin.Name);
        }

        public void AddAccount(IAccount account)
        {
            _storageLayer.AddAccount(account);
        }

        public IAccount GetAccountById(int id)
        {
            return _storageLayer.GetAccountById(id);
        }

        public IAccount GetAccountByUsername(string username)
        {
            return _storageLayer.GetAccountByUsername(username);
        }

        public IAccount GetAccountByAddress(string address)
        {
            return _storageLayer.GetAccountByAddress(address);
        }

        public IList<IPaymentDetails> GetPaymentsForAccount(int id, IPaginationQuery paginationQuery)
        {
            return _storageLayer.GetPaymentsForAccount(id, paginationQuery);
        }
    }
}
