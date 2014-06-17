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

using System.Collections.Generic;

namespace Coinium.Crypto
{
    /// <summary>
    /// Merkle tree builder.
    /// </summary>
    /// <remarks>
    /// To get a better understanding of merkle trees check: http://www.youtube.com/watch?v=gUwXCt1qkBU#t=09m09s 
    /// </remarks>
    /// <specification>https://en.bitcoin.it/wiki/Protocol_specification#Merkle_Trees</specification>
    /// <example>
    /// Python implementation: http://runnable.com/U3HnDaMrJFk3gkGW/bitcoin-block-merkle-root-2-for-python
    /// Original implementation: https://code.google.com/p/bitcoinsharp/source/browse/src/Core/Block.cs#330
    /// </example>
    public interface IMerkleTree
    {
        IList<byte[]> Steps { get; }

        List<string> Branches { get; }

        byte[] WithFirst(byte[] first);
    }
}
