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
using CoiniumServ.Banning;
using CoiniumServ.Blocks;
using CoiniumServ.Coin.Config;
using CoiniumServ.Cryptology.Algorithms;
using CoiniumServ.Daemon;
using CoiniumServ.Daemon.Config;
using CoiniumServ.Jobs.Manager;
using CoiniumServ.Jobs.Tracker;
using CoiniumServ.Logging;
using CoiniumServ.Markets;
using CoiniumServ.Metrics;
using CoiniumServ.Miners;
using CoiniumServ.Payments;
using CoiniumServ.Payments.New;
using CoiniumServ.Persistance.Blocks;
using CoiniumServ.Persistance.Layers;
using CoiniumServ.Persistance.Layers.Hybrid;
using CoiniumServ.Persistance.Providers;
using CoiniumServ.Persistance.Providers.MySql;
using CoiniumServ.Pools;
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
        #region global objects
        IPoolManager GetPoolManager();

        IStatisticsManager GetStatisticsManager();

        ILogManager GetLogManager();

        IDaemonManager GetPaymentDaemonManager();

        #endregion

        #region pool objects

        IPool GetPool(IPoolConfig poolConfig);

        /// <summary>
        /// Returns a new instance of daemon client.
        /// </summary>
        /// <returns></returns>
        IDaemonClient GetDaemonClient(IDaemonConfig daemonConfig, ICoinConfig coinConfig);

        IMinerManager GetMinerManager(IPoolConfig poolConfig, IStorageLayer storageLayer);

        IJobManager GetJobManager(IPoolConfig poolConfig, IDaemonClient daemonClient, IJobTracker jobTracker, IShareManager shareManager, IMinerManager minerManager, IHashAlgorithm hashAlgorithm);

        IJobTracker GetJobTracker(IPoolConfig poolConfig);

        IShareManager GetShareManager(IPoolConfig poolConfig, IDaemonClient daemonClient, IJobTracker jobTracker, IStorageLayer storageLayer, IBlockProcessor blockProcessor);

        IPaymentProcessor GetPaymentProcessor(IPoolConfig poolConfig, IDaemonClient daemonClient, IStorageLayer storageLayer, IBlockProcessor blockProcessor);

        IBlockProcessor GetBlockProcessor(IPoolConfig poolConfig, IDaemonClient daemonClient, IStorageLayer storageLayer);

        IBanManager GetBanManager(IPoolConfig poolConfig, IShareManager shareManager);

        IVardiffManager GetVardiffManager(IPoolConfig poolConfig, IShareManager shareManager);

        INetworkInfo GetNetworkInfo(IDaemonClient daemonClient, IHashAlgorithm hashAlgorithm, IPoolConfig poolConfig);

        IBlocksCache GetBlocksCache(IStorageLayer storageLayer);

        IMiningServer GetMiningServer(string type, IPoolConfig poolConfig, IPool pool, IMinerManager minerManager, IJobManager jobManager,IBanManager banManager);

        IRpcService GetMiningService(string type, IPoolConfig poolConfig, IShareManager shareManager, IDaemonClient daemonClient);

        #endregion        

        #region payment objects 

        IBlockAccounter GetPaymentCalculator(IPoolConfig poolConfig, IStorageLayer storageLayer);

        INewPaymentRound GetPaymentRound(IPersistedBlock block, IStorageLayer storageLayer);

        #endregion

        #region hash algorithms

        /// <summary>
        /// Returns instance of the given hash algorithm
        /// </summary>
        /// <param name="algorithm"></param>
        /// <returns></returns>
        IHashAlgorithm GetHashAlgorithm(string algorithm);

        IAlgorithmManager GetAlgorithmManager();

        #endregion

        #region storage objects

        IStorageProvider GetStorageProvider(string type, IPoolConfig poolConfig, IStorageProviderConfig config);

        IStorageLayer GetStorageLayer(string type, IEnumerable<IStorageProvider> providers, IDaemonClient daemonClient, IPoolConfig poolConfig);

        IMigrationManager GetMigrationManager(IMySqlProvider provider, IPoolConfig poolConfig);

        #endregion

        #region web-server objects

        IWebServer GetWebServer();

        INancyBootstrapper GetWebBootstrapper();

        IMetricsManager GetMetricsManager();

        #endregion

        #region market objects

        IMarketManager GetMarketManager();

        #endregion
    }
}
