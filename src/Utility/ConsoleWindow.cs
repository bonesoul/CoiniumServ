/*
 *   Coinium project - crypto currency pool software - https://github.com/raistlinthewiz/coinium
 *   Copyright (C) 2013 Hüseyin Uslu, Int6 Studios - http://www.coinium.org
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

namespace coinium.Utility
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
            Console.WriteLine("Coinium comes with ABSOLUTELY NO WARRANTY.");
            Console.WriteLine("This is free software, and you are welcome to redistribute it under certain conditions; see the LICENSE file for details.");
            Console.WriteLine();
        }
    }
}
