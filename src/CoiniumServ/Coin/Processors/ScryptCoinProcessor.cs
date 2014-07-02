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
using System.Text;
using CryptSharp;
using CryptSharp.Utility;

namespace Coinium.Core.Coin.Processors
{
    public class ScryptCoinProcessor : ICoinProcessor
    {
        private const string Padding = "000000800000000000000000000000000000000000000000000000000000000000000000000000000000000080020000";

        private const int N = 1024;

        private const int R = 1;

        private const int P = 1;

        private readonly uint _multiplier;

        public uint Multiplier { get { return _multiplier; } }

        public ScryptCoinProcessor()
        {
            _multiplier = (UInt32)Math.Pow(2, 16);
        }

        public byte[] Hash(byte[] input)
        {
            var result = SCrypt.ComputeDerivedKey(input, input, N, R, P, null, 32);
            return result;
        }

        public byte[] BlockHash(byte[] input)
        {
            var result = Crypter.Sha256.Crypt(input);
            return new UTF8Encoding().GetBytes(result);
        }

        public string GenerateBlock(int version, uint prevBlockHash, uint merkleRootHash, uint time, uint bits, uint nonce)
        {
            var sb = new StringBuilder();
            sb.Append(version);
            sb.Append(prevBlockHash);
            sb.Append(merkleRootHash);
            sb.Append(time);
            sb.Append(bits);
            sb.Append(nonce);
            return sb.ToString();
        }

        public string GenerateHeader(byte[] blockHash)
        {
            var hex = new StringBuilder(blockHash.Length * 2);
            foreach (byte b in blockHash)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        public uint GenerateProofHash(string block)
        {
            return Convert.ToUInt32(Hash(new UTF8Encoding().GetBytes(block)));
        }

        public string PadHex(string hexString)
        {
            return hexString + Padding;
        }
    }
}
