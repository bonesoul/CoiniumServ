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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CoiniumServ.Persistance;
using CoiniumServ.Persistance.Blocks;

namespace CoiniumServ.Statistics
{
    public class LatestBlocks:ILatestBlocks
    {
        private IEnumerable<IPersistedBlock> _blocks;

        private readonly IStorage _storage;

        public LatestBlocks(IStorage storage)
        {
            _storage = storage;
        }

        public IEnumerator<IPersistedBlock> GetEnumerator()
        {
            return _blocks.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Recache(object state)
        {
            // read latest blocks            
            _blocks = _storage.GetAllBlocks().OrderByDescending(x => x.Key).Take(20).Select(item => item.Value).ToList();
        }
    }
}
