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
using CoiniumServ.Accounts;
using CoiniumServ.Algorithms;
using CoiniumServ.Banning;
using CoiniumServ.Blocks;
using CoiniumServ.Coin.Config;
using CoiniumServ.Daemon;
using CoiniumServ.Daemon.Config;
using CoiniumServ.Jobs.Manager;
using CoiniumServ.Jobs.Tracker;
using CoiniumServ.Logging;
using CoiniumServ.Markets;
using CoiniumServ.Markets.Exchanges;
using CoiniumServ.Mining;
using CoiniumServ.Mining.Software;
using CoiniumServ.Payments;
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
using CoiniumServ.Utils.Metrics;
using CoiniumServ.Vardiff;
using Nancy.Bootstrapper;

namespace CoiniumServ.Container
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

        IMinerManager GetMinerManager(IPoolConfig poolConfig, IStorageLayer storageLayer, IAccountManager accountManager);

        IJobManager GetJobManager(IPoolConfig poolConfig, IDaemonClient daemonClient, IJobTracker jobTracker, IShareManager shareManager, IMinerManager minerManager, IHashAlgorithm hashAlgorithm);

        IJobTracker GetJobTracker(IPoolConfig poolConfig);

        IShareManager GetShareManager(IPoolConfig poolConfig, IDaemonClient daemonClient, IJobTracker jobTracker, IStorageLayer storageLayer);

        IBlockProcessor GetBlockProcessor(IPoolConfig poolConfig, IDaemonClient daemonClient, IStorageLayer storageLayer);

        IBanManager GetBanManager(IPoolConfig poolConfig, IShareManager shareManager);

        IVardiffManager GetVardiffManager(IPoolConfig poolConfig, IShareManager shareManager);

        INetworkInfo GetNetworkInfo(IDaemonClient daemonClient, IHashAlgorithm hashAlgorithm, IPoolConfig poolConfig);

        IProfitInfo GetProfitInfo(INetworkInfo networkInfo, IPoolConfig poolConfig);

        IBlockRepository GetBlockRepository(IStorageLayer storageLayer);

        IMiningServer GetMiningServer(string type, IPoolConfig poolConfig, IPool pool, IMinerManager minerManager, IJobManager jobManager,IBanManager banManager);

        IRpcService GetMiningService(string type, IPoolConfig poolConfig, IShareManager shareManager, IDaemonClient daemonClient);

        IAccountManager GetAccountManager(IStorageLayer storageLayer, IPoolConfig poolConfig);

        #endregion        

        #region payment objects 

        IPaymentManager GetPaymentManager(IPoolConfig poolConfig, IBlockProcessor blockProcessor, IBlockAccounter blockAccounter, IPaymentProcessor paymentProcessor);

        IBlockAccounter GetBlockAccounter(IPoolConfig poolConfig, IStorageLayer storageLayer, IAccountManager accountManager);

        IPaymentProcessor GetPaymentProcessor(IPoolConfig poolConfig, IStorageLayer storageLayer, IDaemonClient daemonClient, IAccountManager accountManager);

        IPaymentRound GetPaymentRound(IPersistedBlock block, IStorageLayer storageLayer, IAccountManager accountManager);

        IPaymentRepository GetPaymentRepository(IStorageLayer storageLayer);

        #endregion

        #region hash algorithms

        /// <summary>
        /// Returns the given hash algorithm helper.
        /// </summary>
        /// <returns></returns>
        IHashAlgorithm GetHashAlgorithm(ICoinConfig coinConfig);

        IAlgorithmManager GetAlgorithmManager();

        IHashAlgorithmStatistics GetHashAlgorithmStatistics(string name);

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

        IBittrexClient GetBittrexClient();

        ICryptsyClient GetCryptsyClient();

        IPoloniexClient GetPoloniexClient();

        #endregion

        #region mining software

        ISoftwareRepository GetSoftwareRepository();

        IMiningSoftware GetMiningSoftware(IMiningSoftwareConfig config);

        #endregion
    }
}
