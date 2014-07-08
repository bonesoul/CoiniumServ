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

namespace Coinium.Persistance
{
    public class PersistedBlock : IPersistedBlock
    {
        public uint Height { get; private set; }

        public List<IPersistedBlockHashes> Hashes { get; private set; }

        public IPersistedBlockHashes OutstandingHashes
        {
            get
            {
                switch (Status)
                {
                    case PersistedBlockStatus.Pending:
                        return Hashes.FirstOrDefault(hash => hash.Status == PersistedBlockStatus.Pending);
                    case PersistedBlockStatus.Kicked:
                        return Hashes.FirstOrDefault(hash => hash.Status == PersistedBlockStatus.Kicked);
                    case PersistedBlockStatus.Orphan:
                        return Hashes.FirstOrDefault(hash => hash.Status == PersistedBlockStatus.Orphan);
                    case PersistedBlockStatus.Confirmed:
                        return Hashes.FirstOrDefault(hash => hash.Status == PersistedBlockStatus.Confirmed);                    
                }

                return null;
            }
        }

        public PersistedBlockStatus Status
        {
            get
            {
                if (Hashes.Any(hashes => hashes.Status == PersistedBlockStatus.Confirmed)) // if any hash is confirmed
                    return PersistedBlockStatus.Confirmed; // block is confirmed
                else if (Hashes.Any(hashes => hashes.Status == PersistedBlockStatus.Orphan)) // if any hash is orphaned
                    return PersistedBlockStatus.Orphan; // block is orphaned
                else if (Hashes.Any(hashes => hashes.Status == PersistedBlockStatus.Kicked)) // if any hash is kicked
                    return PersistedBlockStatus.Kicked; // block is kicked
                else
                    return PersistedBlockStatus.Pending; // block is pending
            }
        }        

        public PersistedBlock(uint height)
        {
            Height = height;
            Hashes = new List<IPersistedBlockHashes>();
        }

        public void AddHashes(IPersistedBlockHashes hash)
        {
            Hashes.Add(hash);
        }

        public override string ToString()
        {
            return string.Format("Height: {0}, Status: {1}, Outstanding: {2}", Height, Status,OutstandingHashes);
        }
    }
}
