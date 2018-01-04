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
