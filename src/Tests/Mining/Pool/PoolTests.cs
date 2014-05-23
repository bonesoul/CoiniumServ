using System;
using Coinium.Coin.Algorithms;
using Coinium.Coin.Daemon;
using Coinium.Miner;
using Coinium.Mining.Jobs;
using Coinium.Mining.Pool.Config;
using Coinium.Mining.Share;
using Coinium.Rpc.Service;
using Coinium.Server;
using Moq;
using Xunit;

namespace Tests.Mining.Pool
{
    public class PoolTests
    {

        private readonly Mock<IDaemonClient> _daemonClient;
        private readonly Mock<IMinerManager> _minerManager;
        private readonly Mock<IServerFactory >_serverFactory;
        private readonly Mock<IServiceFactory> _serviceFactory;
        private readonly Mock<IJobManagerFactory> _jobManagerFactory;
        private readonly Mock<IShareManagerFactory> _shareManagerFactory;
        private readonly Mock<IHashAlgorithmFactory> _hashAlgorithmFactory;
        private readonly Mock<IJobManager> _jobManager;
        private readonly Mock<IShareManager> _shareManager;
        private readonly Mock<IMiningServer> _server;
        private readonly Mock<IRpcService> _service;

        public PoolTests()
        {
            _daemonClient = new Mock<IDaemonClient>();
            _minerManager = new Mock<IMinerManager>();
            _jobManagerFactory = new Mock<IJobManagerFactory>();
            _shareManagerFactory = new Mock<IShareManagerFactory>();
            _serverFactory = new Mock<IServerFactory>();
            _serviceFactory = new Mock<IServiceFactory>();
            _hashAlgorithmFactory = new Mock<IHashAlgorithmFactory>();
            _jobManager = new Mock<IJobManager>();
            _shareManager = new Mock<IShareManager>();
            _server = new Mock<IMiningServer>();
            _service = new Mock<IRpcService>();
        }

