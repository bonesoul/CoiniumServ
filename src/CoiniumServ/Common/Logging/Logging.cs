#region License
// 
//     CoiniumServ - crypto currency pool software - https://github.com/CoiniumServ/CoiniumServ
//     Copyright (C) 2013 - 2014, Coinium Project - http://www.coinium.org
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
using System.IO;
using Microsoft.CSharp.RuntimeBinder;
using Serilog;
using Serilog.Events;

namespace Coinium.Common.Logging
{
    /// <summary>
    /// Controls the logging facilities.
    /// </summary>
    public static class Logging
    {
        public static string RootFolder { get; private set; }

        public static void Init(dynamic globalConfig)
        {
            try
            {
                // read the root folder for logs.
                RootFolder = !string.IsNullOrEmpty(globalConfig.logs.root) ? globalConfig.logs.root : "logs";

                if (!Directory.Exists(RootFolder)) // make sure log root exists.
                    Directory.CreateDirectory(RootFolder);

                // create the global logger.
                var loggerConfig = new LoggerConfiguration();

                // read log targets.
                var targets = globalConfig.logs.targets;
                foreach (dynamic target in targets)
                {
                    var enabled = target.enabled;
                    if (!enabled)
                        continue;

                    switch ((string) target.type)
                    {
                        case "console":
                            AddConsoleLog(loggerConfig, target);
                            break;
                        case "file":
                            AddLogFile(loggerConfig, target);
                            break;
                    }
                }

                // lower the default minimum level to verbose as sinks can only rise them but not lower.
                loggerConfig.MinimumLevel.Verbose();

                // bind the config to global log.
                Log.Logger = loggerConfig.CreateLogger();
            }
            catch (RuntimeBinderException e)
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine("Couldn't read log targets in config.json! [{0}]", e);
                System.Console.ResetColor();
                Environment.Exit(-1);
            }
        }

        private static void AddConsoleLog(LoggerConfiguration configuration, dynamic target)
        {
            LogEventLevel level = GetLogLevel(target.level);
            configuration.WriteTo.ColoredConsole(level, "{Timestamp:HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}");            
        }

        private static void AddLogFile(LoggerConfiguration configuration, dynamic target)
        {
            LogEventLevel level = GetLogLevel(target.level);
            bool rolling = target.rolling;
            var fileName = target.filename;
            string filePath = String.Format("{0}/{1}", RootFolder, fileName);

            if (rolling)
                configuration.WriteTo.RollingFile(filePath, level);
            else
                configuration.WriteTo.File(filePath, level);
        }

        private static LogEventLevel GetLogLevel(dynamic level)
        {
            switch ((string)level)
            {
                case "verbose":
                    return LogEventLevel.Verbose;
                case "debug":
                    return LogEventLevel.Debug;
                case "information":
                    return LogEventLevel.Information;
                case "warning":
                    return LogEventLevel.Warning;
                case "error":
                    return LogEventLevel.Error;
                case "fatal":
                    return LogEventLevel.Fatal;
                default:
                    return LogEventLevel.Verbose;
            }
        }
    }
}
