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
            _applicationContext.Container.Register<IAlgorithmManager, AlgorithmManager>().AsSingleton();
            _applicationContext.Container.Register<IHashAlgorithm, Blake>(AlgorithmManager.Blake).AsSingleton();
            _applicationContext.Container.Register<IHashAlgorithm, Fugue>(AlgorithmManager.Fugue).AsSingleton();
            _applicationContext.Container.Register<IHashAlgorithm, Groestl>(AlgorithmManager.Groestl).AsSingleton();
            _applicationContext.Container.Register<IHashAlgorithm, Keccak>(AlgorithmManager.Keccak).AsSingleton();
            _applicationContext.Container.Register<IHashAlgorithm, Scrypt>(AlgorithmManager.Scrypt).AsSingleton();
            _applicationContext.Container.Register<IHashAlgorithm, Sha256>(AlgorithmManager.Sha256).AsSingleton();
            _applicationContext.Container.Register<IHashAlgorithm, Shavite3>(AlgorithmManager.Shavite3).AsSingleton();
            _applicationContext.Container.Register<IHashAlgorithm, Skein>(AlgorithmManager.Skein).AsSingleton();
            _applicationContext.Container.Register<IHashAlgorithm, X11>(AlgorithmManager.X11).AsSingleton();
            _applicationContext.Container.Register<IHashAlgorithm, X13>(AlgorithmManager.X13).AsSingleton();
            _applicationContext.Container.Register<IHashAlgorithm, X14>(AlgorithmManager.X14).AsSingleton();
            _applicationContext.Container.Register<IHashAlgorithm, X15>(AlgorithmManager.X15).AsSingleton();
            _applicationContext.Container.Register<IHashAlgorithm, X17>(AlgorithmManager.X17).AsSingleton();
        }
    }
}