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
