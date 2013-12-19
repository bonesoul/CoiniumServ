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

namespace coinium.Common.Platform
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
