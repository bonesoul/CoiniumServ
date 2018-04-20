#region License
// 
//     MIT License
//
//     CoiniumServ - Crypto Currency Mining Pool Server Software
//     Copyright (C) 2013 - 2017, CoiniumServ Project
//     Hüseyin Uslu, shalafiraistlin at gmail dot com
//     https://github.com/bonesoul/CoiniumServ
// 
//     Permission is hereby granted, free of charge, to any person obtaining a copy
//     of this software and associated documentation files (the "Software"), to deal
//     in the Software without restriction, including without limitation the rights
//     to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//     copies of the Software, and to permit persons to whom the Software is
//     furnished to do so, subject to the following conditions:
//     
//     The above copyright notice and this permission notice shall be included in all
//     copies or substantial portions of the Software.
//     
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//     FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//     LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//     OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//     SOFTWARE.
// 
#endregion

using System.Collections.Generic;
using CoiniumServ.Daemon.Responses;

namespace CoiniumServ.Daemon
{
    public interface IDaemonClient
    {
        BlockTemplate GetBlockTemplate(bool modeRequired = false);

        BlockTemplate GetBlockTemplate(string blockHex);

        string SubmitBlock(string blockHex);

        Block GetBlock(string hash);

        Getwork Getwork();

        bool Getwork(string data);

        Info GetInfo();

        Info GetBlockChainInfo();

        Info GetNetworkInfo();

        Info GetWalletInfo();

        MiningInfo GetMiningInfo();

        ValidateAddress ValidateAddress(string walletAddress);

        GetAddressInfo GetAddressInfo(string walletAddress);

        Transaction GetTransaction(string txId);

        decimal GetBalance(string account = "");

        string MakeRawRequest(string method, params object[] parameters);

        Dictionary<string, decimal> ListAccounts();

        string GetAccount(string bitcoinAddress);

        string SendMany(string fromAccount, Dictionary<string, decimal> toBitcoinAddress, int minConf = 1, string comment = "");
    }
}
