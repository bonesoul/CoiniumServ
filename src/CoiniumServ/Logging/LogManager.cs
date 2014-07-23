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

using System.IO;
using System.Linq;
using CoiniumServ.Configuration;
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

        private readonly ILogConfig _config;

        public LogManager(IConfigManager configManager)
        {
            _config = configManager.LogConfig;
        }

        public void Initialize()
        {
            // read the root folder for logs.
            _rootFolder = !string.IsNullOrEmpty(_config.Root) ? _config.Root : "logs";

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

            foreach (var target in _config.Targets)
            {
                switch (target.Type)
                {
                    case LogTargetType.Console:
                        CreateConsoleLog(globalConfig, target);
                        break;
                    case LogTargetType.File:
                        CreateFileLog(globalConfig, target);
                        break;
                    case LogTargetType.Packet:
                        CreatePacketLog(packetLoggerConfig, target);
                        break;
                }
            }

            // lower the default minimum level to verbose as sinks can only rise them but not lower.
            globalConfig.MinimumLevel.Verbose();
            packetLoggerConfig.MinimumLevel.Verbose();

            Log.Logger = globalConfig.CreateLogger(); // bind the config to global log.
            PacketLogger = packetLoggerConfig.CreateLogger();
        }

        private void CreateConsoleLog(LoggerConfiguration config, ILogTarget target)
        {
            config.WriteTo.ColoredConsole(target.Level, ConsoleLogFormat);
        }

        private void CreateFileLog(LoggerConfiguration config, ILogTarget target)
        {
            string filePath = string.Format("{0}/{1}", _rootFolder, target.Filename);

            if (target.Rolling)
                config.WriteTo.RollingFile(filePath, target.Level, FileLogFormat);
            else
                config.WriteTo.File(filePath, target.Level, FileLogFormat);
        }

        private void CreatePacketLog(LoggerConfiguration config, ILogTarget target)
        {
            string filePath = string.Format("{0}/{1}", _rootFolder, target.Filename);

            if (target.Rolling)
                config.WriteTo.RollingFile(filePath, target.Level, FileLogFormat);
            else
                config.WriteTo.File(filePath, target.Level, FileLogFormat);
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
