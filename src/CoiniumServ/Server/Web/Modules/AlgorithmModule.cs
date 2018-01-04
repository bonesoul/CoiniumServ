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

using CoiniumServ.Algorithms;
using CoiniumServ.Server.Web.Models.Algorithm;
using Nancy;
using Nancy.CustomErrors;
using Nancy.Helpers;

namespace CoiniumServ.Server.Web.Modules
{
    public class AlgorithmModule:NancyModule
    {
        public AlgorithmModule(IAlgorithmManager algorithmManager)
            :base("/algorithm")
        {
            Get["/{slug}"] = _ =>
            {
                var algorithm = algorithmManager.Get(HttpUtility.HtmlEncode(_.slug)); // find the requested algorithm.

                if (algorithm == null)
                {
                    return View["error", new ErrorViewModel
                    {
                        Details = string.Format("The requested algorithm does not exist: {0}", _.slug)
                    }];                    
                }

                ViewBag.Header = algorithm.Name;

                return View["algorithm", new AlgorithmModel
                {
                    Algorithm = algorithm
                }];
            };
        }
    }
}
