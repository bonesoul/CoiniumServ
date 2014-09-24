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
using JsonConfig;
using Serilog;

namespace CoiniumServ.Coin.Config
{
    public class BlockTemplateOptions:IBlockTemplateOptions
    {
        public bool ModeRequired { get; private set; }

        public bool Valid { get; private set; }

        public BlockTemplateOptions(dynamic config)
        {
            try
            {
                // if no value is set, use the default value as false.
                ModeRequired = config.modeRequired is NullExceptionPreventer ? false : config.modeRequired;

                Valid = true;
            }
            catch (Exception e)
            {
                Valid = false;
                Log.Logger.ForContext<CoinOptions>().Error("Error loading block template options: {0:l}", e.Message);
            }
        }
    }
}
