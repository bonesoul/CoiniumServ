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
using CoiniumServ.Persistance.Blocks;
using CoiniumServ.Shares;

namespace CoiniumServ.Persistance
{
    public interface IStorage
    {
        bool IsEnabled { get; }

        void AddShare(IShare share);

        void AddBlock(IShare share);

        void SetRemainingBalances(IList<IWorkerBalance> workerBalances);

        void DeleteShares(IPaymentRound round);

        void MoveSharesToCurrentRound(IPaymentRound round);

        void MoveBlock(IPaymentRound round);

        IDictionary<string, int> GetBlockCounts();

        void DeleteExpiredHashrateData(int until);

        IDictionary<string, double> GetHashrateData(int since);

        IList<IPendingBlock> GetPendingBlocks();

        IDictionary<UInt32, IPersistedBlock> GetAllBlocks();

        Dictionary<UInt32, Dictionary<string, double>> GetSharesForRounds(IList<IPaymentRound> rounds);

        Dictionary<string, double> GetPreviousBalances();
    }
}
