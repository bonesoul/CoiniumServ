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

using Coinium.Core.Config;

namespace Coinium.Core.Server.Web
{
    public sealed class Config : ConfigSection
    {
        private Config()
            : base("WebServer")
        { }

        public bool Enabled
        {
            get { return this.GetBoolean("Enabled", true); }
            set { this.Set("Enabled", value); }
        }

        public int Port
        {
            get { return this.GetInt("Port", 80); }
            set { this.Set("Port", value); }
        }

        public string Interface
        {
            get { return this.GetString("Interface", "localhost"); }
            set { this.Set("Interface", value); }
        }

        private static readonly Config _instance = new Config();
        public static Config Instance { get { return _instance; } }
    }
}