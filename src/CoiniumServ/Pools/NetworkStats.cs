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

using CoiniumServ.Daemon;
using CoiniumServ.Daemon.Exceptions;

namespace CoiniumServ.Pools
{
    public class NetworkStats:INetworkStats
    {
        public double Difficulty { get; private set; }
        public int Round { get; private set; }
        public ulong Hashrate { get; private set; }

        private readonly IDaemonClient _daemonClient;

        public NetworkStats(IDaemonClient daemonClient)
        {
            _daemonClient = daemonClient;
        }

        public string ServiceResponse { get; private set; }

        public void Recache()
        {
            try
            {
                var miningInfo = _daemonClient.GetMiningInfo();
                Hashrate = miningInfo.NetworkHashps;
                Difficulty = miningInfo.Difficulty;
                Round = miningInfo.Blocks + 1;
            }
            catch (RpcException)
            {
                Hashrate = 0;
                Difficulty = 0;
                Round = -1;
            }
        }
    }
}
