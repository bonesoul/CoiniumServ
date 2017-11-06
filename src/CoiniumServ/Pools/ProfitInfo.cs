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
