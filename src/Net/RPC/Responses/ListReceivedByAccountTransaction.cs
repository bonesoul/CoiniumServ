namespace coinium.Net.RPC.Responses
{
    public class ListReceivedByAccountTransaction
    {
        public string Account { get; set; }
        public double Amount { get; set; }
        public int Confirmations { get; set; }
    }
}
