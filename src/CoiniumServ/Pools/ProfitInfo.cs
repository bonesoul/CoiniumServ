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
    public class ProfitInfo : IProfitInfo
    {
        private readonly IPoolConfig _poolConfig;

        private readonly IMarketManager _marketManager;

        private readonly INetworkInfo _networkInfo;

        private readonly ILogger _logger;

        public decimal PriceInBtc { get; private set; }

        public decimal PriceInUsd { get; private set; }

        public double BlocksPerMhPerHour { get; private set; }

        public double CoinsPerMhPerHour { get; private set; }

        public double BtcPerMhPerHour { get; private set; }

        public double UsdPerMhPerHour { get; private set; }

        public ProfitInfo(IMarketManager marketManager, INetworkInfo networkInfo, IPoolConfig poolConfig)
        {
            _logger = Log.ForContext<ProfitInfo>().ForContext("Component", poolConfig.Coin.Name);

            _poolConfig = poolConfig;
            _networkInfo = networkInfo;
            _marketManager = marketManager;
            _marketManager.Update += OnMarketUpdate;
        }

        private void OnMarketUpdate(object sender, EventArgs e)
        {
            SetPrices();
            CalculateProfitability();
        }

        private void SetPrices()
        {
            var coinMarket = _marketManager.GetBestMarketFor(_poolConfig.Coin.Symbol, "BTC");
            if (coinMarket == null)
                return;

            var btcMarket = _marketManager.GetBestMarketFor("BTC", "USD");
            if (btcMarket == null)
                return;

            PriceInBtc = Math.Round((decimal)coinMarket.Bid, 8);
            PriceInUsd = Math.Round((decimal)btcMarket.Bid * PriceInBtc, 8);
        }

        private void CalculateProfitability()
        {
            const int interval = 86400; // second in a day - 60 * 60 * 24
            const int hashrate = 1 * 1000 * 1000; // hashrate - 1 MH/s.
            BlocksPerMhPerHour = interval / ((_networkInfo.Difficulty * Math.Pow(2, 32)) / hashrate);
            CoinsPerMhPerHour = BlocksPerMhPerHour*_networkInfo.Reward;
            BtcPerMhPerHour = CoinsPerMhPerHour*Convert.ToDouble(PriceInBtc);
            UsdPerMhPerHour = CoinsPerMhPerHour*Convert.ToDouble(PriceInUsd);
        }
    }
}
