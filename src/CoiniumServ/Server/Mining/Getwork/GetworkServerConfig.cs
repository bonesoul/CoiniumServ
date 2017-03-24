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
using Serilog;

namespace CoiniumServ.Server.Mining.Getwork
{
    public class GetworkServerConfig : IGetworkServerConfig 
    {
        public bool Valid { get; private set; }

        public bool Enabled { get; private set; }

        public string BindInterface { get; private set; }

        public Int32 Port { get; private set; }

        public GetworkServerConfig(dynamic config)
        {
            try
            {
                // disable getwork server until it's implementation is done.
                Enabled = false;
                Valid = true;

                // load the config data.
                //Enabled = config.enabled;
                //BindInterface = string.IsNullOrEmpty(config.bind) ? "0.0.0.0" : config.bind;
                //Port = config.port;

                //Valid = true;
            }

            catch (Exception e)
            {
                Valid = false;
                Log.Logger.ForContext<GetworkServerConfig>().Error(e, "Error loading vannila server configuration");
            }
        }
    }
}
