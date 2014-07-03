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
using Coinium.Crypto.Algorithms;
using Coinium.Daemon;
using Coinium.Mining.Jobs.Manager;
using Coinium.Mining.Jobs.Tracker;
using Coinium.Mining.Miners;
using Coinium.Mining.Pools.Config;
using Coinium.Mining.Shares;
using Coinium.Payments;
using Coinium.Persistance;
using Coinium.Server;
using Coinium.Service;
using Coinium.Utils.Configuration;
using NSubstitute;
using Should.Fluent;
using Xunit;

namespace Tests.Mining.Pools
{
    public class PoolTests
    {
        // factory mocks
        private readonly IServerFactory _serverFactory;
        private readonly IServiceFactory _serviceFactory;
        private readonly IJobManagerFactory _jobManagerFactory;
        private readonly IJobTrackerFactory _jobTrackerFactory;
        private readonly IShareManagerFactory _shareManagerFactory;
        private readonly IHashAlgorithmFactory _hashAlgorithmFactory;
        private readonly IMinerManagerFactory _minerManagerFactory;
        private readonly IStorageFactory _storageFactory;
        private readonly IPaymentProcessorFactory _paymentProcessorFactory;

        // object mocks.
        private readonly IDaemonClient _daemonClient;
        private readonly IMinerManager _minerManager;
        private readonly IJobManager _jobManager;
        private readonly IJobTracker _jobTracker;
        private readonly IShareManager _shareManager;
        private readonly IStorage _storage;
        private readonly IMiningServer _miningServer;
        private readonly IRpcService _rpcService;
        private readonly IPaymentProcessor _paymentProcessor;

        /// <summary>
        /// Initialize mock objects.
        /// </summary>
        public PoolTests()
        {
            _jobManagerFactory = Substitute.For<IJobManagerFactory>();
            _jobTrackerFactory = Substitute.For<IJobTrackerFactory>();
            _hashAlgorithmFactory = Substitute.For<IHashAlgorithmFactory>();
            _shareManagerFactory = Substitute.For<IShareManagerFactory>();
            _minerManagerFactory = Substitute.For<IMinerManagerFactory>();
            _serverFactory = Substitute.For<IServerFactory>();
            _serviceFactory = Substitute.For<IServiceFactory>();
            _storageFactory = Substitute.For<IStorageFactory>();
            _paymentProcessorFactory = Substitute.For<IPaymentProcessorFactory>();

            _daemonClient = Substitute.For<IDaemonClient>();
            _minerManager = Substitute.For<IMinerManager>();
            _jobManager = Substitute.For<IJobManager>();
            _jobTracker = Substitute.For<IJobTracker>();
            _shareManager = Substitute.For<IShareManager>();
            _miningServer = Substitute.For<IMiningServer>();
            _rpcService = Substitute.For<IRpcService>();
            _storage = Substitute.For<IStorage>();
            _paymentProcessor = Substitute.For<IPaymentProcessor>();
        }

        /// <summary>
        /// Tests pool constructor with all valid parameters. Should succeed.
        /// </summary>
        [Fact]
        public void ConstructorTest_NonNullParams_ShouldSucceed()
        {
            var pool = new Coinium.Mining.Pools.Pool(
                _hashAlgorithmFactory,
                _serverFactory,
                _serviceFactory,
                _daemonClient,
                _minerManagerFactory,
                _jobTrackerFactory,
                _jobManagerFactory,
                _shareManagerFactory,
                _storageFactory,
                _paymentProcessorFactory
                );

            pool.Should().Not.Be.Null();
            pool.InstanceId.Should().Be.GreaterThan((UInt32)0);
        }

        /// <summary>
        /// Tests pool constructor with null HashAlgorithm, should trow exception.
        /// </summary>
        [Fact]
        public void ConstructorTest_NullHashAlgorithmFactory_ShouldThrow()
        {
            Exception ex = Assert.Throws<ArgumentNullException>(() =>
            {
                var pool = new Coinium.Mining.Pools.Pool(
                    null,
                    _serverFactory,
                    _serviceFactory,
                    _daemonClient,
                    _minerManagerFactory,
                    _jobTrackerFactory,
                    _jobManagerFactory,
                    _shareManagerFactory,
                    _storageFactory,
                    _paymentProcessorFactory
                    );
            });

            ex.Message.Should().Contain("IHashAlgorithmFactory");
        }

