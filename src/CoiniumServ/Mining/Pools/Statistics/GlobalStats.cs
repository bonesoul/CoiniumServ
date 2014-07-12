using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coinium.Mining.Pools.Statistics
{
    public class GlobalStats : IGlobalStats, IStatisticsProvider
    {
        public UInt64 Hashrate { get; private set; }
        public UInt32 WorkerCount { get; private set; }

        public GlobalStats()
        {
            
        }

        public void Recache()
        {
            throw new NotImplementedException();
        }
    }
}
