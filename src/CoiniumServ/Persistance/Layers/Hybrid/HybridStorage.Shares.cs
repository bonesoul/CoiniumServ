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
using System.Globalization;
using CoiniumServ.Payments;
using CoiniumServ.Persistance.Blocks;
using CoiniumServ.Shares;
using CoiniumServ.Utils.Helpers;

namespace CoiniumServ.Persistance.Layers.Hybrid
{
    public partial class HybridStorage
    {
        public void AddShare(IShare share)
        {
            try
            {
                if (!IsEnabled || !_redisProvider.IsConnected)
                    return;

                //_client.StartPipe(); // batch the commands.

                // add the share to round 
                var currentKey = string.Format("{0}:shares:round:current", _coin);
                _redisProvider.Client.HIncrByFloat(currentKey, share.Miner.Username, share.Difficulty);

                // increment shares stats.
                var statsKey = string.Format("{0}:stats", _coin);
                _redisProvider.Client.HIncrBy(statsKey, share.IsValid ? "validShares" : "invalidShares", 1);

                // add to hashrate
                if (share.IsValid)
                {
                    var hashrateKey = string.Format("{0}:hashrate", _coin);
                    var entry = string.Format("{0}:{1}", share.Difficulty, share.Miner.Username);
                    _redisProvider.Client.ZAdd(hashrateKey, Tuple.Create(TimeHelpers.NowInUnixTimestamp(), entry));
                }

                //_client.EndPipe(); // execute the batch commands.
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while comitting share: {0:l}", e.Message);
            }
        }

        public void MoveCurrentShares(int height)
        {
            try
            {
                if (!IsEnabled || !_redisProvider.IsConnected)
                    return;

                // rename round.
                var currentKey = string.Format("{0}:shares:round:current", _coin);
                var roundKey = string.Format("{0}:shares:round:{1}", _coin, height);
                _redisProvider.Client.Rename(currentKey, roundKey);
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while moving shares for new block: {0:l}", e.Message);
            }
        }

        public void MoveOrphanedShares(IPersistedBlock block)
        {
            try
            {
                if (!IsEnabled || !_redisProvider.IsConnected)
                    return;

                if (block.Status == BlockStatus.Confirmed || block.Status == BlockStatus.Pending)
                    return;

                var round = string.Format("{0}:shares:round:{1}", _coin, block.Height); // rounds shares key.
                var current = string.Format("{0}:shares:round:current", _coin); // current round key.

                //_client.StartPipeTransaction(); // batch the commands as atomic.

                // add shares to current round again.
                foreach (var entry in _redisProvider.Client.HGetAll(round))
                {
                    _redisProvider.Client.HIncrByFloat(current, entry.Key, double.Parse(entry.Value, CultureInfo.InvariantCulture));
                }

                _redisProvider.Client.Del(round); // delete the round shares.

                //_client.EndPipe(); // execute the batch commands.
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while moving shares: {0:l}", e.Message);
            }
        }

        public void RemoveShares(IPaymentRound round)
        {
            try
            {
                if (!IsEnabled || !_redisProvider.IsConnected)
                    return;

                var roundKey = string.Format("{0}:shares:round:{1}", _coin, round.Block.Height);

                _redisProvider.Client.Del(roundKey); // delete the associated shares.            
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while deleting shares: {0:l}", e.Message);
            }
        }

        public Dictionary<string, double> GetCurrentShares()
        {
            var shares = new Dictionary<string, double>();

            try
            {
                if (!IsEnabled || !_redisProvider.IsConnected)
                    return shares;

                var key = string.Format("{0}:shares:round:{1}", _coin, "current");
                var hashes = _redisProvider.Client.HGetAll(key);

                foreach (var hash in hashes)
                {
                    shares.Add(hash.Key, double.Parse(hash.Value, CultureInfo.InvariantCulture));
                }
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while getting shares for current round: {0:l}", e.Message);
            }

            return shares;
        }

        public Dictionary<string, double> GetShares(IPersistedBlock block)
        {
            var shares = new Dictionary<string, double>();

            try
            {
                if (!IsEnabled || !_redisProvider.IsConnected)
                    return shares;

                var key = string.Format("{0}:shares:round:{1}", _coin, block.Height);
                var hashes = _redisProvider.Client.HGetAll(key);

                foreach (var hash in hashes)
                {
                    shares.Add(hash.Key, double.Parse(hash.Value, CultureInfo.InvariantCulture));
                }
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while getting shares for round; {0:l}", e.Message);
            }

            return shares;
        }
    }
}
