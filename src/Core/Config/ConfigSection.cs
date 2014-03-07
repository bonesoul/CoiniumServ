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

using Nini.Config;

namespace Coinium.Core.Config
{
    public class ConfigSection
    {
        public IConfig Section { get; private set; }

        /// <summary>
        /// Gets the section from main config.
        /// </summary>
        /// <param name="sectionName"></param>
        public ConfigSection(string sectionName)
        {
            this.Section = ConfigManager.Section(sectionName) ?? ConfigManager.AddSection(sectionName);
        }

        /// <summary>
        /// Gets the section from a given source.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sectionName"></param>
        public ConfigSection(IniConfigSource source, string sectionName)
        {
            this.Section = source.Configs[sectionName] ?? source.AddConfig(sectionName);
        }

        protected bool GetBoolean(string key, bool defaultValue) { return this.Section.GetBoolean(key, defaultValue); }
        protected double GetDouble(string key, double defaultValue) { return this.Section.GetDouble(key, defaultValue); }
        protected float GetFloat(string key, float defaultValue) { return this.Section.GetFloat(key, defaultValue); }
        protected int GetInt(string key, int defaultValue) { return this.Section.GetInt(key, defaultValue); }
        protected int GetInt(string key, int defaultValue, bool fromAlias) { return this.Section.GetInt(key, defaultValue, fromAlias); }
        protected long GetLong(string key, long defaultValue) { return this.Section.GetLong(key, defaultValue); }
        protected string GetString(string key, string defaultValue) { return this.Section.Get(key, defaultValue); }
        protected string[] GetEntryKeys() { return this.Section.GetKeys(); }
        protected void Set(string key, object value) { this.Section.Set(key, value); }
    }
}
