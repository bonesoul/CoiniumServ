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

namespace Coinium.Utils.Platform
{
    /// <summary>
    /// Platform Manager that identifies platforms & manages them.
    /// </summary>
    public class PlatformManager
    {
        /// <summary>
        /// Current .Net framework.
        /// </summary>
        public static NetFrameworks Framework { get; private set; }

        /// <summary>
        /// Current .Net framework's version.
        /// </summary>
        public static Version FrameworkVersion { get; private set; }

        static PlatformManager()
        {
            IdentifyPlatform();
        }

        /// <summary>
        /// Identifies the current platform and used frameworks.
        /// </summary>
        private static void IdentifyPlatform()
        {
            // find dot.net framework.
            Framework = IsRunningOnMono() ? NetFrameworks.Mono : NetFrameworks.DotNet;
            FrameworkVersion = Environment.Version;
        }

        /// <summary>
        /// Returns true if code runs over Mono framework.
        /// </summary>
        /// <returns>true if running over Mono, false otherwise.</returns>
        public static bool IsRunningOnMono()
        {
            return Type.GetType("Mono.Runtime") != null;
        }
    }
}
