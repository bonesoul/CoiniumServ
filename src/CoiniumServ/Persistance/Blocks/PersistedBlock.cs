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

using System.Diagnostics;

namespace CoiniumServ.Persistance.Blocks
{
    [DebuggerDisplay("Height: {Height}, Status: {Status}")]
    public class PersistedBlock:IPersistedBlock
    {
        public uint Height { get; private set; }

        public BlockStatus Status { get; set; }

        public string BlockHash { get; private set; }

        public string TransactionHash { get; private set; }

        public decimal Reward { get; private set; }

        public decimal Amount { get; private set; }

        public bool IsPending { get { return Status != BlockStatus.Orphaned && Status != BlockStatus.Confirmed; } }

        public PersistedBlock(BlockStatus status, uint height, string blockHash, string transactionHash, decimal amount, decimal reward)
        {
            Status = status;
            Height = height;
            BlockHash = blockHash;
            TransactionHash = transactionHash;
            Amount = amount;
            Reward = reward;
        }
    }
}
