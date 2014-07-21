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

using CoiniumServ.Coin.Config;
using CoiniumServ.Cryptology.Algorithms;
using CoiniumServ.Daemon;
using CoiniumServ.Daemon.Config;
using CoiniumServ.Mining.Banning;
using CoiniumServ.Mining.Jobs.Manager;
using CoiniumServ.Mining.Jobs.Tracker;
using CoiniumServ.Mining.Miners;
using CoiniumServ.Mining.Pools;
using CoiniumServ.Mining.Pools.Config;
using CoiniumServ.Mining.Shares;
using CoiniumServ.Persistance;

namespace CoiniumServ.Factories
{
    /// <summary>
    /// Object factory that creates instances of objects
    /// </summary>
    public interface IObjectFactory
    {
        #region hash algorithms

        /// <summary>
        /// Returns instance of the given hash algorithm
        /// </summary>
        /// <param name="algorithm"></param>
        /// <returns></returns>
        IHashAlgorithm GetHashAlgorithm(string algorithm);

        #endregion

        #region pool objects

        IPoolManager GetPoolManager();

        IPool GetPool(IPoolConfig poolConfig);

        /// <summary>
        /// Returns a new instance of daemon client.
        /// </summary>
        /// <param name="pool"></param>
        /// <param name="daemonConfig"></param>
        /// <returns></returns>
        IDaemonClient GetDaemonClient(string pool, IDaemonConfig daemonConfig);

        IMinerManager GetMiningManager(string pool, IDaemonClient daemonClient);

        IJobManager GetJobManager(string pool, IDaemonClient daemonClient, IJobTracker jobTracker, IShareManager shareManager,
           IMinerManager minerManager, IHashAlgorithm hashAlgorithm, IWalletConfig walletConfig,
           IRewardsConfig rewardsConfig);

        IJobTracker GetJobTracker();

        IShareManager GetShareManager(string pool, IDaemonClient daemonClient, IJobTracker jobTracker, IStorage storage);

        IBanManager GetBanManager(string pool, IShareManager shareManager, IBanConfig banConfig);

        #endregion
    }
}
