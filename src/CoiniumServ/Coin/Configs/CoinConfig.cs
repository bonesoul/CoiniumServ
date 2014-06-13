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

namespace Coinium.Coin.Configs
{
    public class CoinConfig : ICoinConfig
    {
        public bool Valid { get; private set; }
        public string Name { get; private set; }
        public string Symbol { get; private set; }
        public string Algorithm { get; private set; }

        public CoinConfig(dynamic config)
        {
            if (config == null)
            {
                Valid = false;
                return;
            }

            Name = config.name;
            Symbol = config.symbol;
            Algorithm = config.algorithm;

            Valid = true;
        }
    }
}
