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
using CoiniumServ.Miners;
using CoiniumServ.Payments;
using CoiniumServ.Persistance.Blocks;
using CoiniumServ.Shares;

namespace CoiniumServ.Persistance.Layers.Empty
{
    public class EmptyStorageLayer : IStorageLayer
    {
        public bool SupportsShareStorage { get { return false; } }
        public bool SupportsBlockStorage { get { return false; } }
        public bool SupportsPaymentsStorage { get { return false; } }

        public void AddShare(IShare share)
        {
            throw new NotImplementedException();
        }

        public void RemoveShares(IPaymentRound round)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, double> GetCurrentShares()
        {
            throw new NotImplementedException();
        }

        public Dictionary<uint, Dictionary<string, double>> GetShares(IList<IPaymentRound> rounds)
        {
            throw new NotImplementedException();
        }

        public void AddBlock(IShare share)
        {
            throw new NotImplementedException();
        }

        public void UpdateBlock(IPaymentRound round)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, int> GetTotalBlocks()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPersistedBlock> GetBlocks(BlockStatus status)
        {
            throw new NotImplementedException();
        }

        public bool Authenticate(IMiner miner)
        {
            // empty storage layer is only used when no valid storage-layer configuration is available.
            // just authenticate all requests as basically we can't validate nor pay miners actually.
            return true;
        }

        public void UpdateDifficulty(IMiner miner)
        {
            // as we don't have an actual persistance layer, we can't write difficulty update information.
            return;
        }
    }
}
