using System.Collections.Generic;

namespace coinium.Net.RPC.Client.Responses
{
    public class TransactionsSinceBlock
    {
        public List<TransactionSinceBlock> transactions { get; set; }
        public string Lastblock { get; set; }
    }
}
