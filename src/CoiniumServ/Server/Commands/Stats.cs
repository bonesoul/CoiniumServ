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
using System.Diagnostics;
using System.Text;
using CoiniumServ.Utils.Commands;

namespace CoiniumServ.Server.Commands
{
    [CommandGroup("stats", "Renders statistics.\nUsage: stats [system].")]
    public class StatsCommand : CommandGroup
    {
        [Command("system", "Renders system statistics.")]
        public string Detailed(string[] @params)
        {
            var output = new StringBuilder();

            output.AppendFormat("GC Allocated Memory: {0}KB ", GC.GetTotalMemory(true) / 1024);

            if (PerformanceCounterCategory.Exists("Processor") && PerformanceCounterCategory.CounterExists("% Processor Time", "Processor"))
            {
                var processorTimeCounter = new PerformanceCounter { CategoryName = "Processor", CounterName = "% Processor Time", InstanceName = "_Total" };
                output.AppendFormat("Processor Time: {0}%", processorTimeCounter.NextValue());
            }

            if (PerformanceCounterCategory.Exists(".NET CLR LocksAndThreads"))
            {
                if (PerformanceCounterCategory.CounterExists("# of current physical Threads", ".NET CLR LocksAndThreads"))
                {
                    var physicalThreadsCounter = new PerformanceCounter { CategoryName = ".NET CLR LocksAndThreads", CounterName = "# of current physical Threads", InstanceName = Process.GetCurrentProcess().ProcessName };
                    output.AppendFormat("\nPhysical Threads: {0} ", physicalThreadsCounter.NextValue());
                }

                if (PerformanceCounterCategory.CounterExists("# of current logical Threads", ".NET CLR LocksAndThreads"))
                {
                    var logicalThreadsCounter = new PerformanceCounter { CategoryName = ".NET CLR LocksAndThreads", CounterName = "# of current logical Threads", InstanceName = Process.GetCurrentProcess().ProcessName };
                    output.AppendFormat("Logical Threads: {0} ", logicalThreadsCounter.NextValue());
                }

                if (PerformanceCounterCategory.CounterExists("Contention Rate / sec", ".NET CLR LocksAndThreads"))
                {
                    var contentionRateCounter = new PerformanceCounter { CategoryName = ".NET CLR LocksAndThreads", CounterName = "Contention Rate / sec", InstanceName = Process.GetCurrentProcess().ProcessName };
                    output.AppendFormat("Contention Rate: {0}/sec", contentionRateCounter.NextValue());
                }
            }

            if (PerformanceCounterCategory.Exists(".NET CLR Exceptions") && PerformanceCounterCategory.CounterExists("# of Exceps Thrown", ".NET CLR Exceptions"))
            {
                var exceptionsThrownCounter = new PerformanceCounter { CategoryName = ".NET CLR Exceptions", CounterName = "# of Exceps Thrown", InstanceName = Process.GetCurrentProcess().ProcessName };
                output.AppendFormat("\nExceptions Thrown: {0}", exceptionsThrownCounter.NextValue());
            }

            return output.ToString();
        }
    }
}
