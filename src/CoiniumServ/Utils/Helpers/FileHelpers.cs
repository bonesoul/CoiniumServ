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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CoiniumServ.Utils.Platform;
using Serilog;

namespace CoiniumServ.Utils.Helpers
{
    public static class FileHelpers
    {
        public static string AssemblyRoot
        {
            get { return AppDomain.CurrentDomain.BaseDirectory; }
        }

        public static string GetAbsolutePath(string file)
        {
            var path = Path.Combine(AssemblyRoot, file); // first get the path as *unix paths.

            if (PlatformManager.Framework == Frameworks.DotNet) // if we are running on windows,
                path = path.Replace('/', '\\'); // replace to windows-native paths.

            return path;
        }

        public static List<string> GetFilesByExtension(string directory, string expectedExtension)
        {
            var files = new List<string>(); // Store results in the file results list.

            try
            {
                var topDir = GetAbsolutePath(directory);
                var dirInfo = new DirectoryInfo(topDir);

                files.AddRange(from fileInfo in dirInfo.GetFiles()
                    where string.Compare(fileInfo.Extension, expectedExtension, StringComparison.OrdinalIgnoreCase) == 0
                    select string.Format("{0}/{1}", directory, fileInfo.Name));
            }
            catch (DirectoryNotFoundException e)
            {
                Log.Error("Directory not found: {0:l}", e.Message);
            }

            return files;
        }
    }
}