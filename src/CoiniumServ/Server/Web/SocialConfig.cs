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

namespace CoiniumServ.Server.Web
{
    public class SocialConfig:ISocialConfig
    {
        public bool Valid { get; private set; }
        public string Rss { get; private set; }
        public string Twitter { get; private set; }
        public string Facebook { get; private set; }
        public string GooglePlus { get; private set; }
        public string Youtube { get; private set; }

        public SocialConfig(dynamic config)
        {
            try
            {
                Rss = string.IsNullOrEmpty(config.rss)? string.Empty: config.rss;
                Twitter = string.IsNullOrEmpty(config.twitter) ? string.Empty : config.twitter;
                Facebook = string.IsNullOrEmpty(config.facebook) ? string.Empty : config.facebook;
                GooglePlus = string.IsNullOrEmpty(config.googleplus) ? string.Empty : config.googleplus;
                Youtube = string.IsNullOrEmpty(config.youtube) ? string.Empty : config.youtube;
                Valid = true;
            }
            catch (Exception e)
            {
                Valid = false;
                Log.Logger.ForContext<WebServerConfig>().Error(e, "Error loading social configuration");
            }
        }
    }
}
