using System.Collections;
using System.Collections.Generic;

namespace Coinium.Mining.Pools.Statistics
{
    public class PoolStats:IPoolStats
    {
        private readonly Dictionary<string, IPerPoolStats> _pools;

        public PoolStats(IPoolManager poolManager)
        {
            _pools = new Dictionary<string, IPerPoolStats>();

            foreach (var pool in poolManager.GetPools())
            {
                _pools.Add(pool.Config.Coin.Name, pool.Stats);
                pool.Stats.Recache();
            }
        }

        public IEnumerator<KeyValuePair<string, IPerPoolStats>> GetEnumerator()
        {
            return _pools.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
