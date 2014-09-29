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
using System.Reflection;
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

		/// <summary>
		/// Gets the mono version.
		/// </summary>
		/// <value>The mono version.</value>
		public static string MonoVersion { get; private set; }

        static PlatformManager()
        {
            IdentifyPlatform();
        }

        public static void PrintPlatformBanner()
        {
            Log.ForContext<PlatformManager>().Information("Running over {0:l}, framework: {1:l} (v{2:l}).",
                Framework == Frameworks.DotNet ? ".Net" : string.Format("Mono {0}", MonoVersion),
                IsDotNet45 ? "4.5" : "4", FrameworkVersion);
        }

        /// <summary>
        /// Identifies the current platform and used frameworks.
        /// </summary>
        private static void IdentifyPlatform()
        {
            Framework = Type.GetType("Mono.Runtime") != null ? Frameworks.Mono : Frameworks.DotNet;
            IsDotNet45 = Type.GetType("System.Reflection.ReflectionContext", false) != null; /* ReflectionContext exists from .NET 4.5 onwards. */
            FrameworkVersion = Environment.Version;

			if (Framework == Frameworks.Mono)
				MonoVersion = GetMonoVersion();
        }

		private static string GetMonoVersion()
		{
			// we can use reflection to get mono version using Mono.Runtime.GetDisplayName().

			var type = Type.GetType("Mono.Runtime");

			if (type == null)
				return string.Empty;
                   
			var displayName = type.GetMethod("GetDisplayName", BindingFlags.NonPublic | BindingFlags.Static); 

			if (displayName == null)
				return string.Empty;
						  
			return displayName.Invoke(null, null).ToString();
		}
    }
}
