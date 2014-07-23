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

using System.Threading;
using CoiniumServ.Factories;

namespace CoiniumServ.Statistics
{
    public class Statistics:IStatistics, IStatisticsProvider
    {
        public IGlobal Global { get; private set; }
        public IAlgorithms Algorithms { get; private set; }
        public IPools Pools { get; private set; }

        private readonly Timer _timer;
        private const int TimerExpiration = 10;

        public Statistics(IObjectFactory statisticsObjectFactory)
        {
            Pools = statisticsObjectFactory.GetPoolStats();
            Global = statisticsObjectFactory.GetGlobalStatistics();
            Algorithms = statisticsObjectFactory.GetAlgorithmStatistics();

            _timer = new Timer(Recache, null, Timeout.Infinite, Timeout.Infinite); // create the timer as disabled.
            Recache(null); // recache data initially.
        }

        public void Recache(object state)
        {
            Pools.Recache(state);
            Algorithms.Recache(state);
            Global.Recache(state);

            // reset the recache timer.
            _timer.Change(TimerExpiration * 1000, Timeout.Infinite);
        }
    }
}
