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
using CoiniumServ.Algorithms;

namespace CoiniumServ.Mining.Software
{
    public class MiningSoftware :IMiningSoftware
    {
        public string Name { get; private set; }

        public Version Version { get; private set; }

        public IList<IHashAlgorithmStatistics> Algorithms { get; private set; }

        public Platforms Platforms { get; private set; }

        public string Site { get; private set; }

        public IDictionary<string, string> Downloads { get; private set; }

        public MiningSoftware(IAlgorithmManager algorithmManager, IMiningSoftwareConfig config)
        {
            Name = config.Name;

            if(config.Version!=null)
                Version = new Version(config.Version);

            Algorithms = new List<IHashAlgorithmStatistics>();
            foreach(var entry in config.Algorithms)
            {
                var algorithm = algorithmManager.Get(entry);

                if (algorithm == null)
                    continue;

                Algorithms.Add(algorithm);
            }

            foreach (var entry in config.Platforms)
            {
                switch (entry)
                {
                    case "ati":
                        Platforms = Platforms | Platforms.Ati;
                        break;
                    case "asic":
                        Platforms = Platforms | Platforms.Asic;
                        break;
                    case "cpu":
                        Platforms = Platforms | Platforms.Cpu;
                        break;
                    case "nvidia":
                        Platforms = Platforms | Platforms.Nvidia;
                        break;
                }
            }            

            Site = config.Site;
            Downloads = config.Downloads.Where(x => x.Value != null).ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
