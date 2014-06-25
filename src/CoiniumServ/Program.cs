/*
 *   CoiniumServ - crypto currency pool software - https://github.com/CoiniumServ/CoiniumServ
 *   Copyright (C) 2013 - 2014, Coinium Project - http://www.coinium.org
 *
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using Coinium.Common.Commands;
using Coinium.Common.Configuration;
using Coinium.Common.Console;
using Coinium.Common.Logging;
using Coinium.Common.Platform;
using Coinium.Common.Repository;
using Coinium.Mining.Pool;
using Serilog;
using Nancy.TinyIoc;

namespace Coinium
{
    class Program
    {
        /// <summary>
        /// Used for uptime calculations.
        /// </summary>
        public static readonly DateTime StartupTime = DateTime.Now; // used for uptime calculations.

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
            new Bootstrapper(kernel).Run();

            // print intro texts.
            ConsoleWindow.PrintBanner();
            ConsoleWindow.PrintLicense();

            // check if we have a valid config file.
            var globalConfig = kernel.Resolve<IGlobalConfigFactory>().Get();
            if (globalConfig == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Couldn't read config.json! Make sure you rename config-sample.json as config.json.");
                Console.ResetColor();
                return;
            }

            // init logging facilities.
            Logging.Init(globalConfig);

            // print a version banner.
            Log.Information("Coinium {0} warming-up..", Assembly.GetAssembly(typeof(Program)).GetName().Version);
            Log.Information(string.Format("Running over {0} {1}.", PlatformManager.Framework, PlatformManager.FrameworkVersion));

            // start pool manager.
            var poolManager = kernel.Resolve<IPoolManager>();
            poolManager.Run();

            // run pools.
            foreach (var pool in poolManager.GetPools())
            {
                pool.Start();
            }

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
                Log.Fatal(exception, "Terminating program because of unhandled exception!");
                Environment.Exit(-1);
            }
            else
                Log.Error(exception, "Caught unhandled exception");
        }

        #endregion
    }
}
