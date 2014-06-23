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
using Newtonsoft.Json;
using NSubstitute;
using Xunit;

/* Sample data

    block template:
    ---------------
    rpcData: {"version":2,"previousblockhash":"aa3c3bf83b7f5a83eec6b8f1f9d084d6ad959e2450bca49c0e1731ee650d227c","transactions":[{"data":"01000000016776b0bb23755f339fd686cc3520232fcabc67b1cf7c46db5e1001f1548f02c3000000006c493046022100e232b56bd73a289503a34d8d995d387a61d0d7b3f47937cc42d40cfde2ef2b88022100ae7d07d5e1e55a321ce376d7ecd8933e021461c75e61b1d3cdba5f5cc445ae050121026b4b206a1ae5eb0fb035f06620a776f7937f33b18c48546ec11a6487e044ebd2ffffffff03a0c44a00000000001976a91450ebe876273d158b45637c63634477dd837703ff88acc0e1e400000000001976a91474a6c12c3a0eb28c60858e7789874fd14f89fc3f88ac001bb700000000001976a9146ef79c825d1d6ab7091f895209821eb88c2d692588ac00000000","hash":"95cf883db8b289db5ca8feccbcc3f98a6eda9adc266feaff9793d4c034fe9207","depends":[],"fee":100000,"sigops":3},{"data":"0100000002d2dfaea3edde08bdd295d0beeebede257d345f8ba4fffb6249fd290cce5cba9d000000006c4930460221008a18bc398220fd5d0a8a98d83856fb64ba6676d8584be242cbcd41656544215d022100a99eecb0b6a97c1a58a37cb70b2a3e960d4e87f5a34f4a121cb0619235396bf2012103d102ea35118140bfda0e2c1275fa69de7b811e81f81d502f1f35da28cb0740d9ffffffff703037e59b412c35452188acd7641b1e63dbc0f88a36e24274308ab09301fe73010000006c493046022100dd9f0782971093966d1e06f9fac84d3b46dc2da7760d967b57f59487f97a39aa0221009644d7b4c70e4303249a005def501db32450c059f80ac247b8ec232a2a562a1d012103fe72b8849d57acc4cfc6083e52c88e15502409977a8896aaeea91621564257b3ffffffff0240771b00000000001976a91411bb8487673ed706da0fbbf10511fa76c33a54f988ac001bb700000000001976a914b2ab4c235481136de520835e1d68c3a26f2c081488ac00000000","hash":"9fa57db425858c8c66cd443909b04dd34ebe5f70bba36f4cd0dbffccd0761c6e","depends":[],"fee":100000,"sigops":2}],"coinbaseaux":{"flags":"062f503253482f"},"coinbasevalue":5000200000,"target":"000000ffff000000000000000000000000000000000000000000000000000000","mintime":1403532080,"mutable":["time","transactions","prevblock"],"noncerange":"00000000ffffffff","sigoplimit":20000,"sizelimit":1000000,"curtime":1403532193,"bits":"1e00ffff","height":312753}
 
    generation transaction:
    --------------- 
    -- scriptSigPart data --
    -> height: 312753 serialized: 03b1c504
    -> coinbase: 062f503253482f hex: 062f503253482f
    -> date: 1403532192250 final:1403532192 serialized: 04a033a853
    -- p1 data --
    txVersion: 1 packed: 01000000
    txInputsCount: 1 varIntBuffer: 01
    txInPrevOutHash: 0 uint256BufferFromHash: 0000000000000000000000000000000000000000000000000000000000000000
    txInPrevOutIndex: 4294967295 packUInt32LE: ffffffff
    scriptSigPart1.length: 17 extraNoncePlaceholder.length:8 scriptSigPart2.length:14 all: 39 varIntBuffer: 27
    scriptSigPart1: 03b1c504062f503253482f04a033a85308
    p1: 01000000010000000000000000000000000000000000000000000000000000000000000000ffffffff2703b1c504062f503253482f04a033a85308
    -- generateOutputTransactions --
    block-reward: 5000200000
    recipient-reward: 50002000 packInt64LE: 50f8fa0200000000
    lenght: 25 varIntBuffer: 19
    script: 76a9147d576fbfca48b899dc750167dd2a2a6572fff49588ac
    pool-reward: 4950198000 packInt64LE: f0060e2701000000
    lenght: 25 varIntBuffer: 19
    script: 76a914329035234168b8da5af106ceb20560401236849888ac
    txOutputBuffers.lenght : 2 varIntBuffer: 02
    -- p2 --
    scriptSigPart2: 0d2f6e6f64655374726174756d2f
    txInSequence: 0 packUInt32LE: 00000000
    outputTransactions: 02f0060e27010000001976a914329035234168b8da5af106ceb20560401236849888ac50f8fa02000000001976a9147d576fbfca48b899dc750167dd2a2a6572fff49588ac
    txLockTime: 0 packUInt32LE: 00000000
    txComment: 
    p2: 0d2f6e6f64655374726174756d2f0000000002f0060e27010000001976a914329035234168b8da5af106ceb20560401236849888ac50f8fa02000000001976a9147d576fbfca48b899dc750167dd2a2a6572fff49588ac00000000

    job-params:
    ---------------
    getJobParams: ["1","650d227c0e1731ee50bca49cad959e24f9d084d6eec6b8f13b7f5a83aa3c3bf8","01000000010000000000000000000000000000000000000000000000000000000000000000ffffffff2703b1c504062f503253482f04a033a85308","0d2f6e6f64655374726174756d2f0000000002f0060e27010000001976a914329035234168b8da5af106ceb20560401236849888ac50f8fa02000000001976a9147d576fbfca48b899dc750167dd2a2a6572fff49588ac00000000",["0792fe34c0d49397ffea6f26dc9ada6e8af9c3bcccfea85cdb89b2b83d88cf95","d485a87ed591434bd41ae2e43e148194c11696b885aebdbf11a13b94d6b2b8b3"],"00000002","1e00ffff","53a833a1",true]
 
    handle-submit:
    ---------------
    handleSubmit: {"params":["mn4jUMneEBjZuDPEdFuj6BmFPmehmrT2Zc","1","00000000","53a833a1","7aa65500"],"id":2,"method":"mining.submit"}
 
    process-share:
    ---------------
    processShare- jobId: 1 previousDifficulty: undefined difficulty: 32 extraNonce1: 18000000 extraNonce2: 00000000 nTime: 53a833a1 ipAddress: 10.0.0.40 port: 3333 workerName: mn4jUMneEBjZuDPEdFuj6BmFPmehmrT2Zc
    coinbaseBuffer: 01000000010000000000000000000000000000000000000000000000000000000000000000ffffffff2703b1c504062f503253482f04a033a8530818000000000000000d2f6e6f64655374726174756d2f0000000002f0060e27010000001976a914329035234168b8da5af106ceb20560401236849888ac50f8fa02000000001976a9147d576fbfca48b899dc750167dd2a2a6572fff49588ac00000000
    coinbaseHash: 8a1ee357a6d3e534f940b989589b274782b6c75bf8e1e6aa6446555b3f554746
    merkleRoot: 4863264eecac1c2e3eb34787a1de7eaa7892e94d8171f44938134235ef878796
    headerBuffer: 020000007c220d65ee31170e9ca4bc50249e95add684d0f9f1b8c6ee835a7f3bf83b3caa968787ef3542133849f471814de99278aa7edea18747b33e2e1cacec4e266348a133a853ffff001e0055a67a
    headerHash: 9f32b533452421e6d1b6d551eaa211c44d8960721fbb7def4fdaf2f856050000
    headerBigNum: 36853504499501551065917854588454243228055102381977310589860745642848927
    shareDiff: 47.94171216079637 diff1: 2.695953529101131e+67 shareMultiplier: 65536
    blockDiffAdjusted : 256 job.difficulty: 0.00390625
    2014-06-23 17:03:38 [Pool]	[litecoin] (Thread 1) Share accepted at diff 32/47.94171216 by mn4jUMneEBjZuDPEdFuj6BmFPmehmrT2Zc [10.0.0.40]
    emit: {"job":"1","ip":"10.0.0.40","port":3333,"worker":"mn4jUMneEBjZuDPEdFuj6BmFPmehmrT2Zc","height":312753,"blockReward":5000200000,"difficulty":32,"shareDiff":"47.94171216","blockDiff":256,"blockDiffActual":0.00390625}
    previousblockhash: 4dd0a5ab630f0ced46f930f41f8d2ada50b7a8ed6e69adb7b043e5fdf5d8aa83 previousblockhashreversed: f5d8aa83b043e5fd6e69adb750b7a8ed1f8d2ada46f930f4630f0ced4dd0a5ab    
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
        private readonly IConnection _connection;
        private readonly IJobManager _jobManager;
        private readonly IHashAlgorithm _hashAlgorithm;
        private readonly StratumMiner _miner;
        private readonly GenerationTransaction _generationTransaction;        
        private readonly IJob _job;        

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

            // generation transaction
            _generationTransaction = Substitute.For<GenerationTransaction>(_extraNonce, _daemonClient, _blockTemplate, false);
            _generationTransaction.Create();

            // the job.
            _job = new Job(1, _blockTemplate, _generationTransaction, _merkleTree);

            // the job manager.
            _connection = Substitute.For<IConnection>();
            _jobManager = Substitute.For<IJobManager>();
            _jobManager.GetJob(1).Returns(_job);

            // hash algorithm
            _hashAlgorithm = Substitute.For<IHashAlgorithm>();

            // miner
            _miner = new StratumMiner(1, _connection);
        }

        [Fact]
        public void ProcessShare()
        {

            // create the share manager
            var shareManager = new ShareManager(_hashAlgorithm, _jobManager, _daemonClient);

            // init share the json
            const string shareJson = "{\"params\":[\"mn4jUMneEBjZuDPEdFuj6BmFPmehmrT2Zc\",\"1\",\"00000000\",\"53a02404\",\"efa22500\"],\"id\":2,\"method\":\"mining.submit\"}";
            dynamic @object = JsonConvert.DeserializeObject(shareJson);
            var shareObject = @object.@params;

            string minerName = shareObject[0];
            string jobId = shareObject[1];
            string extraNonce2 = shareObject[2];
            string nTime = shareObject[3];
            string nonce = shareObject[4];

            //shareManager.ProcessShare(_miner, jobId, extraNonce2, nTime, nonce);
        }
    }
}
