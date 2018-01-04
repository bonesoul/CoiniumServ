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

using CoiniumServ.Utils.Helpers;
using Nancy;
using System.IO;

namespace CoiniumServ.Server.Web
{
    public class RootPathProvider : IRootPathProvider
    {
        private readonly string _rootPath; // root path of the web files.

        public RootPathProvider(string template)
        {
            // determine the root path
            #if DEBUG // on debug mode use static files form source directory, so live edits can be possible
                // note: we need to convert relative path to absolute path as nancy can only server static files with absolute path.
                _rootPath = Path.GetFullPath(FileHelpers.GetAbsolutePath($"../../../src/web/{template}")); // if not yet do so.
            #else // on release mode use static files from bin/Release.
                _rootPath = FileHelpers.GetAbsolutePath($"web/{template}");
            #endif
        }

        public string GetRootPath()
        {
            return _rootPath;
        }
    }
}
