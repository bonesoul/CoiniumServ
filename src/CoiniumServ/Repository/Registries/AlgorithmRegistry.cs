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

namespace CoiniumServ.Repository.Registries
{
    public class AlgorithmRegistry : IRegistry
    {
        private readonly IApplicationContext _applicationContext;

        public AlgorithmRegistry(IApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public void RegisterInstances()
        {
            // hash algorithms
            _applicationContext.Container.Register<IHashAlgorithm, Blake>(Algorithms.Blake).AsSingleton();
            _applicationContext.Container.Register<IHashAlgorithm, Fugue>(Algorithms.Fugue).AsSingleton();
            _applicationContext.Container.Register<IHashAlgorithm, Groestl>(Algorithms.Groestl).AsSingleton();
            _applicationContext.Container.Register<IHashAlgorithm, Keccak>(Algorithms.Keccak).AsSingleton();
            _applicationContext.Container.Register<IHashAlgorithm, Scrypt>(Algorithms.Scrypt).AsSingleton();
            _applicationContext.Container.Register<IHashAlgorithm, Sha256>(Algorithms.Sha256).AsSingleton();
            _applicationContext.Container.Register<IHashAlgorithm, Shavite3>(Algorithms.Shavite3).AsSingleton();
            _applicationContext.Container.Register<IHashAlgorithm, Skein>(Algorithms.Skein).AsSingleton();
            _applicationContext.Container.Register<IHashAlgorithm, X11>(Algorithms.X11).AsSingleton();
            _applicationContext.Container.Register<IHashAlgorithm, X13>(Algorithms.X13).AsSingleton();
            _applicationContext.Container.Register<IHashAlgorithm, X15>(Algorithms.X15).AsSingleton();
            _applicationContext.Container.Register<IHashAlgorithm, X17>(Algorithms.X17).AsSingleton();
        }
    }
}