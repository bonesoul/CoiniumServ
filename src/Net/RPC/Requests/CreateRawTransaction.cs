using System.Collections.Generic;

namespace coinium.Net.RPC.Requests
{
    public class CreateRawTransaction
    {
        public List<CreateRawTransactionInput> Inputs { get; set; }

        /// <summary>
        /// A dictionary with the output address and amount per addres.
        /// </summary>
        public Dictionary<string, decimal> Outputs { get; set; }

        public CreateRawTransaction()
        {
            Inputs = new List<CreateRawTransactionInput>();
            Outputs = new Dictionary<string, decimal>();
        }

        public void AddInput(string transactionId, int output)
        {
            Inputs.Add(new CreateRawTransactionInput { TransactionId = transactionId, Output = output });
        }

        public void AddOutput(string address, decimal amount)
        {
            Outputs.Add(address, amount);
        }
    }
}
