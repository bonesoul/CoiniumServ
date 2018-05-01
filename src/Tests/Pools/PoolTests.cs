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

using System;
using CoiniumServ.Configuration;
using CoiniumServ.Container;
using CoiniumServ.Daemon;
using CoiniumServ.Daemon.Responses;
using CoiniumServ.Pools;
using NSubstitute;
using Should.Fluent;
using Xunit;

namespace CoiniumServ.Tests.Pools
{
    public class PoolTests
    {
        // object mocks.
        private readonly IObjectFactory _objectFactory;
        private readonly IDaemonClient _daemonClient;
        private readonly IConfigManager _configManager;
        private readonly IPoolConfig _config;

        /// <summary>
        /// Initialize mock objects.
        /// </summary>
        public PoolTests()
        {
            // factory mockup.
            _objectFactory = Substitute.For<IObjectFactory>();

            // config-manager mockup
            _configManager = Substitute.For<IConfigManager>();

            // pool-config mockup.
            _config = Substitute.For<IPoolConfig>();
            _config.Daemon.Valid.Returns(true);

            // daemon client mockup.
            _daemonClient = _objectFactory.GetDaemonClient(_config.Daemon, _config.Coin);
            _daemonClient.GetInfo().Returns(new Info());
            _daemonClient.GetBlockChainInfo().Returns(new Info());
            _daemonClient.GetNetworkInfo().Returns(new Info());
            _daemonClient.GetWalletInfo().Returns(new Info());
            _daemonClient.GetMiningInfo().Returns(new MiningInfo());
        }


        /// <summary>
        /// Tests pool constructor with all valid parameters. Should succeed.
        /// </summary>
        [Fact]
        public void ConstructorTest_NonNullParams_ShouldSuccess()
        {
            var pool = new Pool(_config,_configManager, _objectFactory); // create the pool instance.

            pool.Should().Not.Be.Null();
            //pool.InstanceId.Should().Be.GreaterThan((UInt32)0); // pool should be already created an instance id.
        }

        /// <summary>
        /// Tests pool constructor with null paramets and expects exceptions being thronw.
        /// </summary>
        [Fact]
        public void ConstructorTest_NullParams_ShouldThrowException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                new Pool(null, _configManager, _objectFactory);
            }).Message.Should().Contain("poolConfig");

            Assert.Throws<ArgumentNullException>(() =>
            {
                new Pool(_config, null, _objectFactory);
            }).Message.Should().Contain("configManager");

            Assert.Throws<ArgumentNullException>(() =>
            {
                new Pool(_config, _configManager, null);
            }).Message.Should().Contain("objectFactory");
        }
    }
}
