using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coinium.Persistance.Blocks;

namespace Coinium.Mining.Pools.Statistics
{
    public interface IBlockStats : IStatisticsProvider, IEnumerable<IPersistedBlock>
    {
    }
}
