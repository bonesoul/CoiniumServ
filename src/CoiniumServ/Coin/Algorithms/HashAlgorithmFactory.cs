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

namespace Coinium.Coin.Algorithms
{
    public class HashAlgorithmFactory : IHashAlgorithmFactory
    {
        /// <summary>
        /// The application context.
        /// </summary>
        private readonly IApplicationContext _applicationContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="HashAlgorithmFactory" /> class.
        /// </summary>
        /// <param name="applicationContext">The application context.</param>
        public HashAlgorithmFactory(IApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        /// <summary>
        /// Gets the specified algorithm name.
        /// </summary>
        /// <param name="algorithmName">Name of the algorithm.</param>
        /// <returns></returns>
        public IHashAlgorithm Get(string algorithmName)
        {
            // Default to Scrypt
            if (string.IsNullOrWhiteSpace(algorithmName)) algorithmName = AlgorithmNames.Scrypt;

            return _applicationContext.Container.Resolve<IHashAlgorithm>(algorithmName);
        }
    }
}