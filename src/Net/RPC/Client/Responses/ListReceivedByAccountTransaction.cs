namespace coinium.Net.RPC.Client.Responses
{
    public class ListReceivedByAccountTransaction
    {
        public string Account { get; set; }
        public double Amount { get; set; }
        public int Confirmations { get; set; }
    }
}
