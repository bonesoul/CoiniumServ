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
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.CSharp.RuntimeBinder;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace CoiniumServ.Utils
{
    /// <summary>
    /// Controls the logging facilities.
    /// </summary>
    public static class Logging
    {
        // TODO: change to singleton.

        public static ILogger PacketLogger { get; private set; }

        private const string ConsoleLogFormat = "{Timestamp:HH:mm:ss} [{Level}] [{Source:l}] [{Component:l}] {Message}{NewLine}{Exception}";
        private const string FileLogFormat = "{Timestamp} [{Level}] [{Source:l}] [{Component:l}] {Message}{NewLine}{Exception}";

        private static string _rootFolder;

        public static void Init(dynamic config)
        {
            try
            {
                // read the root folder for logs.
                _rootFolder = !string.IsNullOrEmpty(config.logs.root) ? config.logs.root : "logs";

                if (!Directory.Exists(_rootFolder)) // make sure log root exists.
                    Directory.CreateDirectory(_rootFolder);

                // create the global logger.
                var globalConfig = new LoggerConfiguration();
                var packetLoggerConfig = new LoggerConfiguration();
                
                // add enrichers
                globalConfig.Enrich.With(new SourceEnricher());
                globalConfig.Enrich.With(new ComponentEnricher());

                packetLoggerConfig.Enrich.With(new SourceEnricher());
                packetLoggerConfig.Enrich.With(new ComponentEnricher());

                // read log targets.
                var targets = config.logs.targets;
                foreach (dynamic target in targets)
                {
                    var enabled = target.enabled;

                    if (!enabled)
                        continue;

                    switch ((string) target.type)
                    {
                        case "console":
                            AddConsoleLog(globalConfig, target);
                            break;
                        case "file":
                            AddLogFile(globalConfig, target);
                            break;
                        case "packet":
                            AddPacketLog(packetLoggerConfig, target);
                            break;
                    }
                }

                // lower the default minimum level to verbose as sinks can only rise them but not lower.
                globalConfig.MinimumLevel.Verbose();
                packetLoggerConfig.MinimumLevel.Verbose();

                Log.Logger = globalConfig.CreateLogger(); // bind the config to global log.
                PacketLogger = packetLoggerConfig.CreateLogger();
            }
            catch (RuntimeBinderException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Couldn't read log targets in config.json! [{0}]", e);
                Console.ResetColor();
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

            string filePath = String.Format("{0}/{1}", _rootFolder, fileName);

            if (rolling)
                config.WriteTo.RollingFile(filePath, level, FileLogFormat);
            else
                config.WriteTo.File(filePath, level, FileLogFormat);
        }

        private static void AddPacketLog(LoggerConfiguration config, dynamic target)
        {
            LogEventLevel level = GetLogLevel(target.level);

            bool rolling = target.rolling;
            var fileName = target.filename;            

            string filePath = String.Format("{0}/{1}", _rootFolder, fileName);

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

    public class SourceEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddPropertyIfAbsent(logEvent.Properties.Keys.Contains("SourceContext")
                ? propertyFactory.CreateProperty("Source", logEvent.Properties["SourceContext"].ToString().Replace("\"","").Split('.').Last())
                : propertyFactory.CreateProperty("Source", "n/a"));
        }
    }

    public class ComponentEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("Component", "global"));
        }
    }
}
