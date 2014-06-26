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
using System.Numerics;
using Coinium.Coin.Algorithms;
using Coinium.Coin.Daemon.Responses;
using Coinium.Crypto;
using Coinium.Transactions;

namespace Coinium.Server.Stratum.Notifications
{
    public interface IJob : IEnumerable<object>
    {
        UInt64 Id { get; }

        string PreviousBlockHash { get; }

        string PreviousBlockHashReversed { get;  }

        string CoinbaseInitial { get; }

        string CoinbaseFinal { get; }

        string Version { get; }

        string EncodedDifficulty { get; }

        BigInteger Target { get; }

        double Difficulty { get; }

        string nTime { get; }

        bool CleanJobs { get; set; }

        IHashAlgorithm HashAlgorithm { get; }

        IBlockTemplate BlockTemplate { get; }

        IGenerationTransaction GenerationTransaction { get; }

        IMerkleTree MerkleTree { get; }

        new IEnumerator<object> GetEnumerator();
    }
}
