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

using System;
using Coinium.Common.Helpers.IO;
using Nini.Config;
using Serilog;

namespace Coinium.Core.Config
{
    public sealed class ConfigManager
    {
        private static IniConfigSource _mainConfigParser; // the ini parser.
        private static string _mainConfigFile;
        private static bool _mainConfigFileExists = false; // does the ini file exists?

        static ConfigManager()
        {
            LoadMainConfig();
        }

        private static void LoadMainConfig()
        {
            try
            {
                _mainConfigFile = string.Format("{0}/Conf/{1}", FileHelpers.AssemblyRoot, "default.conf"); // the config file's location.
                _mainConfigParser = new IniConfigSource(_mainConfigFile); // see if the file exists by trying to parse it.
                _mainConfigFileExists = true;
            }
            catch (Exception)
            {
                _mainConfigParser = new IniConfigSource(); // initiate a new .ini source.
                _mainConfigFileExists = false;
                Log.Warning("Error loading settings {0}, will be using default settings.", _mainConfigFile);
            }
            finally
            {
                // adds aliases so we can use On and Off directives in ini files.
                _mainConfigParser.Alias.AddAlias("On", true);
                _mainConfigParser.Alias.AddAlias("Off", false);

                // logger level aliases.
                //Parser.Alias.AddAlias("MinimumLevel", Logger.Level.Trace);
                //Parser.Alias.AddAlias("MaximumLevel", Logger.Level.Trace);

                _mainConfigParser.ExpandKeyValues();
            }
        }

        static internal IConfig Section(string section) // Returns the asked config section.
        {
            return _mainConfigParser.Configs[section];
        }

        static internal IConfig AddSection(string section) // Adds a config section.
        {
            return _mainConfigParser.AddConfig(section);
        }
    }
}
