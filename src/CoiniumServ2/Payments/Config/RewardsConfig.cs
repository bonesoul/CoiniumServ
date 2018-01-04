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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using JsonConfig;
using Serilog;

namespace CoiniumServ.Payments.Config
{
    public class RewardsConfig:IRewardsConfig
    {
        public bool Valid { get; private set; }

        /// <summary>
        /// list of addresses that gets a percentage from each mined block (ie, pool fee.)
        /// </summary>
        private readonly IDictionary<string, float> _rewards;

        public RewardsConfig(dynamic config)
        {
            try
            {
                _rewards = new Dictionary<string, float>();

                // weird stuff going below because of JsonConfig libraries handling of dictionaries.
                foreach (ConfigObject kvp in config)
                    foreach (KeyValuePair<string, object> pair in kvp)
                        _rewards.Add(pair.Key, float.Parse(pair.Value.ToString(), CultureInfo.InvariantCulture));

                Valid = true;
            }
            catch (Exception e)
            {
                Valid = false;
                Log.Logger.ForContext<RewardsConfig>().Error(e, "Error loading rewards configuration");
            }
        }

        public IEnumerator<KeyValuePair<string, float>> GetEnumerator()
        {
            return _rewards.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
