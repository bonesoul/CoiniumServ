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
using CoiniumServ.Vardiff;
using Serilog;

namespace CoiniumServ.Server.Mining.Stratum.Config
{
    public class StratumServerConfig:IStratumServerConfig
    {
        public bool Valid { get; private set; }

        public bool Enabled { get; private set; }

        public string BindInterface { get; private set; }

        public Int32 Port { get; private set; }

        public Int32 Diff { get; private set; }
        public IVardiffConfig Vardiff { get; private set; }

        public StratumServerConfig(dynamic config)
        {
            try
            {
                // set the defaults;
                BindInterface = "0.0.0.0";

                // load the config data.
                Enabled = config.enabled;
                BindInterface = config.bind;
                Port = config.port;
                Diff = config.diff;
                Vardiff = new VardiffConfig(config.vardiff);

                Valid = true;
            }
            catch (Exception e)
            {
                Valid = false;
                Log.Logger.ForContext<StratumServerConfig>().Error(e, "Error loading stratum server configuration");
            }
        }
    }
}
