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
using System.Collections.Generic;
using System.Linq;
using CoiniumServ.Persistance.Layers;
using CoiniumServ.Persistance.Layers.Hybrid;
using CoiniumServ.Persistance.Layers.Mpos;
using CoiniumServ.Persistance.Layers.Null;
using Serilog;

namespace CoiniumServ.Persistance
{
    public class StorageConfig:IStorageConfig
    {
        public bool Valid { get; private set; }

        public IStorageLayerConfig Layer { get; private set; }

        private readonly ILogger _logger;

        public StorageConfig(dynamic config)
        {
            try
            {
                _logger = Log.ForContext<StorageConfig>();

                // try loading layer configs.

                var layers = new List<IStorageLayerConfig>
                {
                    new HybridStorageConfig(config.hybrid),
                    new MposStorageConfig(config.mpos)
                };

                var enabledLayers = layers.Count(layer => layer.Enabled && layer.Valid); // find the count of enabled storage layers.

                if (enabledLayers == 0) // make sure we have at least a single enabled layer.
                {
                    _logger.Error("Storage will be not working as no valid storage configuration was found!");
                    Layer = NullStorageConfig.Null;
                    Valid = false;
                }
                else if (enabledLayers > 1) // we can have either hybrid or mpos layer enabled only at a time.
                {
                    _logger.Error("Storage will be not working as only a single storage configuration can be enabled!");
                    Layer = NullStorageConfig.Null;
                    Valid = false;
                }
                else // the configuration meets our expectations
                {
                    Layer = layers.First(layer => layer.Enabled && layer.Valid); // set the enabled layer.
                    Valid = true;
                }
            }
            catch (Exception e)
            {
                Valid = false;
                _logger.Error(e, "Error loading storage configuration");
            }
        }
    }
}
