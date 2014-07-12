using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coinium.Mining.Pools.Statistics
{
    public class AlgoStats:IAlgoStats
    {
        private readonly Dictionary<string, IPerAlgorithmStats> _algorithms;

        public AlgoStats()
        {
            _algorithms = new Dictionary<string, IPerAlgorithmStats>();

            _algorithms.Add("test", new PerAlgorithmStats());
            _algorithms.Add("test2", new PerAlgorithmStats());
            _algorithms.Add("test3", new PerAlgorithmStats());
            _algorithms.Add("test4", new PerAlgorithmStats());
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
