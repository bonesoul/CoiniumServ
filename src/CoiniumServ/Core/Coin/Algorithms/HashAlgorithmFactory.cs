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

using System.Collections.Generic;
using Serilog;

namespace Coinium.Core.Coin.Algorithms
{
    public static class HashAlgorithmFactory
    {
        private static readonly Dictionary<string, IHashAlgorithm> Algorithms;
        static HashAlgorithmFactory()
        {
            Algorithms = new Dictionary<string, IHashAlgorithm>
            {
                {"scrypt", new Scrypt()}
            };
        }

        public static IHashAlgorithm Get(string algorithm)
        {
            switch (algorithm.ToLower())
            {
                case "scrypt":
                    return Algorithms["scrypt"];
                default:
                    Log.Warning("Unknown algorithm: {0}", algorithm);
                    return null;
            }
        }
    }
}
