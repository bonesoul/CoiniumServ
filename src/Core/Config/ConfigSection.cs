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
        private readonly IConfig _section;

        public ConfigSection(string sectionName)
        {
            this._section = ConfigManager.Section(sectionName) ?? ConfigManager.AddSection(sectionName);
        }

        public void Save()
        {
            ConfigManager.Save();
        }

        protected bool GetBoolean(string key, bool defaultValue) { return this._section.GetBoolean(key, defaultValue); }
        protected double GetDouble(string key, double defaultValue) { return this._section.GetDouble(key, defaultValue); }
        protected float GetFloat(string key, float defaultValue) { return this._section.GetFloat(key, defaultValue); }
        protected int GetInt(string key, int defaultValue) { return this._section.GetInt(key, defaultValue); }
        protected int GetInt(string key, int defaultValue, bool fromAlias) { return this._section.GetInt(key, defaultValue, fromAlias); }
        protected long GetLong(string key, long defaultValue) { return this._section.GetLong(key, defaultValue); }
        protected string GetString(string key, string defaultValue) { return this._section.Get(key, defaultValue); }
        protected string[] GetEntryKeys() { return this._section.GetKeys(); }
        protected void Set(string key, object value) { this._section.Set(key, value); }
    }
}
