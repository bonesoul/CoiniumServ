namespace coinium.Net.RPC.Responses
{
    public class ListReceivedByAddressTransaction
    {
        public string Address { get; set; }
        public string Account { get; set; }
        public double Amount { get; set; }
        public int Confirmations { get; set; }
    }
}
