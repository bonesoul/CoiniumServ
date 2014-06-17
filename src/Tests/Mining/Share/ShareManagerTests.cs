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
using System.Linq;
using Coinium.Coin.Algorithms;
using Coinium.Coin.Daemon;
using Coinium.Coin.Daemon.Responses;
using Coinium.Common.Extensions;
using Coinium.Crypto;
using Coinium.Mining.Jobs;
using Coinium.Mining.Share;
using Coinium.Net.Server.Sockets;
using Coinium.Server.Stratum;
using Coinium.Server.Stratum.Notifications;
using Coinium.Transactions;
using Coinium.Transactions.Script;
using Newtonsoft.Json;
using NSubstitute;
using Xunit;

/* Sample data
    handleSubmit:
    {
        "params": [
            "mn4jUMneEBjZuDPEdFuj6BmFPmehmrT2Zc",
            "1",
            "00000000",
            "53a02404",
            "efa22500"
        ],
        "id": 2,
        "method": "mining.submit"
    }            
 */

namespace Tests.Mining.Share
{
    public class ShareManagerTests
    {
        // object mocks.
        private readonly IDaemonClient _daemonClient;
        private readonly IBlockTemplate _blockTemplate;
        private readonly IExtraNonce _extraNonce;
        private readonly IMerkleTree _merkleTree;

        public ShareManagerTests()
        {
            // daemon client
            _daemonClient = Substitute.For<IDaemonClient>();
            _daemonClient.ValidateAddress(Arg.Any<string>()).Returns(new ValidateAddress { IsValid = true });

            // block template
            const string json = "{\"result\":{\"version\":2,\"previousblockhash\":\"e9bbcc9b46ed98fd4850f2d21e85566defdefad3453460caabc7a635fc5a1261\",\"transactions\":[],\"coinbaseaux\":{\"flags\":\"062f503253482f\"},\"coinbasevalue\":5000000000,\"target\":\"0000004701b20000000000000000000000000000000000000000000000000000\",\"mintime\":1402660580,\"mutable\":[\"time\",\"transactions\",\"prevblock\"],\"noncerange\":\"00000000ffffffff\",\"sigoplimit\":20000,\"sizelimit\":1000000,\"curtime\":1402661060,\"bits\":\"1d4701b2\",\"height\":302526},\"error\":null,\"id\":1}";
            var @object = JsonConvert.DeserializeObject<DaemonResponse<BlockTemplate>>(json);
            _blockTemplate = @object.Result;

            // extra nonce
            _extraNonce = Substitute.For<IExtraNonce>();

            // merkle tree
            var hashList = _blockTemplate.Transactions.Select(transaction => transaction.Hash.HexToByteArray()).ToList();            
            _merkleTree = new MerkleTree(hashList);
        }

        [Fact]
        public void ProcessShare()
        {
            var generationTransaction = Substitute.For<GenerationTransaction>(_extraNonce, _daemonClient, _blockTemplate, false);
            generationTransaction.Create();

            var job = new Job(1, _blockTemplate, generationTransaction, _merkleTree);

            // create the job manager.
            var connection = Substitute.For<IConnection>();
            var jobManager = Substitute.For<IJobManager>();
            jobManager.GetJob(1).Returns(job);

            // create the share manager
            var miner = Substitute.For<StratumMiner>(1, connection);
            var hashAlgorithm = Substitute.For<IHashAlgorithm>();
            var shareManager = new ShareManager(hashAlgorithm, jobManager, _daemonClient);

            // init share the json
            const string shareJson = "{\"params\":[\"mn4jUMneEBjZuDPEdFuj6BmFPmehmrT2Zc\",\"1\",\"00000000\",\"53a02404\",\"efa22500\"],\"id\":2,\"method\":\"mining.submit\"}";
            dynamic @object = JsonConvert.DeserializeObject(shareJson);
            var shareObject = @object.@params;

            string minerName = shareObject[0];
            string jobId = shareObject[1];
            string extraNonce2 = shareObject[2];
            string nTime = shareObject[3];
            string nonce = shareObject[4];

            //shareManager.ProcessShare(miner, jobId, extraNonce2, nTime, nonce);
        }
    }
}
