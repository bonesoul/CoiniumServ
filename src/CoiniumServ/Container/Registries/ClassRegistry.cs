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

using CoiniumServ.Accounts;
using CoiniumServ.Blocks;
using CoiniumServ.Coin.Config;
using CoiniumServ.Configuration;
using CoiniumServ.Container.Context;
using CoiniumServ.Daemon;
using CoiniumServ.Jobs.Tracker;
using CoiniumServ.Mining.Software;
using CoiniumServ.Payments;
using CoiniumServ.Pools;
using CoiniumServ.Server.Web;
using Nancy.Bootstrapper;

namespace CoiniumServ.Container.Registries
{
    public class ClassRegistry : IRegistry
    {
        private readonly IApplicationContext _applicationContext;

        public ClassRegistry(IApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public void RegisterInstances()
        {
            // per-pool objects
            _applicationContext.Container.Register<IPool, Pool>().AsMultiInstance();
            _applicationContext.Container.Register<IDaemonClient, DaemonClient>().AsMultiInstance();
            _applicationContext.Container.Register<IJobTracker, JobTracker>().AsMultiInstance();
            _applicationContext.Container.Register<IBlockProcessor, BlockProcessor>().AsMultiInstance();
            _applicationContext.Container.Register<INetworkInfo, NetworkInfo>().AsMultiInstance();
            _applicationContext.Container.Register<IBlockRepository, BlocksRepository>().AsMultiInstance();
            _applicationContext.Container.Register<IPaymentRepository, PaymentRepository>().AsMultiInstance();
            _applicationContext.Container.Register<IAccountManager, AccountManager>().AsMultiInstance();

            // payment objects
            _applicationContext.Container.Register<IBlockAccounter, BlockAccounter>().AsMultiInstance();
            _applicationContext.Container.Register<IPaymentRound, PaymentRound>().AsMultiInstance();
            _applicationContext.Container.Register<IPaymentProcessor, PaymentProcessor>().AsMultiInstance();

            // config
            _applicationContext.Container.Register<IJsonConfigReader, JsonConfigReader>().AsSingleton();
            _applicationContext.Container.Register<IPoolConfig, PoolConfig>().AsMultiInstance();
            _applicationContext.Container.Register<ICoinConfig, CoinConfig>().AsMultiInstance();
            _applicationContext.Container.Register<IMiningSoftwareConfig, MiningSoftwareConfig>().AsMultiInstance();

            // web
            _applicationContext.Container.Register<INancyBootstrapper, NancyBootstrapper>().AsSingleton();

            // software
            _applicationContext.Container.Register<IMiningSoftware, MiningSoftware>().AsMultiInstance();
        }
    }
}