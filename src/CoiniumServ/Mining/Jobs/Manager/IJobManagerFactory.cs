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
using Coinium.Crypto.Algorithms;
using Coinium.Daemon;
using Coinium.Mining.Jobs.Tracker;
using Coinium.Mining.Miners;
using Coinium.Mining.Pools.Config;
using Coinium.Mining.Shares;

namespace Coinium.Mining.Jobs.Manager
{
    public interface IJobManagerFactory
    {
        /// <summary>
        /// Gets the specified daemon client.
        /// </summary>
        /// <param name="daemonClient">The daemon client.</param>
        /// <param name="jobtracker"></param>
        /// <param name="shareManager"></param>
        /// <param name="minerManager">The miner manager.</param>
        /// <param name="hashAlgorithm"></param>
        /// <param name="walletConfig"></param>
        /// <param name="rewardsConfig"></param>
        /// <returns></returns>
        IJobManager Get(IDaemonClient daemonClient, IJobTracker jobtracker, IShareManager shareManager, IMinerManager minerManager, IHashAlgorithm hashAlgorithm, IWalletConfig walletConfig, IRewardsConfig rewardsConfig);
    }
}