        [Fact]
        public void ConstructorTest_NonNullParams_ShouldSucceed()
        {
            var pool = new Coinium.Mining.Pool.Pool(
                _hashAlgorithmFactory.Object,
                _serverFactory.Object,
                _serviceFactory.Object,
                _daemonClient.Object,
                _minerManager.Object,
                _jobManagerFactory.Object,
                _shareManagerFactory.Object);

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
                    _serverFactory.Object,
                    _serviceFactory.Object,
                    _daemonClient.Object,
                    _minerManager.Object,
                    _jobManagerFactory.Object,
                    _shareManagerFactory.Object);
            });

            Assert.True(ex.Message.Contains("IHashAlgorithmFactory"));
        }

        [Fact]
        public void ConstructorTest_NullServerFactory_ShouldThrow()
        {
            Exception ex = Assert.Throws<ArgumentNullException>(() =>
            {
                var pool = new Coinium.Mining.Pool.Pool(
                    _hashAlgorithmFactory.Object,
                    null,
                    _serviceFactory.Object,
                    _daemonClient.Object,
                    _minerManager.Object,
                    _jobManagerFactory.Object,
                    _shareManagerFactory.Object);
            });

            Assert.True(ex.Message.Contains("IServerFactory"));
        }

        [Fact]
        public void ConstructorTest_NullServiceFactory_ShouldThrow()
        {
            Exception ex = Assert.Throws<ArgumentNullException>(() =>
            {
                var pool = new Coinium.Mining.Pool.Pool(
                    _hashAlgorithmFactory.Object,
                    _serverFactory.Object,
                    null,
                    _daemonClient.Object,
                    _minerManager.Object,
                    _jobManagerFactory.Object,
                    _shareManagerFactory.Object);
            });

            Assert.True(ex.Message.Contains("IServiceFactory"));
        }

        [Fact]
        public void ConstructorTest_NullDaemonClient_ShouldThrow()
        {
            Exception ex = Assert.Throws<ArgumentNullException>(() =>
            {
                var pool = new Coinium.Mining.Pool.Pool(
                    _hashAlgorithmFactory.Object,
                    _serverFactory.Object,
                    _serviceFactory.Object,
                    null,
                    _minerManager.Object,
                    _jobManagerFactory.Object,
                    _shareManagerFactory.Object);
            });

            Assert.True(ex.Message.Contains("IDaemonClient"));
        }

        [Fact]
        public void ConstructorTest_NullMinerManager_ShouldThrow()
        {
            Exception ex = Assert.Throws<ArgumentNullException>(() =>
            {
                var pool = new Coinium.Mining.Pool.Pool(
                    _hashAlgorithmFactory.Object,
                    _serverFactory.Object,
                    _serviceFactory.Object,
                    _daemonClient.Object,
                    null,
                    _jobManagerFactory.Object,
                    _shareManagerFactory.Object);
            });

            Assert.True(ex.Message.Contains("IMinerManager"));
        }

        [Fact]
        public void ConstructorTest_NullJobManagerFactory_ShouldThrow()
        {
            Exception ex = Assert.Throws<ArgumentNullException>(() =>
            {
                var pool = new Coinium.Mining.Pool.Pool(
                    _hashAlgorithmFactory.Object,
                    _serverFactory.Object,
                    _serviceFactory.Object,
                    _daemonClient.Object,
                    _minerManager.Object,
                    null,
                    _shareManagerFactory.Object);
            });

            Assert.True(ex.Message.Contains("IJobManagerFactory"));
        }

        [Fact]
        public void ConstructorTest_NullShareManagerFactory_ShouldThrow()
        {
            Exception ex = Assert.Throws<ArgumentNullException>(() =>
            {
                var pool = new Coinium.Mining.Pool.Pool(
                    _hashAlgorithmFactory.Object,
                    _serverFactory.Object,
                    _serviceFactory.Object,
                    _daemonClient.Object,
                    _minerManager.Object,
                    _jobManagerFactory.Object,
                    null);
            });

            Assert.True(ex.Message.Contains("IShareManagerFactory"));
        }

        [Fact]
        public void InitializeTest_NonNullParams_ShouldSucceed()
        {
            #region Assemble

            var pool = new Coinium.Mining.Pool.Pool(
                _hashAlgorithmFactory.Object,
                _serverFactory.Object,
                _serviceFactory.Object,
                _daemonClient.Object,
                _minerManager.Object,
                _jobManagerFactory.Object,
                _shareManagerFactory.Object);

            Assert.NotNull(pool);
            Assert.True(pool.InstanceId > 0, "InstanceId was not initialized.");

            var config = new Mock<IPoolConfig>();
            config.SetupAllProperties();
            config.SetupGet(x => x.Daemon.Valid).Returns(true);

            _jobManagerFactory
                .Setup(x => x.Get(_daemonClient.Object, _minerManager.Object))
                .Returns(_jobManager.Object);

            _jobManager.Setup(x => x.Initialize(pool.InstanceId));

            var hashAlgorithmMock = Mock.Of<IHashAlgorithm>();

            _hashAlgorithmFactory.Setup(x => x.Get(config.Object.Coin.Algorithm)).Returns(hashAlgorithmMock);

            _shareManagerFactory.Setup(x => x.Get(hashAlgorithmMock, _jobManager.Object)).Returns(_shareManager.Object);

            _daemonClient.Setup(x => x.Initialize(config.Object.Daemon));

            _serverFactory.Setup(x => x.Get(RpcServiceNames.Stratum, _minerManager.Object)).Returns(_server.Object);
            _serviceFactory.Setup(
                x => x.Get(RpcServiceNames.Stratum, _jobManager.Object, _shareManager.Object, _daemonClient.Object)).Returns(_service.Object);

            _server.Setup(x => x.Initialize(config.Object.Stratum));

            #endregion Assemble

            #region Act

            pool.Initialize(config.Object);

            #endregion Act

            #region Assert

            // No assertions because affected properties are private
            // TODO: make assertions possible on Pool

            #endregion Assert
        }
    }
}
