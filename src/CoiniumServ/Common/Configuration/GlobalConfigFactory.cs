#region License
// 
//     CoiniumServ - Crypto Currency Mining Pool Server Software
//     Copyright (C) 2013 - 2014, CoiniumServ Project - http://www.coinium.org
//     https://github.com/CoiniumServ/CoiniumServ
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
using Coinium.Common.Context;
using Serilog;

namespace Coinium.Common.Configuration
{
    public class GlobalConfigFactory : IGlobalConfigFactory
    {
        private const string FileName = "config.json";

        private dynamic _data = null;

        /// <summary>
        /// The _application context
        /// </summary>
        private IApplicationContext _applicationContext;

        public GlobalConfigFactory(IApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;            
        }

        public dynamic Get()
        {
            if (_data == null) // read the main config file, if we haven't so yet.
                _data = JsonConfigReader.Read(FileName);
            return _data ?? null;
        }
    }
}
