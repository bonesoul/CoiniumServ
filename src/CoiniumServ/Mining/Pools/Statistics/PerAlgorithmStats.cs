using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coinium.Mining.Pools.Statistics
{
    public class PerAlgorithmStats:IPerAlgorithmStats
    {
        public string Name { get; private set; }
        public int WorkerCount { get; set; }
        public ulong Hashrate { get; set; }

        public PerAlgorithmStats(string algorithm)
        {
            Name = algorithm;
        }

        public void Reset()
        {
            Hashrate = 0;
            WorkerCount = 0;
        }
    }
}
