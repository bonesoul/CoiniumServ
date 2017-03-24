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
using CoiniumServ.Pools;
using CoiniumServ.Shares;
using CoiniumServ.Utils.Buffers;
using CoiniumServ.Utils.Helpers;
using Serilog;

namespace CoiniumServ.Vardiff
{
    public class VardiffManager:IVardiffManager
    {
        public IVardiffConfig Config { get; private set; }

        private readonly int _bufferSize;
        private readonly float _tMin;
        private readonly float _tMax;
        private readonly ILogger _logger;

        public VardiffManager(IPoolConfig poolConfig, IShareManager shareManager)
        {
            _logger = Log.ForContext<VardiffManager>().ForContext("Component", poolConfig.Coin.Name);

            Config = poolConfig.Stratum.Vardiff;

            if (!Config.Enabled)
                return;

            shareManager.ShareSubmitted += OnShare;

            var variance = Config.TargetTime * ((float)Config.VariancePercent / 100);
            _bufferSize = Config.RetargetTime / Config.TargetTime * 4;
            _tMin = Config.TargetTime - variance;
            _tMax = Config.TargetTime + variance;
        }

        private void OnShare(object sender, EventArgs e)
        {
            var shareArgs = (ShareEventArgs) e;
            var miner = shareArgs.Miner;

            if (miner == null)
                return;

            var now = TimeHelpers.NowInUnixTimestamp();

            if (miner.VardiffBuffer == null)
            {
                miner.LastVardiffRetarget = now - Config.RetargetTime / 2;
                miner.LastVardiffTimestamp = now;
                miner.VardiffBuffer = new RingBuffer(_bufferSize);
                return;
            }

            var sinceLast = now - miner.LastVardiffTimestamp; // how many seconds elapsed since last share?
            miner.VardiffBuffer.Append(sinceLast); // append it to vardiff buffer.
            miner.LastVardiffTimestamp = now;

            if (now - miner.LastVardiffRetarget < Config.RetargetTime && miner.VardiffBuffer.Size > 0) // check if we need a re-target.
                return;

            miner.LastVardiffRetarget = now;
            var average = miner.VardiffBuffer.Average;
            var deltaDiff = Config.TargetTime/average;

            if (average > _tMax && miner.Difficulty > Config.MinimumDifficulty)
            {
                if (deltaDiff*miner.Difficulty < Config.MinimumDifficulty)
                    deltaDiff = Config.MinimumDifficulty/miner.Difficulty;
            }
            else if (average < _tMin)
            {
                if (deltaDiff*miner.Difficulty > Config.MaximumDifficulty)
                    deltaDiff = Config.MaximumDifficulty/miner.Difficulty;
            }
            else
                return;

            var newDifficulty = miner.Difficulty*deltaDiff; // calculate the new difficulty.
            miner.SetDifficulty(newDifficulty); // set the new difficulty and send it.
            _logger.Debug("Difficulty updated to {0} for miner: {1:l}", miner.Difficulty, miner.Username);

            miner.VardiffBuffer.Clear();            
        }
    }
}
