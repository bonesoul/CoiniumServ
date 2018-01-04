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

using CoiniumServ.Coin.Config;
using CoiniumServ.Container.Context;
using CoiniumServ.Mining.Software;
using CoiniumServ.Pools;
using Nancy.TinyIoc;

namespace CoiniumServ.Configuration
{
    /// <summary>
    /// Configuration factory that handles configs.
    /// </summary>
    public class ConfigFactory:IConfigFactory
    {
        /// <summary>
        /// The _application context
        /// </summary>
        private readonly IApplicationContext _applicationContext;

        public ConfigFactory(IApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public IConfigManager GetConfigManager()
        {
            return _applicationContext.Container.Resolve<IConfigManager>();
        }

        public IJsonConfigReader GetJsonConfigReader()
        {
            return _applicationContext.Container.Resolve<IJsonConfigReader>();
        }

        public IPoolConfig GetPoolConfig(dynamic config, ICoinConfig coinConfig)
        {
            var @params = new NamedParameterOverloads
            {
                {"config", config},
                {"coinConfig", coinConfig}
            };

            return _applicationContext.Container.Resolve<IPoolConfig>(@params);
        }

        public ICoinConfig GetCoinConfig(dynamic config)
        {
            var @params = new NamedParameterOverloads
            {
                {"config", config},
            };

            return _applicationContext.Container.Resolve<ICoinConfig>(@params);
        }

        public IMiningSoftwareConfig GetMiningSoftwareConfig(dynamic config)
        {
            var @params = new NamedParameterOverloads
            {
                {"config", config},
            };

            return _applicationContext.Container.Resolve<IMiningSoftwareConfig>(@params);
        }
    }
}
 