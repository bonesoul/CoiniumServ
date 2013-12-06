/*
 * Coinium project, Copyright (C) 2013, Int6 Studios - All Rights Reserved. - http://www.coinium.org
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace coinium_serv.Utility
{
    /// <summary>
    /// Utility class to handle console window stuff.
    /// </summary>
    class ConsoleWindow
    {
        /// <summary>
        /// Prints an info banner.
        /// </summary>
        public static void PrintBanner()
        {            
            Console.WriteLine(@"             .__       .__               ");
            Console.WriteLine(@"  ____  ____ |__| ____ |__|__ __  _____  ");
            Console.WriteLine(@"_/ ___\/  _ \|  |/    \|  |  |  \/     \ ");
            Console.WriteLine(@"\  \__(  <_> )  |   |  \  |  |  /  Y Y  \");
            Console.WriteLine(@" \___  >____/|__|___|  /__|____/|__|_|  /");
            Console.WriteLine(@"     \/              \/               \/ ");

            Console.WriteLine();
        }

        /// <summary>
        /// Prints a copyright banner.
        /// </summary>
        public static void PrintLicense()
        {
            Console.WriteLine("Copyright (C) 2013, Coinium project");
            Console.WriteLine("Coinium-serv comes with ABSOLUTELY NO WARRANTY.");
            Console.WriteLine("This is free software, and you are welcome to redistribute it under certain conditions; see the LICENSE file for details.");
            Console.WriteLine();
        }
    }
}
