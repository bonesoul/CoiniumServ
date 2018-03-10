
using CoiniumServ.Daemon.Converters;
using Newtonsoft.Json;

namespace CoiniumServ.Daemon.Responses
{
    public class PreInfo
    {
        public string Version { get; set; }

        public int ProtocolVersion { get; set; }

        public int WalletVersion { get; set; }

        public decimal Balance { get; set; }

        public long Blocks { get; set; }

        public long TimeOffset { get; set; }

        public long Connections { get; set; }

        public string Proxy { get; set; }

        [JsonConverter(typeof(DifficultyConverter))]
        public double Difficulty { get; set; }

        public bool Testnet { get; set; }

        public long KeyPoolEldest { get; set; }

        public long KeyPoolSize { get; set; }

        public decimal PayTxFee { get; set; }

        public string Errors { get; set; }
    }
}
