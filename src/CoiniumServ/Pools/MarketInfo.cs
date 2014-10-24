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
using CoiniumServ.Markets;
using Serilog;

namespace CoiniumServ.Pools
{
    public class MarketInfo : IMarketInfo
    {
        private readonly IPoolConfig _poolConfig;

        private readonly IMarketManager _marketManager;

        private readonly ILogger _logger;

        public decimal PriceInBtc { get; private set; }

        public decimal PriceInUsd { get; private set; }

        public MarketInfo(IMarketManager marketManager, IPoolConfig poolConfig)
        {
            _logger = Log.ForContext<MarketInfo>().ForContext("Component", poolConfig.Coin.Name);

            _poolConfig = poolConfig;
            _marketManager = marketManager;
            _marketManager.Update += OnMarketUpdate;
        }

        private void OnMarketUpdate(object sender, EventArgs e)
        {
            PriceInBtc = Math.Round((decimal) _marketManager.GetBestMarketFor(_poolConfig.Coin.Symbol, "BTC").Bid, 8);
            PriceInUsd = Math.Round((decimal) _marketManager.GetBestMarketFor("BTC", "USD").Bid*PriceInBtc, 8);
        }
    }
}
