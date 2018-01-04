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

using System.Collections.Generic;
using CoiniumServ.Persistance.Blocks;
using CoiniumServ.Persistance.Layers;
using CoiniumServ.Persistance.Query;
using Newtonsoft.Json;

namespace CoiniumServ.Pools
{
    public class BlocksRepository : IBlockRepository
    {
        public int Pending { get; private set; }
        public int Confirmed { get; private set; }
        public int Orphaned { get; private set; }
        public int Total { get; private set; }

        private readonly IPaginationQuery _query = new PaginationQuery(1, 5);

        public IList<IPersistedBlock> Latest
        {
            get { return _latestBlocks.AsReadOnly(); }
        }

        public IList<IPersistedBlock> LatestPaid
        {
            get { return _lastPaid.AsReadOnly(); }
        }

        public IPersistedBlock Get(uint height)
        {
            return _storageLayer.GetBlock(height);
        }

        public IList<IPersistedBlock> GetBlocks(IPaginationQuery paginationQuery)
        {
            return _storageLayer.GetBlocks(paginationQuery);
        }

        public IList<IPersistedBlock> GetPaidBlocks(IPaginationQuery paginationQuery)
        {
            return _storageLayer.GetPaidBlocks(paginationQuery);
        }

        private readonly List<IPersistedBlock> _latestBlocks;

        private readonly List<IPersistedBlock> _lastPaid;

        private readonly IStorageLayer _storageLayer;

        public string ServiceResponse { get; private set; }

        public BlocksRepository(IStorageLayer storageLayer)
        {
            _storageLayer = storageLayer;
            _latestBlocks = new List<IPersistedBlock>();
            _lastPaid = new List<IPersistedBlock>();
        }

        public void Recache()
        {
            _latestBlocks.Clear();
            _lastPaid.Clear();

            _latestBlocks.AddRange(_storageLayer.GetBlocks(_query)); // recache latest blocks.
            _lastPaid.AddRange(_storageLayer.GetPaidBlocks(_query)); // recache last paid blocks.

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
