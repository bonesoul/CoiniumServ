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

using System;
using CoiniumServ.Pools;
using Serilog;

namespace CoiniumServ.Persistance.Providers.Redis
{
    public class RedisProvider : IRedisProvider
    {
        public bool IsConnected { get { return Client.IsConnected; } }

        public RedisClient Client { get; private set; }
        
        // private readonly Version _requiredMinimumVersion = new Version(2, 6);

        private readonly IRedisProviderConfig _config;

        private readonly ILogger _logger;

        public RedisProvider(IPoolConfig poolConfig, IRedisProviderConfig config)
        {
            _logger = Log.ForContext<RedisProvider>().ForContext("Component", poolConfig.Coin.Name);

            _config = config;

            Initialize();
        }

        private void Initialize()
        {
            try
            {

                Client = new RedisClient(_config.Host, _config.Port, _config.Password, "version=2.6");

                // select the database
                Client.Select(_config.DatabaseId);

                /*
                // check the version
                var version = GetVersion();
                if (version < _requiredMinimumVersion)
                    throw new Exception(string.Format("You are using redis version {0}, minimum required version is 2.6", version));
                
                _logger.Information("Redis storage initialized: {0:l}:{1}, v{2:l}.", _config.Host, _config.Port, version);
                */

                _logger.Information("Redis storage initialized: {0:l}:{1}.", _config.Host, _config.Port);
            }
            catch (Exception e)
            {
                _logger.Error(e,"Redis storage initialization failed: {0:l}:{1}", _config.Host, _config.Port);
            }
        }

        /*
        private Version GetVersion()
        {
            Version version = null;

            try
            {
                var info = Client.Info("server");

                var parts = info.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

                foreach (var part in parts)
                {
                    var data = part.Split(':');

                    if (data[0] != "redis_version")
                        continue;

                    version = new Version(data[1]);
                }
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while getting version info: {0:l}", e.Message);
            }

            return version;
        }
        */
    }
}
