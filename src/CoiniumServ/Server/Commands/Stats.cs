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
using System.Diagnostics;
using System.Text;
using Coinium.Common.Commands;

namespace Coinium.Server.Commands
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
