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
using System.Globalization;

namespace CoiniumServ.Persistance.Layers.Hybrid
{
    public partial class HybridStorage
    {
        public IDictionary<string, double> GetHashrateData(int since)
        {
            var hashrates = new Dictionary<string, double>();

            try
            {
                if (!IsEnabled || !_redisProvider.IsConnected)
                    return hashrates;

                var key = string.Format("{0}:hashrate", _coin);

                var results = _redisProvider.Client.ZRangeByScore(key, since, double.PositiveInfinity);

                foreach (var result in results)
                {
                    var data = result.Split(':');
                    var share = double.Parse(data[0].Replace(',', '.'), CultureInfo.InvariantCulture);
                    var worker = data[1];

                    if (!hashrates.ContainsKey(worker))
                        hashrates.Add(worker, 0);

                    hashrates[worker] += share;
                }
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while getting hashrate data: {0:l}", e.Message);
            }

            return hashrates;
        }

        public void DeleteExpiredHashrateData(int until)
        {
            try
            {
                if (!IsEnabled || !_redisProvider.IsConnected)
                    return;

                var key = string.Format("{0}:hashrate", _coin);
                _redisProvider.Client.ZRemRangeByScore(key, double.NegativeInfinity, until);
            }
            catch (Exception e)
            {
                _logger.Error("An exception occured while deleting expired hashrate data: {0:l}", e.Message);
            }
        }
    }
}
