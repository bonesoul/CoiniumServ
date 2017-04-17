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

namespace CoiniumServ.Banning
{
    public class BanConfig : IBanConfig
    {
        public bool Enabled { get; private set; }
        public int Duration { get; private set; }
        public int InvalidPercent { get; private set; }
        public int CheckThreshold { get; private set; }
        public int PurgeInterval { get; private set; }
        public bool Valid { get; private set; }

        public BanConfig(dynamic config)
        {
            try
            {
                // load the config data.
                Enabled = config.enabled;
                Duration = config.duration == 0 ? 600 : config.duration;
                InvalidPercent = config.invalidPercent == 0 ? 50 : config.invalidPercent;
                CheckThreshold = config.checkThreshold == 0 ? 100 : config.checkThreshold;
                PurgeInterval = config.purgeInterval == 0 ? 300 : config.purgeInterval;

                Valid = true;
            }
            catch (Exception e)
            {
                Valid = false;
                Log.Logger.ForContext<BanConfig>().Error(e, "Error loading banning configuration");
            }
        }
    }
}
