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
using System.Globalization;
using CoiniumServ.Payments;
using CoiniumServ.Persistance.Blocks;
using CoiniumServ.Shares;
using CoiniumServ.Utils.Helpers;
using CoiniumServ.Server.Mining.Stratum;

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
                var currentKey = $"{_coin}:shares:round:current";
                var miner = (IStratumMiner)share.Miner;
                _redisProvider.Client.HIncrByFloat(currentKey, miner.Username, (double)share.Difficulty);
                //_redisProvider.Client.HIncrByFloat(currentKey, share.Miner.Username, share.Difficulty);

                // increment shares stats.
                var statsKey = $"{_coin}:stats";
                _redisProvider.Client.HIncrBy(statsKey, share.IsValid ? "validShares" : "invalidShares", 1);

                // add to hashrate
                if (share.IsValid)
                {
                    var hashrateKey = $"{_coin}:hashrate";
                    var randomModifier = Convert.ToString(miner.ValidShareCount, 16).PadLeft(8, '0');
                    string modifiedUsername = miner.Username + randomModifier;
                    //var entry = $"{share.Difficulty}:{share.Miner.Username}";
                    var entry = string.Format("{0}:{1}", (double)miner.Difficulty, modifiedUsername);
                    _redisProvider.Client.ZAdd(hashrateKey, Tuple.Create((double)TimeHelpers.NowInUnixTimestamp(), entry));
                }

                //_client.EndPipe(); // execute the batch commands.
            }
            catch (Exception e)
            {
                _logger.Error("An exception occurred while committing share: {0:l}", e.Message);
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
                _logger.Error("An exception occurred while moving shares for new block: {0:l}", e.Message);
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
                _logger.Error("An exception occurred while moving shares: {0:l}", e.Message);
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
                _logger.Error("An exception occurred while deleting shares: {0:l}", e.Message);
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
                _logger.Error("An exception occurred while getting shares for current round: {0:l}", e.Message);
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
                _logger.Error("An exception occurred while getting shares for round; {0:l}", e.Message);
            }

            return shares;
        }
    }
}
