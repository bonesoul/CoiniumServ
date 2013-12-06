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
//using coinium.Net.RPC.Server;
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

            var client = new RPCClient("http://127.0.0.1:13000", "devel", "develpass");

            Thread thread = new Thread(new ThreadStart(serverthread));
            thread.Start();

            client.GetInfo();
            //client.GetAccount("AeZmUGwAnZgn785oYTm7K9BqwhW52kVa6");


            //client.GetInfo();


            Console.ReadLine();
        }

        private static void serverthread()
        {
            TcpListener server = new TcpListener(IPAddress.Parse("127.0.0.1"), 13000);
            server.Start();

            while (true)
            {
                try
                {
                    Console.WriteLine("Waiting for a connection... ");

                    using (TcpClient client = server.AcceptTcpClient())
                    {
                        Console.WriteLine("Connected with " + client.Client.RemoteEndPoint);

                        var rpcResultHandler = new AsyncCallback(
                            _ =>
                                {
                                    var async = ((JsonRpcStateAsync) _);
                                    var result = async.Result;
                                    var writer = ((StreamWriter) async.AsyncState);
                                    
                                    writer.WriteLine(result);
                                    writer.FlushAsync();
                                    Log.Verbose(result);
                                });


                        using (NetworkStream stream = client.GetStream())
                        {
                            var reader = new StreamReader(stream, Encoding.UTF8);
                            var writer = new StreamWriter(stream, Encoding.UTF8);

                            for (string line = reader.ReadLine(); !string.IsNullOrEmpty(line); line = reader.ReadLine())
                            {
                                Log.Verbose(line);

                                var async = new JsonRpcStateAsync(rpcResultHandler, writer);
                                async.JsonRpc = line;
                                JsonRpcProcessor.Process(async,writer);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e, "test");
                }
            }
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
