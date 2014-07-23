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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using CoiniumServ.Miners;
using CoiniumServ.Networking.Server.Sockets;
using CoiniumServ.Server.Mining.Vanilla;
using CoiniumServ.Shares;
using CoiniumServ.Utils.Helpers.Time;
using Serilog;

namespace CoiniumServ.Banning
{
    public class BanManager:IBanManager
    {
        public IBanConfig Config { get; private set; }

        private readonly Dictionary<IPAddress, int> _bannedIps;

        private readonly Timer _timer;

        private readonly ILogger _logger;

        public BanManager(string pool, IShareManager shareManager, IBanConfig banConfig)
        {            
            Config = banConfig;

            if (!Config.Enabled)
                return;

            _logger = Log.ForContext<BanManager>().ForContext("Component", pool);
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

            var totalShares = miner.ValidShares + miner.InvalidShares;

            if (totalShares < Config.CheckThreshold) // check if we exceeded the threshold for checks.
                return;

            var invalidPercentage = miner.InvalidShares/totalShares*100;

            if (invalidPercentage < Config.InvalidPercent)
                // if the miner didn't reach the invalid share percentage, reset his stats.
            {
                miner.ValidShares = 0;
                miner.InvalidShares = 0;
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
            if (miner is IVanillaMiner) // as vanilla miners doesn't use persistent connections, we don't need to disconect him
                return; // but just blacklist his ip.

            var client = (IClient) miner;
            var ip = client.Connection.RemoteEndPoint.Address;

            if(!_bannedIps.ContainsKey(ip))
                _bannedIps.Add(ip, TimeHelpers.NowInUnixTime());

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

            var elapsedTime = TimeHelpers.NowInUnixTime() - banTime; // elapsed time since his ban.
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
