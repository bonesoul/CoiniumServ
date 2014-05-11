/*
 *   Coinium - Crypto Currency Pool Software - https://github.com/CoiniumServ/coinium
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
            System.Console.ForegroundColor = ConsoleColor.DarkYellow;
            System.Console.WriteLine("Copyright (C) 2013 - 2014, Coinium project - http://www.coinium.org");
            System.Console.WriteLine();
            System.Console.WriteLine("Coinium comes with ABSOLUTELY NO WARRANTY.");
            System.Console.WriteLine("This is free software, and you are welcome to redistribute it under certain conditions; see the LICENSE file for details.");
            System.Console.WriteLine();
            System.Console.WriteLine("You can contribute the development of the project by donating;");
            System.Console.WriteLine("BTC : 18qqrtR4xHujLKf9oqiCsjmwmH5vGpch4D.");
            System.Console.WriteLine("LTC : LMXfRb3w8cMUBfqZb6RUkFTPaT6vbRozPa.");
            System.Console.WriteLine("DOGE: D7mzHQtkWD9B1Xwnmjfg9x2DofbaZBg6Lc.");
            System.Console.WriteLine();
            System.Console.ResetColor();
        }
    }
}
