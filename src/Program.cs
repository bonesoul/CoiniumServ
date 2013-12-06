/*
 * Coinium project, Copyright (C) 2013, Int6 Studios - All Rights Reserved. - http://www.coinium.org
 *
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using coinium.Net.RPC.Client;
using coinium.Net.RPC.Server;
using Serilog;
using coinium.Net.RPC;
using coinium.Utility;
using AustinHarris.JsonRpc;

namespace coinium
{
    class Program
    {
        static void Main(string[] args)
        {
            #if !DEBUG
                AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler; // Catch any unhandled exceptions.
            #endif

            Console.ForegroundColor = ConsoleColor.Yellow;
            ConsoleWindow.PrintBanner();
            ConsoleWindow.PrintLicense();
            Console.ResetColor();

            InitLogging();
            Log.Information("coinium-serv {0} warming-up..", Assembly.GetAssembly(typeof(Program)).GetName().Version);

            var server = new RPCServer();
            server.Start();

            var client = new RPCClient("http://127.0.0.1:9332", "devel", "develpass");
            //client.GetInfo();
            //client.GetAccount("AeZmUGwAnZgn785oYTm7K9BqwhW52kVa6");

            Console.ReadLine();
        }

        #region logging facility

        private static void InitLogging()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.ColoredConsole()
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
