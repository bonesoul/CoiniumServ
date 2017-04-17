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
using Serilog.Events;

namespace CoiniumServ.Logging
{
    public class LogTarget:ILogTarget
    {
        public bool Enabled { get; private set; }
        public LogTargetType Type { get; private set; }
        public string Filename { get; private set; }
        public bool Rolling { get; private set; }
        public LogEventLevel Level { get; private set; }
        public bool Valid { get; private set; }

        public LogTarget(dynamic config)
        {
            try
            {
                Enabled = config.enabled;

                switch ((string) config.type)
                {
                    case "file":
                        Type = LogTargetType.File;
                        break;
                    case "packet":
                        Type = LogTargetType.Packet;
                        break;
                    default:
                        return;
                }

                switch ((string) config.level)
                {
                    case "verbose":
                        Level = LogEventLevel.Verbose;
                        break;
                    case "debug":
                        Level = LogEventLevel.Debug;
                        break;
                    case "information":
                        Level = LogEventLevel.Information;
                        break;
                    case "warning":
                        Level = LogEventLevel.Warning;
                        break;
                    case "error":
                        Level = LogEventLevel.Error;
                        break;
                    case "fatal":
                        Level = LogEventLevel.Fatal;
                        break;
                }

                if (Type == LogTargetType.File || Type == LogTargetType.Packet)
                {
                    Filename = config.filename;
                    Rolling = config.rolling;
                }

                Valid = true;
            }
            catch (Exception e)
            {
                Valid = false;
                Log.Logger.ForContext<LogTarget>().Error(e, "Error loading log target configuration");
            }
        }
    }
}
