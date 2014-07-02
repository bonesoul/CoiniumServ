#region License
// 
//     CoiniumServ - Crypto Currency Mining Pool Server Software
//     Copyright (C) 2013 - 2014, CoiniumServ Project - http://www.coinium.org
//     https://github.com/CoiniumServ/CoiniumServ
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
using System.Linq;
using Microsoft.CSharp.RuntimeBinder;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Coinium.Utils.Logging
{
    /// <summary>
    /// Controls the logging facilities.
    /// </summary>
    public static class Logging
    {
        // TODO: change to singleton.

        public const string ConsoleLogFormat = "{Timestamp:HH:mm:ss} [{Level}] [{Component:l}] [{Pool:l}] {Message}{NewLine}{Exception}";
        public const string FileLogFormat = "{Timestamp} [{Level}] [{Component:l}] [{Pool:l}] {Message}{NewLine}{Exception}";

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
                var config = new LoggerConfiguration();

                // add enrichers
                config.Enrich.With(new ComponentEnricher());
                config.Enrich.With(new PoolEnricher());

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
                            AddConsoleLog(config, target);
                            break;
                        case "file":
                            AddLogFile(config, target);
                            break;
                    }
                }

                // lower the default minimum level to verbose as sinks can only rise them but not lower.
                config.MinimumLevel.Verbose();

                // bind the config to global log.
                Log.Logger = config.CreateLogger();
            }
            catch (RuntimeBinderException e)
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine("Couldn't read log targets in config.json! [{0}]", e);
                System.Console.ResetColor();
                Environment.Exit(-1);
            }
        }

        private static void AddConsoleLog(LoggerConfiguration config, dynamic target)
        {
            LogEventLevel level = GetLogLevel(target.level);

            config.WriteTo.ColoredConsole(level, ConsoleLogFormat);        
        }

        private static void AddLogFile(LoggerConfiguration config, dynamic target)
        {
            LogEventLevel level = GetLogLevel(target.level);
            bool rolling = target.rolling;
            var fileName = target.filename;
            string filePath = String.Format("{0}/{1}", RootFolder, fileName);

            if (rolling)
                config.WriteTo.RollingFile(filePath, level, FileLogFormat);
            else
                config.WriteTo.File(filePath, level, FileLogFormat);
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

    public class ComponentEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddPropertyIfAbsent(logEvent.Properties.Keys.Contains("SourceContext")
                ? propertyFactory.CreateProperty("Component", logEvent.Properties["SourceContext"].ToString().Replace("\"","").Split('.').Last())
                : propertyFactory.CreateProperty("Component", "global"));
        }
    }

    public class PoolEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("Pool", "n/a"));
        }
    }
}
