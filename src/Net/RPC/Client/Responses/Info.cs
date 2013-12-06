namespace coinium.Net.RPC.Client.Responses
{
    public class Info
    {
        public string Version { get; set; }
        public string ProtocolVersion { get; set; }
        public string WalletVersion { get; set; }
        public decimal Balance { get; set; }
        public long Blocks { get; set; }
        public long TimeOffset { get; set; }
        public long Connections { get; set; }
        public string Proxy { get; set; }
        public decimal Difficulty { get; set; }
        public bool Testnet { get; set; }
        public long KeyPoolEldest { get; set; }
        public long KeyPoolSize { get; set; }
        public decimal PayTxFee { get; set; }
        public string Errors { get; set; }
    }
}
