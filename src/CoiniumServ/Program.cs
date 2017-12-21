#region License
// 
//     MIT License
//
//     CoiniumServ - Crypto Currency Mining Pool Server Software
//     Copyright (C) 2013 - 2017, CoiniumServ Project
//     Hüseyin Uslu, shalafiraistlin at gmail dot com
//     https://github.com/bonesoul/CoiniumServ
// 
//     Permission is hereby granted, free of charge, to any person obtaining a copy
//     of this software and associated documentation files (the "Software"), to deal
//     in the Software without restriction, including without limitation the rights
//     to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//     copies of the Software, and to permit persons to whom the Software is
//     furnished to do so, subject to the following conditions:
//     
//     The above copyright notice and this permission notice shall be included in all
//     copies or substantial portions of the Software.
//     
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//     FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//     LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//     OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//     SOFTWARE.
// 
#endregion

using System;
using System.Globalization;
using System.Threading;
using CoiniumServ.Configuration;
using CoiniumServ.Container;
using CoiniumServ.Utils;
using CoiniumServ.Utils.Commands;
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
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler; // Catch any unhandled exceptions if we are in release mode.

            // use invariant culture - we have to set it explicitly for every thread we create to 
            // prevent any file-reading problems (mostly because of number formats).
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            // start the ioc kernel.
            var kernel = TinyIoCContainer.Current;
            new Bootstrapper(kernel);
            var objectFactory = kernel.Resolve<IObjectFactory>();
            var configFactory = kernel.Resolve<IConfigFactory>();

            // print intro texts.
            ConsoleWindow.PrintBanner();
            ConsoleWindow.PrintLicense();

            // initialize the log-manager.
            objectFactory.GetLogManager();

            // load the config-manager.
            configFactory.GetConfigManager();

            // create logger to be used later.
            _logger = Log.ForContext<Program>();

            // run global managers
            RunGlobalManagers(objectFactory);

            while (true) // idle loop & command parser
            {
                var line = Console.ReadLine();
                CommandManager.Parse(line);
            }
        }

        private static void RunGlobalManagers(IObjectFactory objectFactory)
        {
            // start pool manager.
            objectFactory.GetPoolManager();

            // run algorithm manager.
            objectFactory.GetAlgorithmManager();

            // run payment manager
            objectFactory.GetPaymentDaemonManager();

            // run statistics manager.
            objectFactory.GetStatisticsManager();

            // run software repository.
            objectFactory.GetSoftwareRepository();

            // start web server.
            objectFactory.GetWebServer();
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
            {
                _logger.Error("We can't get Exception object from UnhandledExceptionEventArgs");
                throw new ArgumentNullException(nameof(e));
            }

            if (e.IsTerminating)
            {
                _logger.Fatal(exception, "Terminating because of unhandled exception!");
#if !DEBUG 
                // prevent console window from being closed when we are in development mode.
                Environment.Exit(-1);
#endif
            }
            else
                _logger.Error(exception, "Caught unhandled exception");
        }

        #endregion
    }
}
