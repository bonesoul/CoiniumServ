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
using CoiniumServ.Daemon;
using CoiniumServ.Pools;
using CoiniumServ.Server.Mining.Service;
using CoiniumServ.Shares;
using Serilog;

namespace CoiniumServ.Server.Mining.Getwork
{
    /// <summary>
    /// Getwork protocol implementation.
    /// </summary>
    public class GetworkService : JsonRpcService, IRpcService
    {
        private readonly IDaemonClient _daemonClient; // TODO: remove this!

        public GetworkService(IPoolConfig poolConfig, IShareManager shareManager, IDaemonClient daemonClient):
            base(poolConfig.Coin.Name)
        {
            _daemonClient = daemonClient;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <remarks>Documentation:
        /// https://en.bitcoin.it/wiki/Getwork
        /// https://github.com/sinisterchipmunk/bitpool/wiki/Bitcoin-Mining-Pool-Developer's-Reference
        /// https://bitcointalk.org/index.php?topic=51281.0
        /// </remarks>
        [JsonRpcMethod("getwork")]
        public Daemon.Responses.Getwork Getwork(string data = null)
        {
            // TODO: fixme! instead use jobmanager and sharemanager.

            if (data == null) // if miner supplied no data
                return _daemonClient.Getwork();  // that means he just wants work.               

            var result = _daemonClient.Getwork(data); // if he supplied a data
            //TODO: fix this according https://bitcointalk.org/index.php?topic=51281.msg611897#msg611897

            if (result) // check his work.
                Log.ForContext<GetworkMiner>().Verbose("Found block!: {0}", data);

            return null;
        }
    }
}
