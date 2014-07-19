#region License
// 
//     CoiniumServ - Crypto Currency Mining Pool Server Software
//     Copyright (C) 2013 - 2014, CoiniumServ Project - http://www.coinium.org
//     http://www.coiniumserv.com - https://github.com/CoiniumServ/CoiniumServ
// 
//     This software is dual-licensed: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
//    
//     For the terms of this license, see licenses/gpl_v3.txt.
// 
//     Alternatively, you can license this software under a commercial
//     license or white-label it as set out in licenses/commercial.txt.
// 
#endregion

using System.Collections.Generic;
using CoiniumServ.Coin.Config;
using CoiniumServ.Daemon.Config;
using CoiniumServ.Daemon.Responses;

namespace CoiniumServ.Daemon
{
    public interface IDaemonClient
    {
        BlockTemplate GetBlockTemplate();

        BlockTemplate GetBlockTemplate(string blockHex);

        string SubmitBlock(string blockHex);

        Block GetBlock(string hash);

        Getwork Getwork();

        bool Getwork(string data);

        Info GetInfo();

        MiningInfo GetMiningInfo();

        ValidateAddress ValidateAddress(string walletAddress);

        Transaction GetTransaction(string txId);

        decimal GetBalance(string account = "");

        string MakeRawRequest(string method, params object[] parameters);

        Dictionary<string, decimal> ListAccounts();

        string GetAccount(string bitcoinAddress);

        string SendMany(string fromAccount, Dictionary<string, decimal> toBitcoinAddress, int minConf = 1, string comment = "");
    }
}
