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

using System.Net;
using CoiniumServ.Jobs.Manager;
using CoiniumServ.Mining;
using CoiniumServ.Pools;
using Serilog;

// classic server uses json-rpc 1.0 (over http) & json-rpc.net (http://jsonrpc2.codeplex.com/)

namespace CoiniumServ.Server.Mining.Getwork
{
    public class GetworkServer : HttpServer, IMiningServer
    {
        public IServerConfig Config { get; private set; }

        private readonly IMinerManager _minerManager;

        private readonly IPool _pool;

        private readonly ILogger _logger;

        public GetworkServer(IPoolConfig poolConfig, IPool pool, IMinerManager minerManager, IJobManager jobManager)
        {
            _pool = pool;
            _minerManager = minerManager;
            _logger = Log.ForContext<GetworkServer>().ForContext("Component", poolConfig.Coin.Name);
        }

        public void Initialize(IServerConfig config)
        {
            Config = config;
            BindInterface = config.BindInterface;
            Port = config.Port;

            Initialize();
            ProcessRequest += ProcessHttpRequest;

            _logger.Information("Getwork server listening on {0:l}:{1}", BindInterface, Port);
        }

        private void ProcessHttpRequest(HttpListenerContext context)
        {
            var miner = _minerManager.Create<GetworkMiner>(_pool);
            miner.Parse(context);                        
        }
    }
}
