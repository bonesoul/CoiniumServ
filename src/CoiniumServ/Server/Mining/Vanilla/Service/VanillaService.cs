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
using CoiniumServ.Coin.Config;
using CoiniumServ.Daemon;
using CoiniumServ.Daemon.Responses;
using CoiniumServ.Server.Mining.Service;
using CoiniumServ.Shares;
using Serilog;

namespace CoiniumServ.Server.Mining.Vanilla.Service
{
    /// <summary>
    /// Vanilla protocol implementation.
    /// </summary>
    public class VanillaService : JsonRpcService, IRpcService
    {
        private readonly IDaemonClient _daemonClient; // TODO: remove this!

        private readonly IShareManager _shareManager;

        public VanillaService(ICoinConfig coinConfig, IShareManager shareManager, IDaemonClient daemonClient):
            base(coinConfig.Name)
        {
            _daemonClient = daemonClient;
            _shareManager = shareManager;
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
        public Getwork Getwork(string data = null)
        {
            var context = (HttpServiceContext) JsonRpcContext.Current().Value;
            var miner = (IVanillaMiner) (context.Miner);

            // TODO: fixme! instead use jobmanager and sharemanager.

            if (data == null) // if miner supplied no data
                return _daemonClient.Getwork();  // that means he just wants work.               

            var result = _daemonClient.Getwork(data); // if he supplied a data
            //TODO: fix this according https://bitcointalk.org/index.php?topic=51281.msg611897#msg611897

            if (result) // check his work.
                Log.ForContext<VanillaMiner>().Verbose("Found block!: {0}", data);

            return null;
        }
    }
}
