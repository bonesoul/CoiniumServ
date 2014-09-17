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
using CoiniumServ.Accounts;
using CoiniumServ.Miners;
using CoiniumServ.Payments;
using CoiniumServ.Persistance.Blocks;
using CoiniumServ.Server.Mining.Stratum;
using CoiniumServ.Shares;

namespace CoiniumServ.Persistance.Layers.Empty
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

        public IDictionary<string, int> GetTotalBlocks()
        {
            return new Dictionary<string, int>();
        }

        public IEnumerable<IPersistedBlock> GetUnpaidBlocks()
        {
            return new List<IPersistedBlock>();
        }

        public IEnumerable<IPersistedBlock> GetPendingBlocks()
        {
            return new List<IPersistedBlock>();
        }

        public IEnumerable<IPersistedBlock> GetLastBlocks(int count = 20)
        {
            return new List<IPersistedBlock>();
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

        public void AddTransaction(ITransaction transaction)
        {
            return;
        }

        public void AddAccount(IAccount user)
        {
            return;
        }

        public IAccount GetAccount(string username)
        {
            return null;
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
