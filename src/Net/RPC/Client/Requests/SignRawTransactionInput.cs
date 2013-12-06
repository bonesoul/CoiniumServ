/*
 * Coinium project, Copyright (C) 2013, Int6 Studios - All Rights Reserved. - http://www.coinium.org
 *
 */

/* This file is based on https://github.com/BitKoot/BitcoinRpcSharp */

using Newtonsoft.Json;

namespace coinium.Net.RPC.Client.Requests
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
