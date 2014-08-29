using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using CoiniumServ.Persistance;
using CoiniumServ.Persistance.Blocks;
using Newtonsoft.Json;

namespace CoiniumServ.Pools
{
    public class BlocksCache : IBlocksCache
    {
        public int Pending { get; private set; }
        public int Confirmed { get; private set; }
        public int Orphaned { get; private set; }
        public int Total { get; private set; }

        public Dictionary<uint, IPersistedBlock> Latest { 
            get { return _storage.ToDictionary(b => b.Height); }
        } 

        private readonly IStorageOld _storageOld;

        private readonly IList<IPersistedBlock> _storage;

        public BlocksCache(IStorageOld storageOld)
        {
            _storageOld = storageOld;
            _storage = new List<IPersistedBlock>();
        }

        public IEnumerator<IPersistedBlock> GetEnumerator()
        {
            return _storage.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IQueryable<IPersistedBlock> SearchFor(Expression<Func<IPersistedBlock, bool>> predicate)
        {
            return _storage.AsQueryable().Where(predicate);
        }

        public IEnumerable<IPersistedBlock> GetAll()
        {
            return _storage;
        }

        public IQueryable<IPersistedBlock> GetAllAsQueryable()
        {
            return _storage.AsQueryable();
        }

        public IReadOnlyCollection<IPersistedBlock> GetAllAsReadOnly()
        {
            return new ReadOnlyCollection<IPersistedBlock>(_storage);
        }

        public int Count { get { return _storage.Count; } }

        public string ServiceResponse { get; private set; }

        public void Recache()
        {
            _storage.Clear();
            
            // recache blocks.
            var blocks = _storageOld.GetAllBlocks().OrderByDescending(x => x.Key).Take(20).Select(item => item.Value).ToList();
            foreach (var block in blocks)
            {
                _storage.Add(block);
            }

            // recache block counts.
            var blockCounts = _storageOld.GetBlockCounts();
            Pending = blockCounts.ContainsKey("pending") ? blockCounts["pending"] : 0;
            Confirmed = blockCounts.ContainsKey("confirmed") ? blockCounts["confirmed"] : 0;
            Orphaned = blockCounts.ContainsKey("orphaned") ? blockCounts["orphaned"] : 0;

            // cache the json-service response
            ServiceResponse = JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        }
    }
}
