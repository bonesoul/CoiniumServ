using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coinium.Mining.Pools.Statistics
{
    public interface IGlobalStats: IStatisticsProvider
    {
        UInt64 Hashrate { get; }

        Int32 WorkerCount { get; }
    }
}
