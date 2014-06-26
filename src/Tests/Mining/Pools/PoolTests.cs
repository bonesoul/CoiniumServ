#region License
// 
//     CoiniumServ - Crypto Currency Mining Pool Server Software
//     Copyright (C) 2013 - 2014, CoiniumServ Project - http://www.coinium.org
//     https://github.com/CoiniumServ/CoiniumServ
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
using Coinium.Coin.Algorithms;
using Coinium.Coin.Daemon;
using Coinium.Mining.Jobs;
using Coinium.Mining.Miners;
using Coinium.Mining.Pools.Config;
using Coinium.Mining.Shares;
using Coinium.Rpc.Service;
using Coinium.Server;
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
        private readonly IShareManagerFactory _shareManagerFactory;
        private readonly IHashAlgorithmFactory _hashAlgorithmFactory;

        // object mocks.
        private readonly IDaemonClient _daemonClient;
        private readonly IMinerManager _minerManager;
        private readonly IJobManager _jobManager;
        private readonly IShareManager _shareManager;
        private readonly IMiningServer _miningServer;
        private readonly IRpcService _rpcService;

        /// <summary>
        /// Initialize mock objects.
        /// </summary>
        public PoolTests()
        {
            _jobManagerFactory = Substitute.For<IJobManagerFactory>();
            _hashAlgorithmFactory = Substitute.For<IHashAlgorithmFactory>();
            _shareManagerFactory = Substitute.For<IShareManagerFactory>();
            _serverFactory = Substitute.For<IServerFactory>();
            _serviceFactory = Substitute.For<IServiceFactory>();

            _daemonClient = Substitute.For<IDaemonClient>();
            _minerManager = Substitute.For<IMinerManager>();
            _jobManager = Substitute.For<IJobManager>();
            _shareManager = Substitute.For<IShareManager>();
            _miningServer = Substitute.For<IMiningServer>();
            _rpcService = Substitute.For<IRpcService>();
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
                _minerManager,
                _jobManagerFactory,
                _shareManagerFactory);

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
                    _minerManager,
                    _jobManagerFactory,
                    _shareManagerFactory);
            });

            Assert.True(ex.Message.Contains("IHashAlgorithmFactory"));
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
                    _minerManager,
                    _jobManagerFactory,
                    _shareManagerFactory);
            });

            Assert.True(ex.Message.Contains("IServerFactory"));
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
                    _minerManager,
                    _jobManagerFactory,
                    _shareManagerFactory);
            });

            Assert.True(ex.Message.Contains("IServiceFactory"));
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
                    _minerManager,
                    _jobManagerFactory,
                    _shareManagerFactory);
            });

            Assert.True(ex.Message.Contains("IDaemonClient"));
        }

        /// <summary>
        /// Tests pool constructor with null MinerManager, should trow exception.
        /// </summary>
        [Fact]
        public void ConstructorTest_NullMinerManager_ShouldThrow()
        {
            Exception ex = Assert.Throws<ArgumentNullException>(() =>
            {
                var pool = new Coinium.Mining.Pools.Pool(
                    _hashAlgorithmFactory,
                    _serverFactory,
                    _serviceFactory,
                    _daemonClient,
                    null,
                    _jobManagerFactory,
                    _shareManagerFactory);
            });

            Assert.True(ex.Message.Contains("IMinerManager"));
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
                    _minerManager,
                    null,
                    _shareManagerFactory);
            });

            Assert.True(ex.Message.Contains("IJobManagerFactory"));
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
                    _minerManager,
                    _jobManagerFactory,
                    null);
            });

            Assert.True(ex.Message.Contains("IShareManagerFactory"));
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
                _minerManager, 
                _jobManagerFactory, 
                _shareManagerFactory);

            pool.Should().Not.Be.Null();
            pool.InstanceId.Should().Be.GreaterThan((UInt32)0);

            // pool-config mockup.
            var config = Substitute.For<IPoolConfig>();
            config.Daemon.Valid.Returns(true);

            // initialize hash algorithm
            var hashAlgorithm = Substitute.For<IHashAlgorithm>();
            _hashAlgorithmFactory.Get(config.Coin.Algorithm).Returns(hashAlgorithm);

            // initalize job manager.
            _jobManagerFactory.Get(_daemonClient, _minerManager, hashAlgorithm).Returns(_jobManager);
            _jobManager.Initialize(pool.InstanceId);

            // initialize share manager.
            _shareManagerFactory.Get(_jobManager, _daemonClient).Returns(_shareManager);
        
            // init daemon client
            _daemonClient.Initialize(config.Daemon);

            // init server
            _serverFactory.Get(RpcServiceNames.Stratum, _minerManager).Returns(_miningServer);

            // init service
            _serviceFactory.Get(RpcServiceNames.Stratum, _jobManager, _shareManager, _daemonClient).Returns(_rpcService);

            // initalize the server.
            _miningServer.Initialize(config.Stratum);

            // initialize the pool.
            pool.Initialize(config);
        }
    }
}
