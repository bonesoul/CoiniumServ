#region License
// 
//     CoiniumServ - Crypto Currency Mining Pool Server Software
//     Copyright (C) 2013 - 2014, CoiniumServ Project - http://www.coinium.org
//     http://www.coiniumserv.com - https://github.com/CoiniumServ/CoiniumServ
// 
//     This software is dual-licensed: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
//    
//     For the terms of this license, see licenses/gpl_v3.txt.
// 
//     Alternatively, you can license this software under a commercial
//     license or white-label it as set out in licenses/commercial.txt.
// 
#endregion

using CoiniumServ.Cryptology.Algorithms;
using CoiniumServ.Repository.Context;

namespace CoiniumServ.Factories
{
    /// <summary>
    /// Object factory that creates instances of objects
    /// </summary>
    public class ObjectFactory:IObjectFactory
    {
        /// <summary>
        /// The application context for internal use.
        /// </summary>
        private readonly IApplicationContext _applicationContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectFactory" /> class.
        /// </summary>
        /// <param name="applicationContext">The application context.</param>
        public ObjectFactory(IApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        /// <summary>
        /// Returns instance of the given hash algorithm.
        /// </summary>
        /// <param name="algorithm"></param>
        /// <returns></returns>
        public IHashAlgorithm GetHashAlgorithm(string algorithm)
        {
            return _applicationContext.Container.Resolve<IHashAlgorithm>(algorithm);
        }
    }
}
