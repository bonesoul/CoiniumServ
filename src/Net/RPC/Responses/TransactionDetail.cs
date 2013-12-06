namespace coinium.Net.RPC.Responses
{
    public class TransactionDetail
    {
        public string Account { get; set; }
        public string Address { get; set; }
        public string Category { get; set; }
        public double Amount { get; set; }
        public double Fee { get; set; }
    }
}
