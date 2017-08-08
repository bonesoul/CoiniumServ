using System.Collections.Generic;
//using CoiniumServ.Daemon.Responses;


namespace CoiniumServ.Overpool
{
	public interface IOverpoolClient
	{
        /*
		BlockTemplate GetBlockTemplate(bool modeRequired = false);

		BlockTemplate GetBlockTemplate(string blockHex);

		string SubmitBlock(string blockHex);

		Block GetBlock(string hash);

		Getwork Getwork();

		bool Getwork(string data);

		Info GetInfo();

		MiningInfo GetMiningInfo();

		ValidateAddress ValidateAddress(string walletAddress);

		Transaction GetTransaction(string txId);
*/
        void Subscribe();

        decimal GetBalance(string account = "");

		string MakeRawRequest(string method, params object[] parameters);

		Dictionary<string, decimal> ListAccounts();

		string GetAccount(string bitcoinAddress);

		string SendMany(string fromAccount, Dictionary<string, decimal> toBitcoinAddress, int minConf = 1, string comment = "");
	}
}