        /// <summary>
        /// Tests pool constructor with null ServerFactory, should trow exception.
        /// </summary>
        [Fact]
        public void ConstructorTest_NullServerFactory_ShouldThrow()
        {
            Exception ex = Assert.Throws<ArgumentNullException>(() =>
            {
                var pool = new Coinium.Mining.Pools.Pool(
                    _hashAlgorithmFactory,
                    null,
                    _serviceFactory,
                    _daemonClient,
                    _minerManagerFactory,
                    _jobTrackerFactory,
                    _jobManagerFactory,
                    _shareManagerFactory,
                    _storageFactory,
                    _paymentProcessorFactory
                    );
            });

            ex.Message.Should().Contain("IServerFactory");
        }

        /// <summary>
        /// Tests pool constructor with null ServiceFactory, should trow exception.
        /// </summary>
        [Fact]
        public void ConstructorTest_NullServiceFactory_ShouldThrow()
        {
            Exception ex = Assert.Throws<ArgumentNullException>(() =>
            {
                var pool = new Coinium.Mining.Pools.Pool(
                    _hashAlgorithmFactory,
                    _serverFactory,
                    null,
                    _daemonClient,
                    _minerManagerFactory,
                    _jobTrackerFactory,
                    _jobManagerFactory,
                    _shareManagerFactory,
                    _storageFactory,
                    _paymentProcessorFactory
                    );
            });

            ex.Message.Should().Contain("IServiceFactory");
        }

        /// <summary>
        /// Tests pool constructor with null DaemonClient, should trow exception.
        /// </summary>
        [Fact]
        public void ConstructorTest_NullDaemonClient_ShouldThrow()
        {
            Exception ex = Assert.Throws<ArgumentNullException>(() =>
            {
                var pool = new Coinium.Mining.Pools.Pool(
                    _hashAlgorithmFactory,
                    _serverFactory,
                    _serviceFactory,
                    null,
                    _minerManagerFactory,
                    _jobTrackerFactory,
                    _jobManagerFactory,
                    _shareManagerFactory,
                    _storageFactory,
                    _paymentProcessorFactory
                    );
            });

            ex.Message.Should().Contain("IDaemonClient");
        }

        /// <summary>
        /// Tests pool constructor with null IMinerManagerFactory, should trow exception.
        /// </summary>
        [Fact]
        public void ConstructorTest_NullMinerManagerFactory_ShouldThrow()
        {
            Exception ex = Assert.Throws<ArgumentNullException>(() =>
            {
                var pool = new Coinium.Mining.Pools.Pool(
                    _hashAlgorithmFactory,
                    _serverFactory,
                    _serviceFactory,
                    _daemonClient,
                    null,
                    _jobTrackerFactory,
                    _jobManagerFactory,
                    _shareManagerFactory,
                    _storageFactory,
                    _paymentProcessorFactory
                    );
            });

            ex.Message.Should().Contain("IMinerManagerFactory");
        }

        /// <summary>
        /// Tests pool constructor with null JobManager, should trow exception.
        /// </summary>
        [Fact]
        public void ConstructorTest_NullJobTrackerFactory_ShouldThrow()
        {
            Exception ex = Assert.Throws<ArgumentNullException>(() =>
            {
                var pool = new Coinium.Mining.Pools.Pool(
                    _hashAlgorithmFactory,
                    _serverFactory,
                    _serviceFactory,
                    _daemonClient,
                    _minerManagerFactory,
                    null,
                    _jobManagerFactory,
                    _shareManagerFactory,
                    _storageFactory,
                    _paymentProcessorFactory
                    );
            });

            ex.Message.Should().Contain("IJobTrackerFactory");
        }

        /// <summary>
        /// Tests pool constructor with null JobManager, should trow exception.
        /// </summary>
        [Fact]
        public void ConstructorTest_NullJobManagerFactory_ShouldThrow()
        {
            Exception ex = Assert.Throws<ArgumentNullException>(() =>
            {
                var pool = new Coinium.Mining.Pools.Pool(
                    _hashAlgorithmFactory,
                    _serverFactory,
                    _serviceFactory,
                    _daemonClient,
                    _minerManagerFactory,
                    _jobTrackerFactory,
                    null,
                    _shareManagerFactory,
                    _storageFactory,
                    _paymentProcessorFactory
                    );
            });

            ex.Message.Should().Contain("IJobManagerFactory");
        }

