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

using CoiniumServ.Configuration;

namespace CoiniumServ.Vardiff
{
    public interface IVardiffConfig : IConfig
    {
        bool Enabled { get; }

        /// <summary>
        /// minimum difficulty that can be assigned to miners.
        /// </summary>
        int MinimumDifficulty { get; }

        /// <summary>
        /// maximum difficulty that can be assigned to miners.
        /// </summary>
        int MaximumDifficulty { get; }

        /// <summary>
        /// try to get a single share per this many seconds from miner.
        /// </summary>
        int TargetTime { get; }

        /// <summary>
        /// retarget a miners difficulty ever this many seconds.
        /// </summary>
        int RetargetTime { get; }

        /// <summary>
        /// allow difficulty for a miner to vary this percent without retargeting.
        /// </summary>
        int VariancePercent { get; }
    }
}
