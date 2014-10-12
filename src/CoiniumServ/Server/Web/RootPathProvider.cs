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

using System.IO;
using CoiniumServ.Utils.Helpers;
using Nancy;

namespace CoiniumServ.Server.Web
{
    public class RootPathProvider : IRootPathProvider
    {
        private string _rootPath; // root path of the web files.

        private readonly string _template; // the template name.
        
        public RootPathProvider(string template)
        {
            _template = template;

            // determine the root path
            #if DEBUG // on debug mode use static files form source directory, so live edits can be possible
                // note: we need to convert relative path to absolute path as nancy can only server static files with absolute path.
                _rootPath = Path.GetFullPath(FileHelpers.GetAbsolutePath(string.Format("../../src/CoiniumServ/web/{0}", _template))); // if not yet do so.
            #else // on release mode use static files from bin/Release.
                _rootPath = FileHelpers.GetAbsolutePath(string.Format("web/{0}", _template));
            #endif
        }

        public string GetRootPath()
        {
            return _rootPath;
        }
    }
}
