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
