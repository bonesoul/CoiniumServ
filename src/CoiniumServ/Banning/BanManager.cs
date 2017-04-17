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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using CoiniumServ.Mining;
using CoiniumServ.Pools;
using CoiniumServ.Server.Mining.Getwork;
using CoiniumServ.Server.Mining.Stratum.Sockets;
using CoiniumServ.Shares;
using CoiniumServ.Utils.Helpers;
using Serilog;

namespace CoiniumServ.Banning
{
    public class BanManager:IBanManager
    {
        public IBanConfig Config { get; private set; }

        private readonly Dictionary<IPAddress, int> _bannedIps;

        private readonly Timer _timer;

        private readonly ILogger _logger;

        public BanManager(IPoolConfig poolConfig, IShareManager shareManager)
        {            
            Config = poolConfig.Banning;

            if (!Config.Enabled)
                return;

            _logger = Log.ForContext<BanManager>().ForContext("Component", poolConfig.Coin.Name);
            _bannedIps = new Dictionary<IPAddress, int>();
            _timer = new Timer(CheckBans, null, Timeout.Infinite, Timeout.Infinite); // create the timer as disabled.

            shareManager.ShareSubmitted += OnShare;

            CheckBans(null);
        }

        private void OnShare(object sender, EventArgs e)
        {
            var shareArgs = (ShareEventArgs)e;
            var miner = shareArgs.Miner;

            if (miner == null)
                return;

            var totalShares = miner.ValidShareCount + miner.InvalidShareCount;

            if (totalShares < Config.CheckThreshold) // check if we exceeded the threshold for checks.
                return;

            var invalidPercentage = miner.InvalidShareCount/totalShares*100;

            if (invalidPercentage < Config.InvalidPercent)
                // if the miner didn't reach the invalid share percentage, reset his stats.
            {
                miner.ValidShareCount = 0;
                miner.InvalidShareCount = 0;
            }
            else // he needs a ban
            {
                _logger.Information("Banned miner {0:l} because of high percentage of invalid shares: {1}%.", miner.Username, invalidPercentage);
                Ban(miner);
            }
        }

        private void Ban(IMiner miner)
        {
            // TODO: add vanilla miners to banlist too.
            if (miner is IGetworkMiner) // as vanilla miners doesn't use persistent connections, we don't need to disconect him
                return; // but just blacklist his ip.

            var client = (IClient) miner;
            var ip = client.Connection.RemoteEndPoint.Address;

            if(!_bannedIps.ContainsKey(ip))
                _bannedIps.Add(ip, TimeHelpers.NowInUnixTimestamp());

            client.Connection.Disconnect();
        }

        public bool IsBanned(IPAddress ip)
        {
            if (!Config.Enabled) // if banning is not enabled,
                return false; // just skip queries.

            if (!_bannedIps.ContainsKey(ip)) // check if ip exists in banlist.
                return false;

            if (!BanExpired(ip)) // if ip has still an ongoing ban.
                return true;

            RemoveBan(ip); // if the code flow reaches here, it means the ban has been expired, so remove it first.
            return false; // and tell that ip has no bans.
        }

        public void CheckBans(object state)
        {
            var expiredBans = 0;
            var banlist = _bannedIps.ToDictionary(x => x.Key); // get a copy of the current banlist

            foreach (var pair in banlist)
            {
                if (!BanExpired(pair.Key)) // check if the ip should be still banned?
                    continue;

                expiredBans++;
                RemoveBan(pair.Key);
            }

            var remainingBans = _bannedIps.Count;

            if(expiredBans > 0)
                _logger.Information("Cleared {0} expired bans [remaining bans: {1}].", expiredBans, remainingBans);
            else
                _logger.Information("No expired bans found to be cleared [remaining bans: {0}].", remainingBans);

            // reset the recache timer.
            _timer.Change(Config.PurgeInterval * 1000, Timeout.Infinite);
        }

        private bool BanExpired(IPAddress ip)
        {
            var banTime = _bannedIps[ip];

            var elapsedTime = TimeHelpers.NowInUnixTimestamp() - banTime; // elapsed time since his ban.
            var timeLeft = Config.Duration - elapsedTime; // time left for his ban

            if (timeLeft > 0) // if he has still remaining time
                return false;

            return true;
        }

        private void RemoveBan(IPAddress ip)
        {
            _bannedIps.Remove(ip);
        }
    }
}
