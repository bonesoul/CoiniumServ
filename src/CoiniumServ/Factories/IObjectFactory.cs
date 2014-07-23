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

using CoiniumServ.Banning;
using CoiniumServ.Coin.Config;
using CoiniumServ.Cryptology.Algorithms;
using CoiniumServ.Daemon;
using CoiniumServ.Daemon.Config;
using CoiniumServ.Jobs.Manager;
using CoiniumServ.Jobs.Tracker;
using CoiniumServ.Logging;
using CoiniumServ.Miners;
using CoiniumServ.Payments;
using CoiniumServ.Persistance;
using CoiniumServ.Pools;
using CoiniumServ.Pools.Config;
using CoiniumServ.Server.Mining;
using CoiniumServ.Server.Mining.Service;
using CoiniumServ.Server.Web;
using CoiniumServ.Shares;
using CoiniumServ.Statistics;
using CoiniumServ.Vardiff;
using Nancy.Bootstrapper;

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

        IPaymentProcessor GetPaymentProcessor(string pool, IDaemonClient daemonClient, IStorage storage, IWalletConfig walletConfig);

        IBanManager GetBanManager(string pool, IShareManager shareManager, IBanConfig banConfig);

        IVardiffManager GetVardiffManager(string pool, IShareManager shareManager, IVardiffConfig vardiffConfig);

        #endregion

        #region pool statistics objects

        IStatistics GetStatistics();

        IGlobal GetGlobalStatistics();

        IAlgorithms GetAlgorithmStatistics();

        IPools GetPoolStats();

        IPerPool GetPerPoolStats(IPoolConfig poolConfig, IDaemonClient daemonClient, IMinerManager minerManager, IHashAlgorithm hashAlgorithm, IBlocks blockStatistics, IStorage storage);

        IBlocks GetBlockStats(ILatestBlocks latestBlocks, IStorage storage);

        ILatestBlocks GetLatestBlocks(IStorage storage);

        #endregion

        #region server & service objects

        IMiningServer GetMiningServer(string type, IPool pool, IMinerManager minerManager, IJobManager jobManager,
            IBanManager banManager, ICoinConfig coinConfig);

        IRpcService GetMiningService(string type, ICoinConfig coinConfig, IShareManager shareManager, IDaemonClient daemonClient);

        IWebServer GetWebServer();

        INancyBootstrapper GetWebBootstrapper();

        #endregion

        #region other objects
        IStorage GetStorage(string type, IPoolConfig poolConfig);

        ILogManager GetLogManager();

        #endregion
    }
}
