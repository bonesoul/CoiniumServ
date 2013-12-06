namespace coinium.Net.RPC.Responses
{
    public class UnspentTransaction
    {
        public string TxId { get; set; }
        public int VOut { get; set; }
        public string Address { get; set; }
        public string ScriptPubKey { get; set; }
        public double Amount { get; set; }
        public int Confirmations { get; set; }
    }
}
