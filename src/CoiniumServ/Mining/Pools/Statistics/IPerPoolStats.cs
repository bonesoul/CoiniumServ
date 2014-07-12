using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coinium.Mining.Pools.Statistics
{
    public interface IPerPoolStats: IStatisticsProvider
    {
        UInt64 Hashrate { get; }

        UInt64 NetworkHashrate { get; }

        double Difficulty { get; }

        int CurrentBlock { get; }

        IBlockStats LatestBlocks { get; }
    }
}
