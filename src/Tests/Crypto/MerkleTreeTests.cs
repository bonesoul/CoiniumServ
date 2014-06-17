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
using Coinium.Common.Extensions;
using Coinium.Crypto;
using Should.Fluent;
using Xunit;

namespace Tests.Crypto
{
    public class MerkleTreeTests
    {
        /// <summary>
        /// Tests steps of a merkle tree.
        /// </summary>
        [Fact]
        public void TestSteps()
        {
            var hashlist = new List<byte[]>
            {
                "999d2c8bb6bda0bf784d9ebeb631d711dbbbfe1bc006ea13d6ad0d6a2649a971".HexToByteArray(),
                "3f92594d5a3d7b4df29d7dd7c46a0dac39a96e751ba0fc9bab5435ea5e22a19d".HexToByteArray(),
                "a5633f03855f541d8e60a6340fc491d49709dc821f3acb571956a856637adcb6".HexToByteArray(),
                "28d97c850eaf917a4c76c02474b05b70a197eaefb468d21c22ed110afe8ec9e0".HexToByteArray(),
            };

            var merkleTree = new MerkleTree(hashlist);
            var result = merkleTree.WithFirst("d43b669fb42cfa84695b844c0402d410213faa4f3e66cb7248f688ff19d5e5f7".HexToByteArray());
            var expected = "82293f182d5db07d08acf334a5a907012bbb9990851557ac0ec028116081bd5a".HexToByteArray();

            result.Should().Equal(expected);
        }
    }
}
