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
using CoiniumServ.Daemon;
using CoiniumServ.Daemon.Responses;
using CoiniumServ.Factories;
using CoiniumServ.Pools;
using CoiniumServ.Pools.Config;
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
        private readonly IPoolConfig _config;

        /// <summary>
        /// Initialize mock objects.
        /// </summary>
        public PoolTests()
        {
            // factory mockup.
            _objectFactory = Substitute.For<IObjectFactory>();

            // pool-config mockup.
            _config = Substitute.For<IPoolConfig>();
            _config.Daemon.Valid.Returns(true);

            // daemon client mockup.
            _daemonClient = _objectFactory.GetDaemonClient(_config.Coin.Name, _config.Daemon);
            _daemonClient.GetInfo().Returns(new Info());
            _daemonClient.GetMiningInfo().Returns(new MiningInfo());
        }


        /// <summary>
        /// Tests pool constructor with all valid parameters. Should succeed.
        /// </summary>
        [Fact]
        public void ConstructorTest_NonNullParams_ShouldSuccess()
        {
            var pool = new Pool(_config, _objectFactory); // create the pool instance.

            pool.Should().Not.Be.Null();
            pool.InstanceId.Should().Be.GreaterThan((UInt32)0); // pool should be already created an instance id.
        }

        /// <summary>
        /// Tests pool constructor with null paramets and expects exceptions being thronw.
        /// </summary>
        [Fact]
        public void ConstructorTest_NullParams_ShouldThrowException()
        {
            var configException = Assert.Throws<ArgumentNullException>(() =>
            {
                var pool = new Pool(null, _objectFactory);
            });

            configException.Message.Should().Contain("poolConfig");

            var factoryException = Assert.Throws<ArgumentNullException>(() =>
            {
                var pool = new Pool(_config, null);
            });

            factoryException.Message.Should().Contain("objectFactory");
        }
    }
}
