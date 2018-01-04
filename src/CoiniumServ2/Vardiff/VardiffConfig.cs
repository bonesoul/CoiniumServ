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
using Serilog;

namespace CoiniumServ.Vardiff
{
    public class VardiffConfig:IVardiffConfig
    {
        public bool Enabled { get; private set; }
        public float MinimumDifficulty { get; private set; }
        public float MaximumDifficulty { get; private set; }
        public int TargetTime { get; private set; }
        public int RetargetTime { get; private set; }
        public int VariancePercent { get; private set; }
        public bool Valid { get; private set; }

        public VardiffConfig(dynamic config)
        {
            try
            {
                // load the config data.
                Enabled = config.enabled;
                MinimumDifficulty = config.minDiff == 0 ? 8: (float)config.minDiff;
                MaximumDifficulty = config.maxDiff == 0 ? 512 : (float)config.maxDiff;
                TargetTime = config.targetTime == 0 ? 15 : config.targetTime;
                RetargetTime = config.retargetTime == 0 ? 90 : config.retargetTime;
                VariancePercent = config.variancePercent == 0 ? 30 : config.variancePercent;

                Valid = true;
            }
            catch (Exception e)
            {
                Valid = false;
                Log.Logger.ForContext<VardiffConfig>().Error(e, "Error loading vardiff configuration");
            }   
        }
    }
}
