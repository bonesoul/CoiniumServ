/*
 *   CoiniumServ - crypto currency pool software - https://github.com/CoiniumServ/CoiniumServ
 *   Copyright (C) 2013 - 2014, Coinium Project - http://www.coinium.org
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

using System.IO;
using JsonConfig;
using Serilog;
using Serilog.Events;

namespace Coinium.Common.Logging
{
    /// <summary>
    /// Controls the logging facilities.
    /// </summary>
    public static class Logging
    {
        public const string LogRoot = @"logs";

        public static void Init()
        {
            if (!Directory.Exists(LogRoot)) // make sure log root exists.
                Directory.CreateDirectory(LogRoot);

            var root = !string.IsNullOrEmpty(Config.Global.logs.root) ? Config.Global.logs.root : "logs";               

            // configure the global logger.
            Log.Logger = new LoggerConfiguration()
                .WriteTo.ColoredConsole()
                .WriteTo.RollingFile(@"logs\server-{Date}.log",restrictedToMinimumLevel: LogEventLevel.Information)
                .WriteTo.RollingFile(@"logs\debug-{Date}.log")
                .MinimumLevel.Verbose()
                .CreateLogger();
        }
    }
}
