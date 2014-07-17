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
using System.Linq;

namespace CoiniumServ.Persistance.Blocks
{
    public class PendingBlock : IPendingBlock
    {
        public uint Height { get; private set; }

        public bool IsFinalized
        {
            get { return Finalized != null; }
        }

        public BlockStatus Status
        {
            get { return IsFinalized ? Finalized.Status : BlockStatus.Pending; }
        }

        public string BlockHash
        {
            get { return IsFinalized ? Finalized.BlockHash : "Many.."; }
        }

        public string TransactionHash
        {
            get { return IsFinalized ? Finalized.BlockHash : "Many.."; }
        }

        public decimal Reward
        {
            get { return IsFinalized ? Finalized.Reward : 0; }
        }

        public decimal Amount
        {
            get { return IsFinalized ? Finalized.Amount : 0; }
        }

        public IFinalizedBlock Finalized { get; private set; }
        public List<IHashCandidate> Candidates { get; private set; }

        public PendingBlock(uint height)
        {
            Height = height;
            Candidates = new List<IHashCandidate>();
            Finalized = null;
        }

        public void AddHashCandidate(IHashCandidate hash)
        {
            Candidates.Add(hash);
        }

        public void Check()
        {
            var confirmedCandidate = Candidates.FirstOrDefault( x => x.Status == BlockStatus.Confirmed);

            if (confirmedCandidate != null)
            {
                Finalized = new ConfirmedBlock(Height, confirmedCandidate);
                return;
            }

            var orphanedCandidate = Candidates.FirstOrDefault(x => x.Status == BlockStatus.Orphaned);
            if (orphanedCandidate != null)
            {
                Finalized = new OrphanedBlock(Height, orphanedCandidate);
                return;
            }

            var kickedCandidate = Candidates.FirstOrDefault(x => x.Status == BlockStatus.Kicked);
            if (kickedCandidate != null)
            {
                Finalized = new KickedBlock(Height, kickedCandidate);
                return;
            }
        }

        public override string ToString()
        {
            return string.Format("Height: {0}, Status: {1}.", Height, Finalized == null ? BlockStatus.Pending : Finalized.Status);
        }
    }
}
