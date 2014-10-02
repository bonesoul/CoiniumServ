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
using Serilog;

namespace CoiniumServ.Coin.Config
{
    public class CoinConfig : ICoinConfig
    {
        public bool Valid { get; private set; }

        public string Name { get; private set; }

        public string Symbol { get; private set; }

        public string Algorithm { get; private set; }

        public string Site { get; private set; }

        public ICoinOptions Options { get; private set; }

        public IBlockExplorerOptions BlockExplorer { get; private set; }

        public dynamic Extra { get; private set; }

        public CoinConfig(dynamic config)
        {
            try
            {
                // set the coin data.
                Name = config.name;
                Symbol = config.symbol;
                Algorithm = config.algorithm;
                Site = string.IsNullOrEmpty(config.site) ? string.Empty : config.site;
                Options = new CoinOptions(config.options);
                BlockExplorer = new BlockExplorerOptions(config.blockExplorer);
                Extra = config.extra;

                if (Name == null || Symbol == null || Algorithm == null) // make sure we have valid name, symbol and algorithm data.
                    Valid = false;
                else
                    Valid = true;
            }
            catch (Exception e)
            {
                Valid = false;
                Log.Logger.ForContext<CoinConfig>().Error("Error loading coin configuration: {0:l}", e.Message);
            }
        }
    }
}
