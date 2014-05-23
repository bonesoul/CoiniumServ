using Coinium.Coin.Algorithms;
using Coinium.Coin.Daemon;
using Coinium.Miner;
using Coinium.Mining.Jobs;
using Coinium.Mining.Pool.Config;
using Coinium.Mining.Share;
using Coinium.Rpc.Service;
using Coinium.Server;
using Moq;
using Should.Fluent;
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
