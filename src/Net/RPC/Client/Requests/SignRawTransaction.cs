using System.Collections.Generic;

namespace coinium.Net.RPC.Client.Requests
{
    public class SignRawTransaction
    {
        /// <summary>
        /// Hexadecimal encoded version of the raw transaction to sign.
        /// </summary>
        public string RawTransactionHex { get; set; }

        /// <summary>
        /// A list of explicitly specified inputs to sign. This can be used
        /// if you do not want to sign all inputs in this transaction just yet.
        /// </summary>
        public List<SignRawTransactionInput> Inputs { get; set; }

        /// <summary>
        /// A list with the private keys needed to sign the transaction.
        /// There keys only have to be included if they are not in the wallet.
        /// </summary>
        public List<string> PrivateKeys { get; set; }

        public SignRawTransaction(string rawTransactionHex)
        {
            RawTransactionHex = rawTransactionHex;
            Inputs = new List<SignRawTransactionInput>();
            PrivateKeys = new List<string>();
        }

        public void AddInput(string transactionId, int output, string scriptPubKey)
        {
            Inputs.Add(new SignRawTransactionInput { TransactionId = transactionId, Output = output, ScriptPubKey = scriptPubKey });
        }

        public void AddKey(string privateKey)
        {
            PrivateKeys.Add(privateKey);
        }
    }
}
