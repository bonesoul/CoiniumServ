#region License
// 
//     CoiniumServ - crypto currency pool software - https://github.com/CoiniumServ/CoiniumServ
//     Copyright (C) 2013 - 2014, Coinium Project - http://www.coinium.org
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
using Coinium.Server.Stratum.Notifications;

namespace Coinium.Mining.Jobs
{
    public interface IJobManager
    {
        Dictionary<UInt64, IJob> Jobs { get; }
        IJobCounter JobCounter { get; }

        IExtraNonce ExtraNonce { get; }

        IJob LastJob { get; }

        IJob GetJob(UInt64 id);

        void AddJob(IJob job);

        void Broadcast();

        /// <summary>
        /// Initializes the specified pool.
        /// </summary>
        /// <param name="instanceId">The instance identifier.</param>
        void Initialize(UInt32 instanceId);
    }
}
