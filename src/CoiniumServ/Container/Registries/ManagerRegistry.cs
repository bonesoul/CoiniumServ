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

using CoiniumServ.Banning;
using CoiniumServ.Configuration;
using CoiniumServ.Container.Context;
using CoiniumServ.Daemon;
using CoiniumServ.Jobs.Manager;
using CoiniumServ.Logging;
using CoiniumServ.Mining;
using CoiniumServ.Mining.Software;
using CoiniumServ.Payments;
using CoiniumServ.Pools;
using CoiniumServ.Shares;
using CoiniumServ.Statistics;
using CoiniumServ.Vardiff;

namespace CoiniumServ.Container.Registries
{
    public class ManagerRegistry : IRegistry
    {
        private readonly IApplicationContext _applicationContext;

        public ManagerRegistry(IApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public void RegisterInstances()
        {
            // global singleton managers.            
            _applicationContext.Container.Register<IPoolManager, PoolManager>().AsSingleton();
            _applicationContext.Container.Register<IConfigManager, ConfigManager>().AsSingleton();
            _applicationContext.Container.Register<IStatisticsManager, StatisticsManager>().AsSingleton();
            _applicationContext.Container.Register<ILogManager, LogManager>().AsSingleton();
            _applicationContext.Container.Register<IDaemonManager, DaemonManager>().AsSingleton();
            _applicationContext.Container.Register<ISoftwareRepository, SoftwareRepository>().AsSingleton();

            // per-pool managers
            _applicationContext.Container.Register<IShareManager, ShareManager>().AsMultiInstance();
            _applicationContext.Container.Register<IMinerManager, MinerManager>().AsMultiInstance();
            _applicationContext.Container.Register<IJobManager, JobManager>().AsMultiInstance();
            _applicationContext.Container.Register<IMinerManager, MinerManager>().AsMultiInstance();
            _applicationContext.Container.Register<IVardiffManager, VardiffManager>().AsMultiInstance();
            _applicationContext.Container.Register<IBanManager, BanManager>().AsMultiInstance();
            _applicationContext.Container.Register<IPaymentManager, PaymentManager>().AsMultiInstance();
        }
    }
}
