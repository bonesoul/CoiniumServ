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
using Serilog;

namespace CoiniumServ.Mining.Software
{
    public class MiningSoftwareConfig:IMiningSoftwareConfig
    {
        public string Name { get; private set; }
        public string Version { get; private set; }
        public IList<string> Platforms { get; private set; }
        public IList<string> Algorithms { get; private set; }
        public string Site { get; private set; }
        public IDictionary<string, string> Downloads { get; private set; }

        public bool Valid { get; private set; }

        public MiningSoftwareConfig(dynamic config)
        {
            try
            {
                Name = config.name;
                Version = config.version;
                Platforms = config.platforms;
                Algorithms = config.algorithms;
                Site = config.site;
                Downloads = new Dictionary<string, string>
                {
                    {"windows", config.download.windows},
                    {"linux", config.download.linux},
                    {"macos", config.download.macos},
                };

                Valid = true;
            }
            catch (Exception e)
            {
                Valid = false;
                Log.Logger.ForContext<MiningSoftwareConfig>().Error(e, "Error loading software configuration");
            }
        }
    }
}
