using System;
using CoiniumServ.Server.Web.Service;
using Newtonsoft.Json;

namespace CoiniumServ.Statistics.New
{
    [JsonObject(MemberSerialization.OptIn)]
    public interface IStatisticsManager:IJsonService
    {
        [JsonProperty("hashrate")]
        UInt64 Hashrate { get; }

        [JsonProperty("miners")]
        Int32 MinerCount { get; }

        [JsonProperty("lastUpdate")]
        DateTime LastUpdate { get; }
    }
}
