#region License
// 
//     MIT License
//
//     CoiniumServ - Crypto Currency Mining Pool Server Software
//     Copyright (C) 2013 - 2017, CoiniumServ Project
//     Hüseyin Uslu, shalafiraistlin at gmail dot com
//     https://github.com/bonesoul/CoiniumServ
// 
//     Permission is hereby granted, free of charge, to any person obtaining a copy
//     of this software and associated documentation files (the "Software"), to deal
//     in the Software without restriction, including without limitation the rights
//     to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//     copies of the Software, and to permit persons to whom the Software is
//     furnished to do so, subject to the following conditions:
//     
//     The above copyright notice and this permission notice shall be included in all
//     copies or substantial portions of the Software.
//     
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//     FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//     LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//     OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//     SOFTWARE.
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
            Console.WriteLine("Copyright (C) 2013 - 2018, Coinium project - https://github.com/CoiniumServ/CoiniumServ");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("CoiniumServ comes with ABSOLUTELY NO WARRANTY.");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("You can contribute the development of the project by donating;");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(" BTC : 1MMdwRZg4K9p6oH2bWUQdohbxGbAvXS1t1");
            Console.WriteLine(" ETH : 0x61aa3e0709e20bcb4aedc2607d4070f1db72e69b");
            Console.WriteLine(" LTC : Ld8cy4ucf3FYThtfTnRQFFp5MKK9rZHjNg");
            Console.WriteLine();
            Console.ResetColor();
        }
    }
}
