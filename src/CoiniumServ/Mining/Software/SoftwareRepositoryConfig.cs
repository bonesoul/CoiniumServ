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
