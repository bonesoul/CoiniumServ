#region License
// 
//     CoiniumServ - Crypto Currency Mining Pool Server Software
//     Copyright (C) 2013 - 2014, CoiniumServ Project - http://www.coinium.org
//     https://github.com/CoiniumServ/CoiniumServ
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

namespace Coinium.Common.Console
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
            System.Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine(@"             .__       .__               ");
            System.Console.WriteLine(@"  ____  ____ |__| ____ |__|__ __  _____  ");
            System.Console.WriteLine(@"_/ ___\/  _ \|  |/    \|  |  |  \/     \ ");
            System.Console.WriteLine(@"\  \__(  <_> )  |   |  \  |  |  /  Y Y  \");
            System.Console.WriteLine(@" \___  >____/|__|___|  /__|____/|__|_|  /");
            System.Console.WriteLine(@"     \/              \/               \/ ");
            System.Console.WriteLine();
        }

        /// <summary>
        /// Prints a copyright banner.
        /// </summary>
        public static void PrintLicense()
        {
            System.Console.ForegroundColor = ConsoleColor.Magenta;
            System.Console.WriteLine("Copyright (C) 2013 - 2014, Coinium project - http://www.coinium.org");
            System.Console.WriteLine();
            System.Console.ForegroundColor = ConsoleColor.DarkYellow;
            System.Console.WriteLine("Coinium comes with ABSOLUTELY NO WARRANTY.");
            System.Console.WriteLine("This is free software, and you are welcome to redistribute it under certain conditions; see the LICENSE file for details.");
            System.Console.WriteLine();
            System.Console.ForegroundColor = ConsoleColor.Cyan;
            System.Console.WriteLine("You can contribute the development of the project by donating;");
            System.Console.WriteLine("BTC : 18qqrtR4xHujLKf9oqiCsjmwmH5vGpch4D.");
            System.Console.WriteLine("LTC : LMXfRb3w8cMUBfqZb6RUkFTPaT6vbRozPa.");
            System.Console.WriteLine("DOGE: D7mzHQtkWD9B1Xwnmjfg9x2DofbaZBg6Lc.");
            System.Console.WriteLine();
            System.Console.ResetColor();
        }
    }
}
