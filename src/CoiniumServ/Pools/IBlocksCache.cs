using System.Collections.Generic;
using CoiniumServ.Persistance.Blocks;
using CoiniumServ.Server.Web.Service;
using CoiniumServ.Statistics.Repository;
using Newtonsoft.Json;

namespace CoiniumServ.Pools
{
    [JsonObject(MemberSerialization.OptIn)]
    public interface IBlocksCache : IRepository<IPersistedBlock>, IJsonService
    {
        [JsonProperty("pending")]
        int Pending { get; }
        
        [JsonProperty("confirmed")]
        int Confirmed { get; }
        
        [JsonProperty("orphaned")]
        int Orphaned { get; }
        
        [JsonProperty("total")]
        int Total { get; }

        [JsonProperty("latest")]
        Dictionary<uint, IPersistedBlock> Latest { get; } 
    }
}
