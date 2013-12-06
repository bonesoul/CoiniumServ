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
using AustinHarris.JsonRpc;
using Serilog;
using coinium.Net.RPC;
using coinium.Net.RPC.Server;
using coinium.Utility;

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

            var client = new RPCClient("http://127.0.0.1:9332", "devel", "develpass");

            //client.GetInfo();
            //client.GetAccount("AeZmUGwAnZgn785oYTm7K9BqwhW52kVa6");

            client.GetInfo();

            var rpcResultHandler = new AsyncCallback(
                _ => 
                    Log.Verbose(((JsonRpcStateAsync)_).Result));

            for (string line = Console.ReadLine(); !string.IsNullOrEmpty(line); line = Console.ReadLine())
            {
                var async = new JsonRpcStateAsync(rpcResultHandler, null);
                async.JsonRpc = line;
                JsonRpcProcessor.Process(async);
            }

            Console.ReadLine();
        }

        private static object[] services = new object[]
            {
                new RPCServer.ExampleCalculatorService()
            };

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
