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
using CoiniumServ.Utils.Helpers;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace CoiniumServ.Logging
{
    public class LogManager:ILogManager
    {
        public static ILogger PacketLogger { get; private set; }

        private const string ConsoleLogFormat = "{Timestamp:HH:mm:ss} [{Level}] [{Source:l}] [{Component:l}] {Message}{NewLine}{Exception}";
        private const string FileLogFormat = "{Timestamp} [{Level}] [{Source:l}] [{Component:l}] {Message}{NewLine}{Exception}";

        private string _rootFolder;

        private LoggerConfiguration _mainConfig; // used by console and file logs.
        private LoggerConfiguration _packetLoggerConfig; // used by the packet log.

        public LogManager()
        {
            Initialize();
        }

        private void Initialize()
        {
            _mainConfig = new LoggerConfiguration(); // set the main config for console and file logs.
            SetupConfiguration(_mainConfig); // setup the configuration with enrichers and so.

            // create the default console.
#if DEBUG
            _mainConfig.WriteTo.ColoredConsole(LogEventLevel.Verbose, ConsoleLogFormat); // use debug level for debug mode.
#else
            _mainConfig.WriteTo.ColoredConsole(LogEventLevel.Information, ConsoleLogFormat); // use information level for release mode.
#endif
            // bind the config to global log.
            Log.Logger = _mainConfig.CreateLogger();

            // set the packet log configuration.
            _packetLoggerConfig = new LoggerConfiguration(); // will be used for packet logger.
            SetupConfiguration(_packetLoggerConfig); // setup the configuration with enrichers and so.
        }

        private void SetupConfiguration(LoggerConfiguration configuration)
        {
            configuration.Enrich.With(new SourceEnricher()); // used for enriching logs with class names.
            configuration.Enrich.With(new ComponentEnricher()); // used for enriching logs with pool names.
            configuration.MinimumLevel.Verbose(); // lower the default minimum level to verbose as sinks can only rise them but not lower.
        }

        public void EmitConfiguration(ILogConfig config)
        {
            // read the root folder for logs.
            _rootFolder = string.IsNullOrEmpty(config.Root) ? "logs" : config.Root;
            var rootPath = FileHelpers.GetAbsolutePath(_rootFolder);

            // make sure log root exists.
            if (!Directory.Exists(rootPath))
                Directory.CreateDirectory(rootPath);  
    
            // loop through file log targets from the config
            foreach (var target in config.Targets)
            {
                if (!target.Enabled)
                    continue;

                switch (target.Type)
                {
                    case LogTargetType.File:
                        CreateFileLog(_mainConfig, target);
                        break;
                    case LogTargetType.Packet:
                        CreatePacketLog(_packetLoggerConfig, target);
                        break;
                }
            }

            if (config.Targets.Count(x => x.Type == LogTargetType.File) > 0) // if we have added new file loggers.
                Log.Logger = _mainConfig.CreateLogger(); // recreate the global logger.

            PacketLogger = _packetLoggerConfig.CreateLogger(); // create the packet logger too even if doesn't really contain any outputs.
        }

        private void CreateFileLog(LoggerConfiguration config, ILogTarget target)
        {
            try
            {
                var filePath = FileHelpers.GetAbsolutePath(string.Format("{0}/{1}", _rootFolder, target.Filename));

                if (target.Rolling)
                    config.WriteTo.RollingFile(filePath, target.Level, FileLogFormat);
                else
                    config.WriteTo.File(filePath, target.Level, FileLogFormat);
            }
            catch (UnauthorizedAccessException e)
            {
                Log.ForContext<LogManager>().Error("Error creating file log {0:l} - {1:l}", target.Filename, e.Message);
            }
        }

        private void CreatePacketLog(LoggerConfiguration config, ILogTarget target)
        {
            try
            {
                var filePath = FileHelpers.GetAbsolutePath(string.Format("{0}/{1}", _rootFolder, target.Filename));

                if (target.Rolling)
                    config.WriteTo.RollingFile(filePath, target.Level, FileLogFormat);
                else
                    config.WriteTo.File(filePath, target.Level, FileLogFormat);
            }
            catch (UnauthorizedAccessException e)
            {
                Log.ForContext<LogManager>().Error("Error creating file log {0:l} - {1:l}", target.Filename, e.Message);
            }
        }
    }

    public class SourceEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddPropertyIfAbsent(logEvent.Properties.Keys.Contains("SourceContext")
                ? propertyFactory.CreateProperty("Source", logEvent.Properties["SourceContext"].ToString().Replace("\"", "").Split('.').Last())
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
