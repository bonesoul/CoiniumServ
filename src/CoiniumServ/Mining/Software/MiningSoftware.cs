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
