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

        #endregion

        #region mining software

        ISoftwareRepository GetSoftwareRepository();

        IMiningSoftware GetMiningSoftware(IMiningSoftwareConfig config);

        #endregion
    }
}
