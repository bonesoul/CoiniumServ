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
using System.Globalization;
using System.Reflection;
using System.Threading;
using CoiniumServ.Factories;
using CoiniumServ.Repository;
using CoiniumServ.Utils;
using CoiniumServ.Utils.Commands;
using CoiniumServ.Utils.Platform;
using CoiniumServ.Utils.Versions;
using Nancy.TinyIoc;
using Serilog;

namespace CoiniumServ
{
    class Program
    {
        /// <summary>
        /// Used for uptime calculations.
        /// </summary>
        public static readonly DateTime StartupTime = DateTime.Now; // used for uptime calculations.

        private static ILogger _logger;

        static void Main(string[] args)
        {
            #if !DEBUG  // Catch any unhandled exceptions if we are in release mode.
                AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;
            #endif

            // use invariant culture - we have to set it explicitly for every thread we create to 
            // prevent any file-reading problems (mostly because of number formats).
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            // start the ioc kernel.
            var kernel = TinyIoCContainer.Current;
            var bootstrapper = new Bootstrapper(kernel);
            var objectFactory = kernel.Resolve<IObjectFactory>();
            var configFactory = kernel.Resolve<IConfigFactory>();

            // print intro texts.
            ConsoleWindow.PrintBanner();
            ConsoleWindow.PrintLicense();

            // check if we have a valid config file.
            var configManager = configFactory.GetConfigManager();
            if (!configManager.ConfigExists)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Couldn't read config/config.json! Make sure you rename config/config-sample.json as config/config.json.");
                Console.ResetColor();
                return;
            }

            // initialize log-manager.
            var logManager = objectFactory.GetLogManager();
            logManager.Initialize();

            // print a version banner.
            _logger = Log.ForContext<Program>();
            _logger.Information("CoiniumServ {0} {1:l} warming-up..", VersionInfo.CodeName, Assembly.GetAssembly(typeof(Program)).GetName().Version);
            PlatformManager.PrintPlatformBanner();

            // initialize config manager.
            configManager.Initialize();

            // start pool manager.
            var poolManager = objectFactory.GetPoolManager();
            poolManager.Run();

            // start web server.
            var webServer = objectFactory.GetWebServer();

            while (true) // idle loop & command parser
            {
                var line = Console.ReadLine();
                CommandManager.Parse(line);
            }
        }

        #region unhandled exception emitter

        /// <summary>
        /// Unhandled exception emitter.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;

            if (exception == null) // if we can't get the exception, whine about it.
                throw new ArgumentNullException("e");

            if (e.IsTerminating)
            {
                _logger.Fatal(exception, "Terminating program because of unhandled exception!");
                Environment.Exit(-1);
            }
            else
                _logger.Error(exception, "Caught unhandled exception");
        }

        #endregion
    }
}
