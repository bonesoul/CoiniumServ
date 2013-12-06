using Newtonsoft.Json;

namespace coinium.Net.RPC.Requests
{
    public class CreateRawTransactionInput
    {
        [JsonProperty("txid")]
        public string TransactionId { get; set; }

        [JsonProperty("vout")]
        public int Output { get; set; }
    }
}
