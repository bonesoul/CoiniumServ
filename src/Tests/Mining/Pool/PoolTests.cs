/*
 *   CoiniumServ - crypto currency pool software - https://github.com/CoiniumServ/CoiniumServ
 *   Copyright (C) 2013 - 2014, Coinium Project - http://www.coinium.org
 *
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using Coinium.Coin.Algorithms;
using Coinium.Coin.Daemon;
using Coinium.Miner;
using Coinium.Mining.Jobs;
using Coinium.Mining.Pool.Config;
using Coinium.Mining.Share;
using Coinium.Rpc.Service;
using Coinium.Server;
using NSubstitute;
using Should.Fluent;
using Xunit;

namespace Tests.Mining.Pool
{
    public class PoolTests
    {

        private readonly IServerFactory _serverFactory;
        private readonly IServiceFactory _serviceFactory;
        private readonly IJobManagerFactory _jobManagerFactory;
        private readonly IShareManagerFactory _shareManagerFactory;
        private readonly IHashAlgorithmFactory _hashAlgorithmFactory;

        private readonly IDaemonClient _daemonClient;
        private readonly IMinerManager _minerManager;
        private readonly IJobManager _jobManager;
        private readonly IShareManager _shareManager;
        private readonly IMiningServer _miningServer;
        private readonly IRpcService _rpcService;

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

        [Fact]
        public void ConstructorTest_NonNullParams_ShouldSucceed()
        {
            var pool = new Coinium.Mining.Pool.Pool(
                _hashAlgorithmFactory,
                _serverFactory,
                _serviceFactory,
                _daemonClient,
                _minerManager,
                _jobManagerFactory,
                _shareManagerFactory);

            Assert.NotNull(pool);
            Assert.True(pool.InstanceId > 0, "InstanceId was not initialized.");
        }

        [Fact]
        public void ConstructorTest_NullHashAlgorithmFactory_ShouldThrow()
        {
            Exception ex = Assert.Throws<ArgumentNullException>(() =>
            {
                var pool = new Coinium.Mining.Pool.Pool(
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

        [Fact]
        public void ConstructorTest_NullServerFactory_ShouldThrow()
        {
            Exception ex = Assert.Throws<ArgumentNullException>(() =>
            {
                var pool = new Coinium.Mining.Pool.Pool(
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

        [Fact]
        public void ConstructorTest_NullServiceFactory_ShouldThrow()
        {
            Exception ex = Assert.Throws<ArgumentNullException>(() =>
            {
                var pool = new Coinium.Mining.Pool.Pool(
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

        [Fact]
        public void ConstructorTest_NullDaemonClient_ShouldThrow()
        {
            Exception ex = Assert.Throws<ArgumentNullException>(() =>
            {
                var pool = new Coinium.Mining.Pool.Pool(
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

        [Fact]
        public void ConstructorTest_NullMinerManager_ShouldThrow()
        {
            Exception ex = Assert.Throws<ArgumentNullException>(() =>
            {
                var pool = new Coinium.Mining.Pool.Pool(
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

        [Fact]
        public void ConstructorTest_NullJobManagerFactory_ShouldThrow()
        {
            Exception ex = Assert.Throws<ArgumentNullException>(() =>
            {
                var pool = new Coinium.Mining.Pool.Pool(
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

        [Fact]
        public void ConstructorTest_NullShareManagerFactory_ShouldThrow()
        {
            Exception ex = Assert.Throws<ArgumentNullException>(() =>
            {
                var pool = new Coinium.Mining.Pool.Pool(
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


        [Fact]
        public void InitializationTest_NonNullParams_ShouldSuccess()
        {
            var pool = new Coinium.Mining.Pool.Pool(
                _hashAlgorithmFactory, 
                _serverFactory, 
                _serviceFactory, 
                _daemonClient,
                _minerManager, 
                _jobManagerFactory, 
                _shareManagerFactory);

            pool.Should().Not.Be.Null();
            Assert.True(pool.InstanceId > 0, "InstanceId was not initialized.");

            // pool-config mockup.
            var config = Substitute.For<IPoolConfig>();
            config.Daemon.Valid.Returns(true);

            // initalize job manager.
            _jobManagerFactory.Get(_daemonClient, _minerManager).Returns(_jobManager);
            _jobManager.Initialize(pool.InstanceId);

            // initialize hash algorithm
            var hashAlgorithm = Substitute.For<IHashAlgorithm>();
            _hashAlgorithmFactory.Get(config.Coin.Algorithm).Returns(hashAlgorithm);

            // initialize share manager.
            _shareManagerFactory.Get(hashAlgorithm, _jobManager, _daemonClient).Returns(_shareManager);
        
            // init daemon client
            _daemonClient.Initialize(config.Daemon);

            // init server
            _serverFactory.Get(RpcServiceNames.Stratum, _minerManager).Returns(_miningServer);

            // init service
            _serviceFactory.Get(RpcServiceNames.Stratum, _jobManager, _shareManager, _daemonClient).Returns(_rpcService);

            _miningServer.Initialize(config.Stratum);

            pool.Initialize(config);
        }
    }
}
