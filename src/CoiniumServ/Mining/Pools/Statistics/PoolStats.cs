using System.Collections;
using System.Collections.Generic;

namespace Coinium.Mining.Pools.Statistics
{
    public class PoolStats:IPoolStats
    {
        private readonly Dictionary<string, IPerPoolStats> _pools;
        private readonly IPoolManager _poolManager;

        public PoolStats(IPoolManager poolManager)
        {
            _poolManager = poolManager;
            _pools = new Dictionary<string, IPerPoolStats>();


            foreach (var pool in poolManager.GetPools())
            {
                _pools.Add(pool.Config.Coin.Name, pool.Stats);
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

        public void Recache(object state)
        {
            foreach (var pool in _poolManager.GetPools())
            {
                pool.Stats.Recache(state);
            }
        }
    }
}
