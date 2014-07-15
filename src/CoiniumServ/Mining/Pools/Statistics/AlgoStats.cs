using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coinium.Mining.Pools.Statistics
{
    public class AlgoStats : IAlgoStats
    {
        private readonly Dictionary<string, IPerAlgorithmStats> _algorithms;
        private readonly IPoolStats _poolStatistics;

        public AlgoStats(IPoolStats poolStatistics)
        {
            _poolStatistics = poolStatistics;
            _algorithms = new Dictionary<string, IPerAlgorithmStats>();
        }

        public void Recache(object state)
        {
            foreach (var pair in _algorithms)
            {
                pair.Value.Reset();
            }

            foreach (var pair in _poolStatistics)
            {
                if (!_algorithms.ContainsKey(pair.Value.Algorithm))
                    _algorithms.Add(pair.Value.Algorithm, new PerAlgorithmStats(pair.Value.Algorithm));

                _algorithms[pair.Value.Algorithm].Hashrate = pair.Value.Hashrate;
                _algorithms[pair.Value.Algorithm].WorkerCount = pair.Value.WorkerCount;
            }
        }

        public IEnumerator<KeyValuePair<string, IPerAlgorithmStats>> GetEnumerator()
        {
            return _algorithms.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
