/*
 *   Coinium - Crypto Currency Pool Software - https://github.com/CoiniumServ/coinium
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

using System;
using Coinium.Core.Commands;

namespace Coinium.Core.Servers.Commands
{
    [CommandGroup("uptime", "Renders uptime statistics.")]
    public class UptimeCommand : CommandGroup
    {
        [DefaultCommand]
        public string Uptime(string[] @params)
        {
            var uptime = DateTime.Now - Program.StartupTime;
            return string.Format("Uptime: {0} days, {1} hours, {2} minutes, {3} seconds.", uptime.Days, uptime.Hours, uptime.Minutes, uptime.Seconds);
        }
    }
}