        /// <summary>
        /// Tests pool constructor with null ShareManager, should trow exception.
        /// </summary>
        [Fact]
        public void ConstructorTest_NullShareManagerFactory_ShouldThrow()
        {
            Exception ex = Assert.Throws<ArgumentNullException>(() =>
            {
                var pool = new Coinium.Mining.Pools.Pool(
                    _hashAlgorithmFactory,
                    _serverFactory,
                    _serviceFactory,
                    _daemonClient,
                    _minerManagerFactory,
                    _jobTrackerFactory,
                    _jobManagerFactory,
                    null,
                    _storageFactory,
                    _paymentProcessorFactory
                    );
            });

            ex.Message.Should().Contain("IShareManagerFactory");
        }

        /// <summary>
        /// Tests pool constructor with null StorageFactory, should trow exception.
        /// </summary>
        [Fact]
        public void ConstructorTest_NullStorageFactory_ShouldThrow()
        {
            Exception ex = Assert.Throws<ArgumentNullException>(() =>
            {
                var pool = new Coinium.Mining.Pools.Pool(
                    _hashAlgorithmFactory,
                    _serverFactory,
                    _serviceFactory,
                    _daemonClient,
                    _minerManagerFactory,
                    _jobTrackerFactory,
                    _jobManagerFactory,
                    _shareManagerFactory,
                    null,
                    _paymentProcessorFactory
                    );
            });

            ex.Message.Should().Contain("IStorageFactory");
        }

        /// <summary>
        /// Tests pool constructor with null StorageFactory, should trow exception.
        /// </summary>
        [Fact]
        public void ConstructorTest_NullPaymentProcessorFactory_ShouldThrow()
        {
            Exception ex = Assert.Throws<ArgumentNullException>(() =>
            {
                var pool = new Coinium.Mining.Pools.Pool(
                    _hashAlgorithmFactory,
                    _serverFactory,
                    _serviceFactory,
                    _daemonClient,
                    _minerManagerFactory,
                    _jobTrackerFactory,
                    _jobManagerFactory,
                    _shareManagerFactory,
                    _storageFactory,
                    null
                    );
            });

            ex.Message.Should().Contain("IPaymentProcessorFactory");
        }

        /// <summary>
        /// Initializes pool with all valid parameters, should succeed.
        /// </summary>
        [Fact]
        public void InitializationTest_NonNullParams_ShouldSuccess()
        {
            var pool = new Coinium.Mining.Pools.Pool(
                _hashAlgorithmFactory, 
                _serverFactory, 
                _serviceFactory, 
                _daemonClient,
                _minerManagerFactory,
                _jobTrackerFactory,
                _jobManagerFactory, 
                _shareManagerFactory,
                _storageFactory,
                _paymentProcessorFactory
                );

            pool.Should().Not.Be.Null();
            pool.InstanceId.Should().Be.GreaterThan((UInt32)0);

            // pool-config mockup.
            var poolConfig = Substitute.For<IPoolConfig>();
            poolConfig.Daemon.Valid.Returns(true);

            // initialize hash algorithm
            var hashAlgorithm = Substitute.For<IHashAlgorithm>();
            _hashAlgorithmFactory.Get(poolConfig.Coin.Algorithm).Returns(hashAlgorithm);

            // initialize the miner manager.
            _minerManagerFactory.Get(_daemonClient);

            // payment processor
            _paymentProcessorFactory.Get(_daemonClient, _storage);

            // initialize storage manager
            _storageFactory.Get(Storages.Redis, poolConfig);

            // initialize the job tracker
            _jobTrackerFactory.Get();

            // initialize share manager.
            _shareManagerFactory.Get(_daemonClient, _jobTracker, _storage).Returns(_shareManager);

            // initalize job manager.
            _jobManagerFactory.Get(_daemonClient, _jobTracker, _shareManager, _minerManager, hashAlgorithm).Returns(_jobManager);
            _jobManager.Initialize(pool.InstanceId);
        
            // init daemon client
            _daemonClient.Initialize(poolConfig.Daemon);

            // init server
            _serverFactory.Get(Services.Stratum, pool, _minerManager, _jobManager).Returns(_miningServer);

            // init service
            _serviceFactory.Get(Services.Stratum, _shareManager, _daemonClient).Returns(_rpcService);

            // initalize the server.
            _miningServer.Initialize(poolConfig.Stratum);

            // initialize the pool.
            pool.Initialize(poolConfig);
        }
    }
}
