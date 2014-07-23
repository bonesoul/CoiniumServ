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

using CoiniumServ.Configuration;
using Serilog.Events;

namespace CoiniumServ.Logging
{
    public interface ILogTarget: IConfig
    {
        bool Enabled { get; }

        /// <summary>
        /// console, file or packet.
        /// </summary>
        LogTargetType Type { get; }

        /// <summary>
        /// filename of the log file.
        /// </summary>
        string Filename { get; }

        /// <summary>
        /// set this true to get a seperate log file for every day.
        /// </summary>
        bool Rolling { get; }

        /// <summary>
        /// minimum log level.
        /// </summary>
        LogEventLevel Level { get; }
    }

    public enum LogTargetType
    {
        /// <summary>
        /// Logs to console.
        /// </summary>
        Console,
        /// <summary>
        /// Logs to a file.
        /// </summary>
        File,
        /// <summary>
        /// Logs network packets to a file.
        /// </summary>
        Packet
    }
}
