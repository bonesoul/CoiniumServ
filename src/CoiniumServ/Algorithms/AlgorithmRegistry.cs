#region License
// 
//     MIT License
//
//     CoiniumServ - Crypto Currency Mining Pool Server Software
//     Copyright (C) 2013 - 2017, CoiniumServ Project
//     Hüseyin Uslu, shalafiraistlin at gmail dot com
//     https://github.com/bonesoul/CoiniumServ
// 
//     Permission is hereby granted, free of charge, to any person obtaining a copy
//     of this software and associated documentation files (the "Software"), to deal
//     in the Software without restriction, including without limitation the rights
//     to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//     copies of the Software, and to permit persons to whom the Software is
//     furnished to do so, subject to the following conditions:
//     
//     The above copyright notice and this permission notice shall be included in all
//     copies or substantial portions of the Software.
//     
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//     FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//     LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//     OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//     SOFTWARE.
// 
#endregion

using CoiniumServ.Algorithms.Implementations;
using CoiniumServ.Container.Context;
using CoiniumServ.Container.Registries;

namespace CoiniumServ.Algorithms
{
    public class AlgorithmRegistry : IRegistry
    {
        // algorithm names
        public const string Blake = "blake";
        public const string C11 = "c11";
        public const string Fresh = "fresh";
        public const string Fugue = "fugue";
        public const string Groestl = "groestl";
        public const string Keccak = "keccak";
        public const string Nist5 = "nist5";
        public const string Qubit = "qubit";
        public const string Scrypt = "scrypt";
        public const string ScryptOg = "scrypt-og";
        public const string ScryptN = "scrypt-n";
        public const string Sha1 = "sha1";
        public const string Sha256 = "sha256";
        public const string Shavite3 = "shavite3";
        public const string Skein = "skein";
        public const string X11 = "x11";
        public const string X13 = "x13";
        public const string X14 = "x14";
        public const string X15 = "x15";
        public const string X17 = "x17";
        public const string X16r = "x16r";

        // todo: add hefty1, qubit support

        private readonly IApplicationContext _applicationContext;

        public AlgorithmRegistry(IApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public void RegisterInstances()
        {
            // available cryptographic hash functions: http://en.wikipedia.org/wiki/List_of_hash_functions#Cryptographic_hash_functions

            // algorithm manager
            _applicationContext.Container.Register<IAlgorithmManager, AlgorithmManager>().AsSingleton();

            // per-algorithm statistics
            _applicationContext.Container.Register<IHashAlgorithmStatistics, HashAlgorithmStatistics>().AsMultiInstance();

            // sha variants
            _applicationContext.Container.Register<IHashAlgorithm, Sha256>(Sha256).AsMultiInstance();
            _applicationContext.Container.Register<IHashAlgorithm, Sha1>(Sha1).AsMultiInstance();

            // scrypt variants
            _applicationContext.Container.Register<IHashAlgorithm, Scrypt>(Scrypt).AsMultiInstance();
            _applicationContext.Container.Register<IHashAlgorithm, ScryptOg>(ScryptOg).AsMultiInstance();
            _applicationContext.Container.Register<IHashAlgorithm, ScryptN>(ScryptN).AsMultiInstance();

            // multi-hashers
            _applicationContext.Container.Register<IHashAlgorithm, X11>(X11).AsMultiInstance();
            _applicationContext.Container.Register<IHashAlgorithm, X13>(X13).AsMultiInstance();
            _applicationContext.Container.Register<IHashAlgorithm, X14>(X14).AsMultiInstance();
            _applicationContext.Container.Register<IHashAlgorithm, X15>(X15).AsMultiInstance();
            _applicationContext.Container.Register<IHashAlgorithm, X17>(X17).AsMultiInstance();
            _applicationContext.Container.Register<IHashAlgorithm, X16r>(X16r).AsMultiInstance();
            _applicationContext.Container.Register<IHashAlgorithm, C11>(C11).AsMultiInstance();

            // misc ones
            _applicationContext.Container.Register<IHashAlgorithm, Blake>(Blake).AsMultiInstance();
            _applicationContext.Container.Register<IHashAlgorithm, Fresh>(Fresh).AsMultiInstance();
            _applicationContext.Container.Register<IHashAlgorithm, Fugue>(Fugue).AsMultiInstance();
            _applicationContext.Container.Register<IHashAlgorithm, Groestl>(Groestl).AsMultiInstance();
            _applicationContext.Container.Register<IHashAlgorithm, Keccak>(Keccak).AsMultiInstance();
            _applicationContext.Container.Register<IHashAlgorithm, Nist5>(Nist5).AsMultiInstance();
            _applicationContext.Container.Register<IHashAlgorithm, Qubit>(Qubit).AsMultiInstance();
            _applicationContext.Container.Register<IHashAlgorithm, Shavite3>(Shavite3).AsMultiInstance();
            _applicationContext.Container.Register<IHashAlgorithm, Skein>(Skein).AsMultiInstance();
        }
    }
}
