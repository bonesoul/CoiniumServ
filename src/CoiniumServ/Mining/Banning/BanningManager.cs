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

using System;
using CoiniumServ.Mining.Miners;
using CoiniumServ.Mining.Pools.Config;
using CoiniumServ.Mining.Shares;
using CoiniumServ.Net.Server.Sockets;
using CoiniumServ.Server.Mining.Vanilla;

namespace CoiniumServ.Mining.Banning
{
    public class BanningManager:IBanningManager
    {
        public IBanningConfig Config { get; private set; }

        public BanningManager(IBanningConfig banningConfig, IShareManager shareManager)
        {
            Config = banningConfig;

            if (!Config.Enabled)
                return;

            shareManager.ShareSubmitted += OnShare;
        }

        private void OnShare(object sender, EventArgs e)
        {
            var shareArgs = (ShareEventArgs)e;
            var miner = shareArgs.Miner;

            if (miner == null)
                return;

            var totalShares = miner.ValidShares + miner.InvalidShares;

            if (totalShares < Config.CheckThreshold) // check if we exceeded the threshold for checks.
                return;

            var invalidPercentage = miner.InvalidShares/totalShares*100;

            if (invalidPercentage < Config.InvalidPercent)
                // if the miner didn't reach the invalid share percentage, reset his stats.
            {
                miner.ValidShares = 0;
                miner.InvalidShares = 0;
            }
            else // he needs a ban
                Ban(miner);
        }

        private void Ban(IMiner miner)
        {
            // todo: add ip to banlist.

            if (miner is IVanillaMiner) // as vanilla miners doesn't use persistent connections, we don't need to disconect him
                return; // but just blacklist hip ip.

            var client = (IClient) miner;
            client.Connection.Disconnect();
        }
    }
}
