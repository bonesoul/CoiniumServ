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
using CoiniumServ.Persistance.Blocks;
using CoiniumServ.Shares;

namespace CoiniumServ.Persistance.Layers.Hybrid
{
    public class HybridStorage : IStorageLayer, IShareStorage, IBlockStorage, IPaymentStorage
    {
        public void AddShare(IShare share)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveShares(IPaymentRound round)
        {
            throw new System.NotImplementedException();
        }

        public Dictionary<string, double> GetCurrentShares()
        {
            throw new System.NotImplementedException();
        }

        public Dictionary<uint, Dictionary<string, double>> GetShares(IList<IPaymentRound> rounds)
        {
            throw new System.NotImplementedException();
        }

        public void AddBlock(IShare share)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateBlock(IPaymentRound round)
        {
            throw new System.NotImplementedException();
        }

        public IDictionary<string, int> GetTotalBlocks()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IPersistedBlock> GetBlocks(BlockStatus status)
        {
            throw new System.NotImplementedException();
        }
    }
}
