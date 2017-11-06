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
using CoiniumServ.Configuration;
using Serilog;

namespace CoiniumServ.Mining.Software
{
    public class SoftwareRepositoryConfig:ISoftwareRepositoryConfig
    {
        private readonly List<IMiningSoftwareConfig> _configs;

        public bool Valid { get; private set; }

        public SoftwareRepositoryConfig(IConfigFactory configFactory, dynamic config)
        {
            try
            {
                _configs = new List<IMiningSoftwareConfig>();

                if (config == null)
                {
                    Valid = false;
                    return;
                }

                foreach (var entry in config.miner)
                {
                    _configs.Add(configFactory.GetMiningSoftwareConfig(entry));
                }

                Valid = true;
            }
            catch (Exception e)
            {
                Valid = false;
                Log.Logger.ForContext<SoftwareRepositoryConfig>().Error(e, "Error loading software manager configuration");
            }
        }

        public IEnumerator<IMiningSoftwareConfig> GetEnumerator()
        {
            return _configs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
