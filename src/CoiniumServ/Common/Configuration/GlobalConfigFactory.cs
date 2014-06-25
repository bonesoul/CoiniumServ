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

using Coinium.Common.Context;
using Serilog;

namespace Coinium.Common.Configuration
{
    public class GlobalConfigFactory : IGlobalConfigFactory
    {
        private const string FileName = "config.json";

        /// <summary>
        /// The _application context
        /// </summary>
        private IApplicationContext _applicationContext;

        public GlobalConfigFactory(IApplicationContext applicationContext)
        {
            Log.Debug("MainConfigFactory() init..");
            _applicationContext = applicationContext;            
        }

        public dynamic Get()
        {
            var data = JsonConfigReader.Read(FileName); // read the main config file

            if (data == null)
                return null;

            return data;
        }
    }
}
