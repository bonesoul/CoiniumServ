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
using System.Collections.Generic;
using System.Dynamic;
using Serilog;

namespace CoiniumServ.Logging
{
    public class LogConfig : ILogConfig
    {
        public string Root { get; private set; }
        public List<ILogTarget> Targets { get; private set; }
        public bool Valid { get; private set; }

        public LogConfig(dynamic config)
        {
            try
            {
                Root = string.IsNullOrEmpty(config.root) ? "logs" : config.root;

                Targets = new List<ILogTarget>();

                if (config.targets is JsonConfig.NullExceptionPreventer)
                    AddDefaults(); // if we don't have any targets defined, setup a few default ones.
                else
                {
                    foreach (var data in config.targets)
                    {
                        var target = new LogTarget(data);
                        if (!target.Valid)
                            continue;

                        Targets.Add(target);
                    }
                }

                Valid = true;
            }
            catch (Exception e)
            {
                Valid = false;
                Log.Logger.ForContext<LogConfig>().Error(e, "Error loading logging configuration");
            }
        }

        private void AddDefaults()
        {
            dynamic consoleLog = new ExpandoObject();
            consoleLog.enabled = true;
            consoleLog.type = "console";
            consoleLog.level = "debug";

            dynamic serverLog = new ExpandoObject();
            serverLog.enabled = true;
            serverLog.type = "file";
            serverLog.filename = "server.log";
            serverLog.level = "information";
            serverLog.rolling = false;

            Targets.Add(new LogTarget(consoleLog));
            Targets.Add(new LogTarget(serverLog));
        }
    }
}
