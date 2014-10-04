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

using AustinHarris.JsonRpc;
using CoiniumServ.Pools;
using CoiniumServ.Server.Mining.Stratum;
using CoiniumServ.Shares;
using NSubstitute;
using Should.Fluent;
using Xunit;

namespace CoiniumServ.Tests.Server.Mining.Stratum
{
    public class StratumServiceTests
    {        
        private readonly IShareManager _shareManager;
        private readonly IPoolConfig _poolConfig;
        private readonly StratumContext _stratumContext;

        public StratumServiceTests()
        {
            _shareManager = Substitute.For<IShareManager>();
            _poolConfig = Substitute.For<IPoolConfig>();

            var miner = Substitute.For<IStratumMiner>();
            _stratumContext = Substitute.For<StratumContext>(miner);            
        }

        [Fact]
        public void MiningSubscribe_WithOutParameters_ShouldEqual()
        {
            _poolConfig.Coin.Name.Returns("zero-params");
            var service = new StratumService(_poolConfig, _shareManager);

            const string request = @"{ 'id' : 1, 'method' : 'mining.subscribe', 'params' : [] }";
            const string expectedResult = "{\"jsonrpc\":\"2.0\",\"result\":[[\"mining.set_difficulty\",\"0\",\"mining.notify\",\"0\"],\"00000000\",4],\"id\":1}";

            var task = JsonRpcProcessor.Process(_poolConfig.Coin.Name, request,_stratumContext);
            task.Wait();

            task.Result.Should().Equal(expectedResult);
        }

        [Fact]
        public void MiningSubscribe_WithSignature_ShouldEqual()
        {
            _poolConfig.Coin.Name.Returns("signature");
            var service = new StratumService(_poolConfig, _shareManager);

            const string request = @"{ 'id' : 1, 'method' : 'mining.subscribe', 'params' : [ 'cgminer/3.7.2' ] }";
            const string expectedResult = "{\"jsonrpc\":\"2.0\",\"result\":[[\"mining.set_difficulty\",\"0\",\"mining.notify\",\"0\"],\"00000000\",4],\"id\":1}";

            var task = JsonRpcProcessor.Process(_poolConfig.Coin.Name, request, _stratumContext);
            task.Wait();

            task.Result.Should().Equal(expectedResult);
        }

        [Fact]
        public void MiningSubscribe_WithSessionId_ShouldEqual()
        {
            _poolConfig.Coin.Name.Returns("session");
            var service = new StratumService(_poolConfig, _shareManager);

            const string request = @"{ 'id' : 1, 'method' : 'mining.subscribe', 'params' : [ 'cgminer/3.7.2', '02000000b507a8fd1ea2b7d9cdec867086f6935228aba1540154f83930377ea5a2e37108' ] }";
            const string expectedResult = "{\"jsonrpc\":\"2.0\",\"result\":[[\"mining.set_difficulty\",\"0\",\"mining.notify\",\"0\"],\"00000000\",4],\"id\":1}";

            var task = JsonRpcProcessor.Process(_poolConfig.Coin.Name, request, _stratumContext);
            task.Wait();

            task.Result.Should().Equal(expectedResult);
        }
    }
}
