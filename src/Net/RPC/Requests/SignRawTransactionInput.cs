using Newtonsoft.Json;

namespace coinium.Net.RPC.Requests
{
    public class SignRawTransactionInput
    {
        [JsonProperty("txid")]
        public string TransactionId { get; set; }

        [JsonProperty("vout")]
        public int Output { get; set; }

        [JsonProperty("scriptPubKey")]
        public string ScriptPubKey { get; set; }
    }
}
