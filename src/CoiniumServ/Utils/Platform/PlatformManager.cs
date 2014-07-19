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
using Serilog;

namespace CoiniumServ.Utils.Platform
{
    /// <summary>
    /// Platform Manager that identifies platforms & manages them.
    /// </summary>
    public class PlatformManager
    {
        /// <summary>
        /// Current framework we are running on.
        /// </summary>
        public static Frameworks Framework { get; private set; }

        /// <summary>
        /// Is the framework .Net 4.5?
        /// </summary>
        public static bool IsDotNet45 { get; private set; }

        /// <summary>
        /// Current framework's version.
        /// </summary>
        public static Version FrameworkVersion { get; private set; }

        private static readonly ILogger Logger;

        static PlatformManager()
        {
            Logger = Log.ForContext<PlatformManager>();

            IdentifyPlatform();
        }

        public static void PrintPlatformBanner()
        {
            Logger.Information("Running over {0:l} {1:l} ({2:l}).", Framework == Frameworks.DotNet ? ".Net" : "Mono",
                IsDotNet45 ? "4.5" : "4",
                FrameworkVersion);
        }        

        /// <summary>
        /// Identifies the current platform and used frameworks.
        /// </summary>
        private static void IdentifyPlatform()
        {
            Framework = Type.GetType("Mono.Runtime") != null ? Frameworks.Mono : Frameworks.DotNet;
            IsDotNet45 = Type.GetType("System.Reflection.ReflectionContext", false) != null; /* ReflectionContext exists from .NET 4.5 onwards. */
            FrameworkVersion = Environment.Version;
        }
    }
}
