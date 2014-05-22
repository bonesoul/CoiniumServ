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

using Coinium.Core.Config;
using Coinium.Core.Context;

namespace Coinium.Core.Coin.Configs
{
    // todo: need to fix the factory.
    public class CoinConfigFactory:ICoinConfigFactory
    {
        /// <summary>
        /// The _application context
        /// </summary>
        private IApplicationContext _applicationContext;

        public CoinConfigFactory(IApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }


        public static ICoinConfig GetConfig(string name)  // todo: needs to be fixed - shouldn't be static
        {
            var fileName = string.Format("config/coins/{0}.json", name);
            var file = JsonConfigReader.Read(fileName);

            if (file == null)
                return null;

            return new CoinConfig(file);
        }        
    }
}
