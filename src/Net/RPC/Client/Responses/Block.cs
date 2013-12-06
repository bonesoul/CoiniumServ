using System.Collections.Generic;

namespace coinium.Net.RPC.Client.Responses
{
    public class Block
    {
        public string Hash { get; set; }
        public int Confirmations { get; set; }
        public int Size { get; set; }
        public int Height { get; set; }
        public int Version { get; set; }
        public string MerkleRoot { get; set; }
        public List<string> Tx { get; set; }
        public int Time { get; set; }
        public int Nonce { get; set; }
        public string Bits { get; set; }
        public double Difficulty { get; set; }
        public string NextBlockHash { get; set; }
    }
}
