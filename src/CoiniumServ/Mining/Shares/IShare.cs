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
using System;
using System.Numerics;
using Coinium.Crypto;
using Coinium.Server.Stratum.Notifications;

namespace Coinium.Mining.Shares
{
    public interface IShare
    {
        bool Valid { get; }
        bool Candidate { get; }

        ShareError Error { get; }

        IJob Job { get; }

        UInt32 NTime { get; }

        UInt32 Nonce { get; }

        UInt32 ExtraNonce1 { get; }

        UInt32 ExtraNonce2 { get; }

        byte[] CoinbaseBuffer { get; }

        Hash CoinbaseHash { get; }

        byte[] MerkleRoot { get; }

        byte[] HeaderBuffer { get; }

        byte[] HeaderHash { get; }

        BigInteger HeaderValue { get; }

        Double Difficulty { get; }

        Double BlockDiffAdjusted { get; }

        byte[] BlockHex { get; }

        byte[] BlockHash { get; }
    }
}
