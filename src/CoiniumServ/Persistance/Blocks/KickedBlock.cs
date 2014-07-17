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
namespace CoiniumServ.Persistance.Blocks
{
    public class KickedBlock:IKickedBlock
    {
        public uint Height { get; private set; }
        public BlockStatus Status { get; private set; }
        public string BlockHash { get; private set; }
        public string TransactionHash { get; private set; }
        public decimal Reward { get; set; }
        public decimal Amount { get; set; }

        public KickedBlock(uint height, string blockHash, string transactionHash, decimal amount, decimal reward)
        {
            Height = height;
            BlockHash = blockHash;
            TransactionHash = transactionHash;
            Amount = amount;
            Reward = reward;
            Status = BlockStatus.Kicked;
        }

        public KickedBlock(uint height, IHashCandidate candidate)
            : this(height, candidate.BlockHash, candidate.TransactionHash, candidate.Amount, candidate.Reward)
        { }

        public override string ToString()
        {
            return string.Format("Height: {0}, Status: Kicked.", Height);
        }
    }
}
