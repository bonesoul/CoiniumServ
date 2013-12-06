using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AustinHarris.JsonRpc;
using System.Threading;
using System.Diagnostics;

namespace TestServer_Console
{
    class Program
    {
        static object[] services = new object[] {
           new CalculatorService()
        };

        static void Main(string[] args)
        {
            string input = "";
            do
            {
                input = PrintOptions();
                if (string.IsNullOrWhiteSpace(input))
                    Benchmark();
                else if (input.StartsWith("C", StringComparison.CurrentCultureIgnoreCase))
                    ConsoleInput();
                else
                    PrintOptions();
            } while (input != "x");
        }

        private static string PrintOptions()
        {
            Console.WriteLine("Hit Enter to run benchmark");
            Console.WriteLine("'c' to start reading console input");
            Console.WriteLine("'x' to exit");
            return Console.ReadLine();
        }

        private static void ConsoleInput()
        {
            var rpcResultHandler = new AsyncCallback(_ => Console.WriteLine(((JsonRpcStateAsync)_).Result));

            for (string line = Console.ReadLine(); !string.IsNullOrEmpty(line); line = Console.ReadLine())
            {
                var async = new JsonRpcStateAsync(rpcResultHandler, null);
                async.JsonRpc = line;
                JsonRpcProcessor.Process(async);
            }
        }

        private static volatile int ctr;
        private static void Benchmark()
        {
            Console.WriteLine("Starting benchmark");
           
            var cnt = 50;
            var iterations = 7;
            for (int iteration = 1; iteration <= iterations; iteration++)
            {
                cnt *= iteration;
                ctr = 0;
                var sw = Stopwatch.StartNew();
                AutoResetEvent are = new AutoResetEvent(false);
                var rpcResultHandler = new AsyncCallback(_ => 
                    {
                        if(Interlocked.Increment(ref ctr) == cnt)
                        {
                            sw.Stop();
                            Console.WriteLine("processed {0} rpc in {1}ms for {2} rpc/sec",cnt,sw.ElapsedMilliseconds, (double)cnt * 1000d / sw.ElapsedMilliseconds);
                            are.Set();
                        }
                    });

                for (int i = 0; i < cnt; i++)
                {
                    var async = new JsonRpcStateAsync(rpcResultHandler, null);
                    async.JsonRpc = "{'method':'add','params':[1,2],'id':1}";
                    JsonRpcProcessor.Process(async);
                }
                are.WaitOne();
            }


            Console.WriteLine("Finished benchmark...");
        }

    }

    
}
