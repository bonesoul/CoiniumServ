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
