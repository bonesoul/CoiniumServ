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

namespace CoiniumServ.Mining
{
    public class MetaConfig :IMetaConfig
    {
        public bool Valid { get; private set; }
        public string MOTD { get; private set; }
        public string TxMessage { get; private set; }

        public MetaConfig(dynamic config)
        {
            try
            {
                // load the config data.
                MOTD = string.IsNullOrEmpty(config.motd)
                    ? "Welcome to CoiniumServ pool, enjoy your stay! - http://www.coinumserv.com"
                    : config.motd;

                TxMessage = string.IsNullOrEmpty(config.txMessage) ? "http://www.coiniumserv.com/" : config.txMessage;

                Valid = true;
            }
            catch (Exception e)
            {
                Valid = false;
                Log.Logger.ForContext<MetaConfig>().Error(e, "Error loading meta configuration");
            }
        }
    }
}
