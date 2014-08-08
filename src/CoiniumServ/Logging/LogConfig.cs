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
using System.Dynamic;
using Serilog;

namespace CoiniumServ.Logging
{
    public class LogConfig : ILogConfig
    {
        public string Root { get; private set; }
        public List<ILogTarget> Targets { get; private set; }
        public bool Valid { get; private set; }

        public LogConfig(dynamic config)
        {
            try
            {
                Root = string.IsNullOrEmpty(config.root) ? "logs" : config.root;

                Targets = new List<ILogTarget>();

                if (config.targets is JsonConfig.NullExceptionPreventer)
                    AddDefaults(); // if we don't have any targets defined, setup a few default ones.
                else
                {
                    foreach (var target in config.targets)
                    {
                        Targets.Add(new LogTarget(target));
                    }
                }

                Valid = true;
            }
            catch (Exception e)
            {
                Valid = false;
                Log.Logger.ForContext<LogConfig>().Error(e, "Error loading logging configuration");
            }
        }

        private void AddDefaults()
        {
            dynamic consoleLog = new ExpandoObject();
            consoleLog.enabled = true;
            consoleLog.type = "console";
            consoleLog.level = "debug";

            dynamic serverLog = new ExpandoObject();
            serverLog.enabled = true;
            serverLog.type = "file";
            serverLog.filename = "server.log";
            serverLog.level = "information";
            serverLog.rolling = false;

            Targets.Add(new LogTarget(consoleLog));
            Targets.Add(new LogTarget(serverLog));
        }
    }
}
