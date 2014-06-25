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
using System.Linq;
using Coinium.Coin.Daemon;
using Coinium.Coin.Daemon.Responses;
using Coinium.Common.Extensions;
using Coinium.Crypto;
using Newtonsoft.Json;
using Should.Fluent;
using Xunit;

namespace Tests.Crypto
{
    public class MerkleTreeTests
    {
        // object mocks.
        private IBlockTemplate _blockTemplate;

        [Fact]
        public void TestsStepsWithZeroTransactions()
        {
            /* 
                coinbaseHash: dcbac3aae04bb6893d22b39426da75473c6d1e23eb3acd701ff682a6a1fecd76
                merkleRoot: 76cdfea1a682f61f70cd3aeb231e6d3c4775da2694b3223d89b64be0aac3badc
            */

            // block template
            const string json = "{\"result\":{\"version\":2,\"previousblockhash\":\"1a47638fd58c3b90cc3b2a7f1973dcdf545be4474d2157af28ad6ce7767acb09\",\"transactions\":[],\"coinbaseaux\":{\"flags\":\"062f503253482f\"},\"coinbasevalue\":5000000000,\"target\":\"000000ffff000000000000000000000000000000000000000000000000000000\",\"mintime\":1403563551,\"mutable\":[\"time\",\"transactions\",\"prevblock\"],\"noncerange\":\"00000000ffffffff\",\"sigoplimit\":20000,\"sizelimit\":1000000,\"curtime\":1403563962,\"bits\":\"1e00ffff\",\"height\":313498},\"error\":null,\"id\":1}";
            var blockTemplateObject = JsonConvert.DeserializeObject<DaemonResponse<BlockTemplate>>(json);
            _blockTemplate = blockTemplateObject.Result;

            var hashList = _blockTemplate.Transactions.Select(transaction => transaction.Hash.HexToByteArray()).ToList();
            var merkleTree = new MerkleTree(hashList);

            var merkleRoot = merkleTree.WithFirst("dcbac3aae04bb6893d22b39426da75473c6d1e23eb3acd701ff682a6a1fecd76".HexToByteArray()).ReverseBuffer();
            merkleRoot.ToHexString().Should().Equal("76cdfea1a682f61f70cd3aeb231e6d3c4775da2694b3223d89b64be0aac3badc");
        }

        [Fact]
        public void TestStepsWithSingleTransaction()
        {
            /* 
                coinbaseHash: 76a3f30f9dfdb980bf08a153f097c6456d5e0d290a41f760ce380c4b9c73f5d0
                merkleRoot: 7875fb5effb2f631634523f777090ba1568ec4c4ceee35a9b1c6832d24a23217
            */

            // block template
            const string json = "{\"result\":{\"version\":2,\"previousblockhash\":\"1c4eb88e47564cb796b5c6648c74bec51d7215ac12fc4168b14827aac74a8062\",\"transactions\":[{\"data\":\"010000000332a82e92f522deee69b09e27858ba9b87585f2a4913ef71018df40909032fdc3000000006a473044022019ca05cb880a04f0d842268b7e75ac6d2695fc544df033e3daeb29239251a8970220031f6336767f2ea617347484e1290ec0bdcc71056ea2d3084e75384905250ec50121030dd394118fb66ca288bff71d8ea762678783b005770f7f9ba4128233191e0847ffffffff086747cbd339b21b950774186091653a7b8f5751b00a906ff6f5561b3a6fcee6010000006b4830450221009ae1ba9a216d313cc592fc2c1ef08f1e0e555a32b6c1b305f685ac882d38356b0220243106bbb5bb76dde142e574cba8f30c1e2f7059e8e9161770396fbd2b50420f0121030dd394118fb66ca288bff71d8ea762678783b005770f7f9ba4128233191e0847ffffffffe2f15804b1e41c36c925c6f64f219b2bdb3c9fbff4c97a4f0e8c7f31d7e6f2af000000006b48304502200be8894fdd7f5c19be248a979c08bbf2395f606e038c3e02c0266474c03699ab022100ff5de87086e487410f5d7b68012655ca6d814f0caeb9ca42d9c425a90f68b3030121030dd394118fb66ca288bff71d8ea762678783b005770f7f9ba4128233191e0847ffffffff02a0f01900000000001976a9141c50209a1dfdf53313d237b75e9aeb553ca1dfda88ac00e1f505000000001976a914cbb9a3e7a7c1651b1006f876f08b40be85b274f588ac00000000\",\"hash\":\"dc3a80ec6c45aa489453b2c4abf6761eb6656d949e26d01793458c166640e5f3\",\"depends\":[],\"fee\":0,\"sigops\":2}],\"coinbaseaux\":{\"flags\":\"062f503253482f\"},\"coinbasevalue\":5000000000,\"target\":\"00000048d4f70000000000000000000000000000000000000000000000000000\",\"mintime\":1403691059,\"mutable\":[\"time\",\"transactions\",\"prevblock\"],\"noncerange\":\"00000000ffffffff\",\"sigoplimit\":20000,\"sizelimit\":1000000,\"curtime\":1403691825,\"bits\":\"1d48d4f7\",\"height\":315152},\"error\":null,\"id\":1}";
            var blockTemplateObject = JsonConvert.DeserializeObject<DaemonResponse<BlockTemplate>>(json);
            _blockTemplate = blockTemplateObject.Result;     

            var hashList = _blockTemplate.Transactions.Select(transaction => transaction.Hash.HexToByteArray()).ToList();
            var merkleTree = new MerkleTree(hashList);

            var merkleRoot = merkleTree.WithFirst("76a3f30f9dfdb980bf08a153f097c6456d5e0d290a41f760ce380c4b9c73f5d0".HexToByteArray()).ReverseBuffer();
            merkleRoot.ToHexString().Should().Equal("7875fb5effb2f631634523f777090ba1568ec4c4ceee35a9b1c6832d24a23217");
        }

        /// <summary>
        /// Tests steps of a merkle tree.
        /// </summary>
        [Fact]
        public void TestStepsWithThreeTransactions()
        {
            var hashlist = new List<byte[]>
            {
                "999d2c8bb6bda0bf784d9ebeb631d711dbbbfe1bc006ea13d6ad0d6a2649a971".HexToByteArray(),
                "3f92594d5a3d7b4df29d7dd7c46a0dac39a96e751ba0fc9bab5435ea5e22a19d".HexToByteArray(),
                "a5633f03855f541d8e60a6340fc491d49709dc821f3acb571956a856637adcb6".HexToByteArray(),
                "28d97c850eaf917a4c76c02474b05b70a197eaefb468d21c22ed110afe8ec9e0".HexToByteArray(),
            };

            var merkleTree = new MerkleTree(hashlist);
            var root = merkleTree.WithFirst("d43b669fb42cfa84695b844c0402d410213faa4f3e66cb7248f688ff19d5e5f7".HexToByteArray()).ReverseBuffer();

            root.ToHexString().Should().Equal("82293f182d5db07d08acf334a5a907012bbb9990851557ac0ec028116081bd5a");
        }
    }
}
