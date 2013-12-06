using System.Collections.Generic;

namespace coinium.Net.RPC.Responses
{
    public class Transaction
    {
        public double Amount { get; set; }
        public double Fee { get; set; }
        public int Confirmations { get; set; }
        public string TxId { get; set; }
        public int Time { get; set; }
        public int TimeReceived { get; set; }
        public string Comment { get; set; }
        public string To { get; set; }
        public List<TransactionDetail> Details { get; set; }
    }
}
