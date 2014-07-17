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

using System.Collections;
using System.Collections.Generic;
using JsonConfig;

namespace CoiniumServ.Mining.Pools.Config
{
    public class RewardsConfig:IRewardsConfig
    {
        public bool Valid { get; private set; }
        private readonly Dictionary<string, float> _rewards;

        public RewardsConfig(dynamic config)
        {
            _rewards = new Dictionary<string, float>();

            // weird stuff going below because of JsonConfig libraries handling of dictionaries.            
            foreach (ConfigObject kvp in config)
                foreach (KeyValuePair<string, object> pair in kvp)
                    _rewards.Add(pair.Key, float.Parse(pair.Value.ToString()));   

            Valid = true;
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
