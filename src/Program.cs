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
using Jayrock.JsonRpc;
using Serilog;
using coinium.Net.RPC;
using coinium.Utility;
using Jayrock.Services;

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

            Thread thread = new Thread(new ThreadStart(Server));
            thread.Start();

            var client = new RPCClient("http://127.0.0.1:13000", "devel", "develpass");

            //client.GetInfo();
            //client.GetAccount("AeZmUGwAnZgn785oYTm7K9BqwhW52kVa6");

            client.GetInfo();
            

            Console.ReadLine();
        }

        private class Service : JsonRpcService
        {
            [JsonRpcMethod("getinfo")]
            public string GetInfo() { return "hello!"; }

            [JsonRpcMethod("add")]
            public int Add(int a, int b) { return a + b; }

            [JsonRpcMethod("env")]
            public IDictionary GetEnvironment() { return Environment.GetEnvironmentVariables(); }
        }

        public static void Server()
        {
            TcpListener server = new TcpListener(IPAddress.Parse("127.0.0.1"), 13000);

            try
            {
                server.Start();

                while (true)
                {
                    Console.WriteLine("Waiting for a connection... ");

                    using (TcpClient client = server.AcceptTcpClient())
                    {
                        Console.WriteLine("Connected with " + client.Client.RemoteEndPoint);

                        using (NetworkStream stream = client.GetStream())
                        {
                            Service service = new Service();
                            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                            StreamWriter writer = new StreamWriter(stream, new UTF8Encoding(false));
                            JsonRpcDispatcher dispatcher = new JsonRpcDispatcher(service);
                            dispatcher.Process(reader, writer);
                            writer.Flush();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.GetBaseException().Message);
                Trace.WriteLine(e.ToString());
            }
            finally
            {
                server.Stop();
            }
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
