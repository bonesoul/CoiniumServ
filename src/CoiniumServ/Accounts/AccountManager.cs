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
