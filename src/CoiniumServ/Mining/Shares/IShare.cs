/*
 *   CoiniumServ - crypto currency pool software - https://github.com/CoiniumServ/CoiniumServ
 *   Copyright (C) 2013 - 2014, Coinium Project - http://www.coinium.org
 *
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Numerics;
using Coinium.Crypto;
using Coinium.Server.Stratum.Notifications;

namespace Coinium.Mining.Shares
{
    public interface IShare
    {
        bool Valid { get; }

        IJob Job { get; }

        UInt32 nTime { get; }

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

        bool Candidate { get; }

        byte[] BlockHex { get; }

        byte[] BlockHash { get; }
    }
}
