using Newtonsoft.Json;

namespace coinium.Net.RPC.Client.Requests
{
    public class CreateRawTransactionInput
    {
        [JsonProperty("txid")]
        public string TransactionId { get; set; }

        [JsonProperty("vout")]
        public int Output { get; set; }
    }
}
