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
using CoiniumServ.Vardiff;
using Serilog;

namespace CoiniumServ.Server.Mining.Stratum
{
    public class StratumServerConfig:IStratumServerConfig
    {
        public bool Valid { get; private set; }

        public bool Enabled { get; private set; }

        public string BindInterface { get; private set; }

        public Int32 Port { get; private set; }

        public float Diff { get; private set; }

        public IVardiffConfig Vardiff { get; private set; }

        public StratumServerConfig(dynamic config)
        {
            try
            {
                // load the config data.
                Enabled = config.enabled;
                BindInterface = string.IsNullOrEmpty(config.bind) ? "0.0.0.0" : config.bind;
                Port = config.port;
                Diff = config.diff == 0 ? 16 : (float)config.diff;
                Vardiff = new VardiffConfig(config.vardiff);

                Valid = true;
            }
            catch (Exception e)
            {
                Valid = false;
                Log.Logger.ForContext<StratumServerConfig>().Error(e, "Error loading stratum server configuration");
            }
        }
    }
}
