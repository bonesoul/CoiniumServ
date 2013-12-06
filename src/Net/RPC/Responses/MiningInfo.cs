namespace coinium.Net.RPC.Responses
{
    public class MiningInfo
    {
        public int Blocks { get; set; }
        public int CurrentBockSize { get; set; }
        public int CurrentBlockTx { get; set; }
        public double Difficulty { get; set; }
        public string Errors { get; set; }
        public bool Generate { get; set; }
        public int GenProcLimit { get; set; }
        public int HashesPerSec { get; set; }
        public int PooledTx { get; set; }
        public bool Testnet { get; set; }
    }
}
