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

using System.Reflection;
using Nancy;
using Serilog;

namespace Coinium.Web.Modules
{
    public class Stats : NancyModule
    {
        public Stats()
        {
            var banner = string.Format("v{0}", Assembly.GetAssembly(typeof(Program)).GetName().Version);

            Get["/"] = x => "coinium";
            Get["/version"] = x => banner;
        }
    }
}
