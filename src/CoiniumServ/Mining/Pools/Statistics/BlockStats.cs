using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Coinium.Persistance;
using Coinium.Persistance.Blocks;

namespace Coinium.Mining.Pools.Statistics
{
    public class BlockStats:IBlockStats
    {
        private IEnumerable<IPersistedBlock> _blocks;

        private readonly IStorage _storage;

        public BlockStats(IStorage storage)
        {
            _storage = storage;
        }

        public void Recache()
        {
            _blocks = _storage.GetAllBlocks().OrderByDescending(x => x.Key).Take(20).Select(item => item.Value).ToList();
        }

        public IEnumerator<IPersistedBlock> GetEnumerator()
        {
            return _blocks.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
