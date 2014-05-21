/*
 *   Coinium - Crypto Currency Pool Software - https://github.com/CoiniumServ/CoiniumServ
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
using Coinium.Common.Console;
using Coinium.Common.Platform;
using Coinium.Core.Coin;
using Coinium.Core.Coin.Daemon;
using Coinium.Core.Commands;
using Coinium.Core.Config;
using Coinium.Core.Mining;
using Coinium.Core.Mining.Pool;
using Coinium.Core.Server;
using Coinium.Core.Server.Stratum;
using Coinium.Net.Server;
using Ninject;
using Ninject.Parameters;
using Serilog;

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
            // Setup ninject
            var kernel = new StandardKernel();
            new Bootstrapper(kernel).Run();

            #if !DEBUG
                AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler; // Catch any unhandled exceptions.
            #endif

            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture; // Use invariant culture - we have to set it explicitly for every thread we create to prevent any file-reading problems (mostly because of number formats).

            ConsoleWindow.PrintBanner();
            ConsoleWindow.PrintLicense();

            InitLogging();
            Log.Information("Coinium {0} warming-up..", Assembly.GetAssembly(typeof(Program)).GetName().Version);
            Log.Information(string.Format("Running over {0} {1}.", PlatformManager.Framework, PlatformManager.FrameworkVersion));

            // start wallet manager.
            DaemonManager.Instance.Run();

            // start pool manager.
            PoolManager.Instance.Run();

            // stratum server.
            var ip = new ConstructorArgument("bindIp", "0.0.0.0");
            var port = new ConstructorArgument("port", 3333);
            var stratumServer = kernel.Get<IServer>(ip, port);

            stratumServer.Start();

            var miningManager = kernel.Get<IMiningManager>();

            // getwork server.
            //var getworkServer = new VanillaServer(8332);
            //getworkServer.Start();

            // Start the server manager.
            ServerManager.Instance.Start();

            while (true) // idle loop & command parser
            {
                var line = Console.ReadLine();
                CommandManager.Parse(line);
            }
        }

        #region logging facility

        private static void InitLogging()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.ColoredConsole()
                .WriteTo.RollingFile(@"logs\Debug.log")
                .MinimumLevel.Verbose()
                .CreateLogger();
        }

        #endregion

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

            Console.WriteLine(
                    e.IsTerminating ? "Terminating because of unhandled exception: {0}" : "Caught unhandled exception: {0}",
                    exception);

            Console.ReadLine();
        }

        #endregion
    }
}
