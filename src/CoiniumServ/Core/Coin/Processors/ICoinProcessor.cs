/*
 *   Coinium - Crypto Currency Pool Software - https://github.com/CoiniumServ/CoiniumServ
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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coinium.Core.Coin.Processors
{
    public interface ICoinProcessor
    {
        uint Multiplier { get; }

        byte[] Hash(byte[] input);

        byte[] BlockHash(byte[] input);

        string GenerateBlock(int version, uint prevBlockHash, uint merkleRootHash, uint time, uint bits, uint nonce);

        string GenerateHeader(byte[] blockHash);

        uint GenerateProofHash(string block);

        string PadHex(string hexString);
    }
}
