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

namespace CoiniumServ.Utils
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
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(@"             .__       .__                                         ");
            Console.WriteLine(@"  ____  ____ |__| ____ |__|__ __  _____   ______ ______________  __");
            Console.WriteLine(@"_/ ___\/  _ \|  |/    \|  |  |  \/     \ /  ___// __ \_  __ \  \/ /");
            Console.WriteLine(@"\  \__(  <_> )  |   |  \  |  |  /  Y Y  \\___ \\  ___/|  | \/\   / ");
            Console.WriteLine(@" \___  >____/|__|___|  /__|____/|__|_|  /____  >\___  >__|    \_/  ");
            Console.WriteLine(@"     \/              \/               \/     \/     \/             ");
            Console.WriteLine();
        }

        /// <summary>
        /// Prints a copyright banner.
        /// </summary>
        public static void PrintLicense()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Copyright (C) 2013 - 2014, Coinium project - http://www.coinium.org");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("CoiniumServ comes with ABSOLUTELY NO WARRANTY.");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("You can contribute the development of the project by donating;");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("BTC : 18qqrtR4xHujLKf9oqiCsjmwmH5vGpch4D");
            Console.WriteLine("LTC : LMXfRb3w8cMUBfqZb6RUkFTPaT6vbRozPa");
            Console.WriteLine("DOGE: DM8FW8REMHj3P4xtcMWDn33ccjikCWJnQr");
            Console.WriteLine();
            Console.ResetColor();
        }
    }
}
