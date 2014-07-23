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
using Serilog.Events;

namespace CoiniumServ.Logging
{
    public class LogTarget:ILogTarget
    {
        public bool Enabled { get; private set; }
        public LogTargetType Type { get; private set; }
        public string Filename { get; private set; }
        public bool Rolling { get; private set; }
        public LogEventLevel Level { get; private set; }
        public bool Valid { get; private set; }

        public LogTarget(dynamic config)
        {
            try
            {
                Enabled = config.enabled;
                Filename = config.filename;
                Rolling = config.rolling;

                switch ((string) config.type)
                {
                    case "console":
                        Type = LogTargetType.Console;
                        break;
                    case "file":
                        Type = LogTargetType.File;
                        break;
                    case "packet":
                        Type = LogTargetType.Packet;
                        break;
                }

                switch ((string) config.level)
                {
                    case "verbose":
                        Level = LogEventLevel.Verbose;
                        break;
                    case "debug":
                        Level = LogEventLevel.Debug;
                        break;
                    case "information":
                        Level = LogEventLevel.Information;
                        break;
                    case "warning":
                        Level = LogEventLevel.Warning;
                        break;
                    case "error":
                        Level = LogEventLevel.Error;
                        break;
                    case "fatal":
                        Level = LogEventLevel.Fatal;
                        break;
                }

                Valid = true;
            }
            catch (Exception e)
            {
                Valid = false;
                Log.Logger.ForContext<LogTarget>().Error(e, "Error loading log target configuration");
            }
        }
    }
}
