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
using System.Diagnostics;
using CoiniumServ.Utils.Helpers;

namespace CoiniumServ.Persistance.Blocks
{
    [DebuggerDisplay("Height: {Height}, Status: {Status}")]
    public class PersistedBlock:IPersistedBlock
    {
        public uint Height { get; private set; }

        public BlockStatus Status { get; set; }

        public bool Accounted { get; set; }

        public string BlockHash { get; private set; }

        public string TransactionHash { get; private set; }

        public decimal Amount { get; private set; }

        public decimal Reward { get; set; }

        public DateTime CreatedAt { get; private set; }

        public bool IsPending { get { return Status != BlockStatus.Orphaned && Status != BlockStatus.Confirmed; } }

        public PersistedBlock(Int32 height, Boolean orphaned, Boolean confirmed, Boolean accounted, String blockHash, String txHash, Decimal amount, Decimal reward, DateTime createdAt)
        {
            // determine the block status
            if(orphaned)
                Status = BlockStatus.Orphaned;
            else if(confirmed)
                Status = BlockStatus.Confirmed;
            else
                Status = BlockStatus.Pending;

            Height = (uint)height;
            Accounted = accounted;
            BlockHash = blockHash;
            TransactionHash = txHash;
            Amount = amount;
            Reward = reward;
            CreatedAt = createdAt;
        }

        public PersistedBlock(UInt32 height, String blockhash, Double amount, Int32 confirmations, Int32 time)
        {
            // determine the block status
            if (confirmations == -1)
                Status = BlockStatus.Orphaned;
            else
                Status = confirmations > 120 ? BlockStatus.Confirmed : BlockStatus.Pending;
            
            Height = height;
            BlockHash = blockhash;
            Amount = (decimal)amount;
            CreatedAt = time.UnixTimestampToDateTime();
        }
    }
}
