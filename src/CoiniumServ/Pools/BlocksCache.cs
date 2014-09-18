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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using CoiniumServ.Persistance.Blocks;
using CoiniumServ.Persistance.Layers;
using Newtonsoft.Json;

namespace CoiniumServ.Pools
{
    public class BlocksCache : IBlocksCache
    {
        public int Pending { get; private set; }
        public int Confirmed { get; private set; }
        public int Orphaned { get; private set; }
        public int Total { get; private set; }

        public IList<IPersistedBlock> Latest
        {
            get { return _latestBlocks.AsReadOnly(); }
        }

        public IList<IPersistedBlock> LatestPaid
        {
            get { return _lastPaid.AsReadOnly(); }
        }

        private readonly List<IPersistedBlock> _latestBlocks;

        private readonly List<IPersistedBlock> _lastPaid;

        private readonly IStorageLayer _storageLayer;

        public BlocksCache(IStorageLayer storageLayer)
        {
            _storageLayer = storageLayer;
            _latestBlocks = new List<IPersistedBlock>();
            _lastPaid = new List<IPersistedBlock>();
        }


        public string ServiceResponse { get; private set; }

        public void Recache()
        {
            _latestBlocks.Clear();
            _lastPaid.Clear();

            _latestBlocks.AddRange(_storageLayer.GetLastestBlocks()); // recache latest blocks.
            _lastPaid.AddRange(_storageLayer.GetLatestPaidBlocks()); // recache last paid blocks.

            // recache block counts.
            var blockCounts = _storageLayer.GetTotalBlocks();
            Pending = blockCounts.ContainsKey("pending") ? blockCounts["pending"] : 0;
            Confirmed = blockCounts.ContainsKey("confirmed") ? blockCounts["confirmed"] : 0;
            Orphaned = blockCounts.ContainsKey("orphaned") ? blockCounts["orphaned"] : 0;
            Total = Pending + Confirmed + Orphaned;

            // cache the json-service response
            ServiceResponse = JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        }
    }
}
