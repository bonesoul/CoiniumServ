#region License
// 
//     CoiniumServ - Crypto Currency Mining Pool Server Software
//     Copyright (C) 2013 - 2014, CoiniumServ Project - http://www.coinium.org
//     http://www.coiniumserv.com - https://github.com/CoiniumServ/CoiniumServ
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

using System.Collections.Generic;
using CoiniumServ.Daemon;
using CoiniumServ.Daemon.Responses;
using Newtonsoft.Json;
using Should.Fluent;
using Xunit;

/* sample data - https://github.com/CoiniumServ/CoiniumServ/wiki/Litecoin-Testnet-Stream
     {
        "result": {
            "version": 2,
            "previousblockhash": "628f9771c0f6f0c4f7de9067ea6000fe1445ac4dae3ea0b2cb7a291f4ba8cdde",
            "transactions": [
                {
                    "data": "010000000ba688978919ba8bc07fe4a534bb91e73f10df759893a853d84362bcec733c343b000000006a47304402207cd91c6b5a06be1213d4a554d590b8ca5e89bbbdbdc609484201353cc5c56a9d022011c4c4f9f10c31397332a5cb768ddd0d571f8f30312ffb8efa7396320e46e0bf01210233638afa905e549afb53febc4aeb23dee9e906a38de894f0352c1e322a94c460ffffffffc0a870acbf4c8ae1b3c06e415b830dc174b7b83dfd65e193d16b772be20dc159000000006a473044022046128d16b96a917f632f0dcc51bc3cc5ac651f1dd80a65a23653c75a8732ccdc022052383cbc35cf21b89e1d8d963773ce052bab9e711e28d6c8955b075722ae610001210233638afa905e549afb53febc4aeb23dee9e906a38de894f0352c1e322a94c460ffffffffa8a1049d86076a990cdd55402c25d2ce0439db3650b11d9d808dfe6e5380cec1000000006a4730440220333ff2b803ab7c01100d65d57d38455d4f7d7f0c2bf83d638242b5b64ab14b0102202db5805b8e005aeca7f3de4540d0b00c513d8a475f22787b3317442ae720500901210233638afa905e549afb53febc4aeb23dee9e906a38de894f0352c1e322a94c460ffffffff5d8c52cb42cbb51e43b6c8a40444a12e38d84739b66033d6db5bd97d12190c0c000000006c49304602210084a4678cfa9e7b338150932537e83efd5a461876b02788ba1652a08793ae53b4022100f10792aa0b235e51f7c23dca49205ace5c5a912a3f7685ce932d081e6f42a19901210233638afa905e549afb53febc4aeb23dee9e906a38de894f0352c1e322a94c460ffffffff7aa2b162938bd59068061d8b6bc65c30da37d3edf66517bdcb8736e457aaf6bf000000006b483045022100ae57759407f72ef6fc771a064b27ddfe09b3df076be1ccb67db324ad8d623c5d02200cb4cdb8486746ffb8c8d4f8c426f936919564200fe2488db164061bc601402501210233638afa905e549afb53febc4aeb23dee9e906a38de894f0352c1e322a94c460fffffffff6dee530fc0390bfa94c28c477506f1b844650a63db4a7cff84f37c1385cc519000000006b483045022100ace7b74472bbaf00c1389508f537a999b868f19e619bddef53cef27ab510866e02205dfce1090c9c02f34a5527e073f957cc3522cf4b0a1cb821573c6153acd3b49a01210233638afa905e549afb53febc4aeb23dee9e906a38de894f0352c1e322a94c460ffffffffeafbfe205e452a8ca544c5ca63863b55b8acd8ae33a168cb00dd08a953676706000000006b483045022100ee78095f2a28ef90a0fb648623172252a263c22d523703bc0028dbb9181b078702207a295130989598d2573b896e3e8b922410d286f6d86bf82bcc8cc636fda256ce01210233638afa905e549afb53febc4aeb23dee9e906a38de894f0352c1e322a94c460ffffffff9c992ae24d265aa58c8ea89c33235247b9db317b3602d743e57dc3b4aa041282000000006b483045022100c4904faadf3cb3894831f0376c4962135c3573462ecbaa1641ba04c4097087a402201a34693d19f741d7463400386783afd1b9691b146bb59f81fb0f19ef82ba7c9701210233638afa905e549afb53febc4aeb23dee9e906a38de894f0352c1e322a94c460ffffffffd347a768891f6b83ba58c4a32dcc881ebfafd0f16023e1381eba6552cf11ed99000000006b4830450221008ba065070659f3bc8992aef5604434aaba1bfc16b1224b39a4be14bc1deb33d4022048d6f97b7fdeda1c3c626b05df1ea70300c25537c36ec4b200825b0333cda6ad01210233638afa905e549afb53febc4aeb23dee9e906a38de894f0352c1e322a94c460ffffffff89551fd7c7dd51b9a6e4aa9830caa769862c24c11ef453e873e5295f73cd5b59000000006b483045022056c4125114487c393c035be859dcf79fe840d091b1f095964acc9e791490cd1f022100af8effc6008d79428c3cafeeb3d43ab9fff5815f6584583e428c15ffc2d3bd3f01210233638afa905e549afb53febc4aeb23dee9e906a38de894f0352c1e322a94c460ffffffffeed66d4bfd40b0a423babde33b0550c39825bf5a7d6377caf1eddef1c14c9c02000000006b483045022100ead4bec71e9ee792ebb92f378d4592e1b65672b806ae9ed382401882902fd7520220497df514d5d08510fb1666f2a83a468999b204afcc4bd938a18eca1f1747e7f701210233638afa905e549afb53febc4aeb23dee9e906a38de894f0352c1e322a94c460ffffffff016072afad0c0000001976a91447d35ca845a6a11484e367e6be43f0d23636e04288ac00000000",
                    "hash": "0d507fde9e1b16386f0798128067adbd37cdee98acd27903f59b0877f7f31737",
                    "depends": [],
                    "fee": 0,
                    "sigops": 1
                },
                {
                    "data": "0100000001a666e23a6e64ddfe0eb28cf39ff4568c05b2f0617528910075946a4a4b6c0688000000006a473044022000f5e4d7490e379127d377786fa6003228d9b259f808eb932c2046248253f02602207b417e8042eaf9cbb9c759124072d4ddb2c478999ef8cefdfdb0f830bfb151210121023bb744351b1f6675ef023125090d5adcf1830a31da89a57a000c08367dadce70ffffffff02805b6d29010000001976a9148a7fd9881a93c13a63310462473a827d022b699a88ac80969800000000001976a914fc9963f591c418921a401373069f4158e295886388ac00000000",
                    "hash": "623de7ab570540ef659076e6be4f4b8227a47249ea727a8a5afba06a5251e332",
                    "depends": [],
                    "fee": 0,
                    "sigops": 2
                }
            ],
            "coinbaseaux": {
                "flags": "062f503253482f"
            },
            "coinbasevalue": 5000000000,
            "target": "0000007b58890000000000000000000000000000000000000000000000000000",
            "mintime": 1401274780,
            "mutable": [
                "time",
                "transactions",
                "prevblock"
            ],
            "noncerange": "00000000ffffffff",
            "sigoplimit": 20000,
            "sizelimit": 1000000,
            "curtime": 1401276010,
            "bits": "1d7b5889",
            "height": 283723
        },
        "error": null,
        "id": 1
    }
 */

namespace CoiniumServ.Tests.Daemon
{
    public class BlockTemplateTests
    {
        // object mocks.
        private IBlockTemplate _expected;

        public BlockTemplateTests()
        {
            // expected block template object.
            _expected = new BlockTemplate
            {
                Version = 2,
                PreviousBlockHash = "628f9771c0f6f0c4f7de9067ea6000fe1445ac4dae3ea0b2cb7a291f4ba8cdde",
                Transactions = new[]
                {
                    new BlockTemplateTransaction()
                    {
                        Data = "010000000ba688978919ba8bc07fe4a534bb91e73f10df759893a853d84362bcec733c343b000000006a47304402207cd91c6b5a06be1213d4a554d590b8ca5e89bbbdbdc609484201353cc5c56a9d022011c4c4f9f10c31397332a5cb768ddd0d571f8f30312ffb8efa7396320e46e0bf01210233638afa905e549afb53febc4aeb23dee9e906a38de894f0352c1e322a94c460ffffffffc0a870acbf4c8ae1b3c06e415b830dc174b7b83dfd65e193d16b772be20dc159000000006a473044022046128d16b96a917f632f0dcc51bc3cc5ac651f1dd80a65a23653c75a8732ccdc022052383cbc35cf21b89e1d8d963773ce052bab9e711e28d6c8955b075722ae610001210233638afa905e549afb53febc4aeb23dee9e906a38de894f0352c1e322a94c460ffffffffa8a1049d86076a990cdd55402c25d2ce0439db3650b11d9d808dfe6e5380cec1000000006a4730440220333ff2b803ab7c01100d65d57d38455d4f7d7f0c2bf83d638242b5b64ab14b0102202db5805b8e005aeca7f3de4540d0b00c513d8a475f22787b3317442ae720500901210233638afa905e549afb53febc4aeb23dee9e906a38de894f0352c1e322a94c460ffffffff5d8c52cb42cbb51e43b6c8a40444a12e38d84739b66033d6db5bd97d12190c0c000000006c49304602210084a4678cfa9e7b338150932537e83efd5a461876b02788ba1652a08793ae53b4022100f10792aa0b235e51f7c23dca49205ace5c5a912a3f7685ce932d081e6f42a19901210233638afa905e549afb53febc4aeb23dee9e906a38de894f0352c1e322a94c460ffffffff7aa2b162938bd59068061d8b6bc65c30da37d3edf66517bdcb8736e457aaf6bf000000006b483045022100ae57759407f72ef6fc771a064b27ddfe09b3df076be1ccb67db324ad8d623c5d02200cb4cdb8486746ffb8c8d4f8c426f936919564200fe2488db164061bc601402501210233638afa905e549afb53febc4aeb23dee9e906a38de894f0352c1e322a94c460fffffffff6dee530fc0390bfa94c28c477506f1b844650a63db4a7cff84f37c1385cc519000000006b483045022100ace7b74472bbaf00c1389508f537a999b868f19e619bddef53cef27ab510866e02205dfce1090c9c02f34a5527e073f957cc3522cf4b0a1cb821573c6153acd3b49a01210233638afa905e549afb53febc4aeb23dee9e906a38de894f0352c1e322a94c460ffffffffeafbfe205e452a8ca544c5ca63863b55b8acd8ae33a168cb00dd08a953676706000000006b483045022100ee78095f2a28ef90a0fb648623172252a263c22d523703bc0028dbb9181b078702207a295130989598d2573b896e3e8b922410d286f6d86bf82bcc8cc636fda256ce01210233638afa905e549afb53febc4aeb23dee9e906a38de894f0352c1e322a94c460ffffffff9c992ae24d265aa58c8ea89c33235247b9db317b3602d743e57dc3b4aa041282000000006b483045022100c4904faadf3cb3894831f0376c4962135c3573462ecbaa1641ba04c4097087a402201a34693d19f741d7463400386783afd1b9691b146bb59f81fb0f19ef82ba7c9701210233638afa905e549afb53febc4aeb23dee9e906a38de894f0352c1e322a94c460ffffffffd347a768891f6b83ba58c4a32dcc881ebfafd0f16023e1381eba6552cf11ed99000000006b4830450221008ba065070659f3bc8992aef5604434aaba1bfc16b1224b39a4be14bc1deb33d4022048d6f97b7fdeda1c3c626b05df1ea70300c25537c36ec4b200825b0333cda6ad01210233638afa905e549afb53febc4aeb23dee9e906a38de894f0352c1e322a94c460ffffffff89551fd7c7dd51b9a6e4aa9830caa769862c24c11ef453e873e5295f73cd5b59000000006b483045022056c4125114487c393c035be859dcf79fe840d091b1f095964acc9e791490cd1f022100af8effc6008d79428c3cafeeb3d43ab9fff5815f6584583e428c15ffc2d3bd3f01210233638afa905e549afb53febc4aeb23dee9e906a38de894f0352c1e322a94c460ffffffffeed66d4bfd40b0a423babde33b0550c39825bf5a7d6377caf1eddef1c14c9c02000000006b483045022100ead4bec71e9ee792ebb92f378d4592e1b65672b806ae9ed382401882902fd7520220497df514d5d08510fb1666f2a83a468999b204afcc4bd938a18eca1f1747e7f701210233638afa905e549afb53febc4aeb23dee9e906a38de894f0352c1e322a94c460ffffffff016072afad0c0000001976a91447d35ca845a6a11484e367e6be43f0d23636e04288ac00000000",
                        Hash = "0d507fde9e1b16386f0798128067adbd37cdee98acd27903f59b0877f7f31737",
                        Depends = new int[0],
                        Fee = 0,
                        Sigops = 1,
                    },
                    new BlockTemplateTransaction()
                    {
                        Data = "0100000001a666e23a6e64ddfe0eb28cf39ff4568c05b2f0617528910075946a4a4b6c0688000000006a473044022000f5e4d7490e379127d377786fa6003228d9b259f808eb932c2046248253f02602207b417e8042eaf9cbb9c759124072d4ddb2c478999ef8cefdfdb0f830bfb151210121023bb744351b1f6675ef023125090d5adcf1830a31da89a57a000c08367dadce70ffffffff02805b6d29010000001976a9148a7fd9881a93c13a63310462473a827d022b699a88ac80969800000000001976a914fc9963f591c418921a401373069f4158e295886388ac00000000",
                        Hash = "623de7ab570540ef659076e6be4f4b8227a47249ea727a8a5afba06a5251e332",
                        Depends = new int[0],
                        Fee = 0,
                        Sigops = 2,
                    },
                },
                CoinBaseAux = new CoinBaseAux
                {
                    Flags = "062f503253482f"
                },
                Coinbasevalue = 5000000000,
                Target = "0000007b58890000000000000000000000000000000000000000000000000000",
                MinTime = 1401274780,
                Mutable = new List<string>
                {
                    "time",
                    "transactions",
                    "prevblock"
                },
                NonceRange = "00000000ffffffff",
                SigOpLimit = 20000,
                SizeLimit = 1000000,
                CurTime = 1401276010,
                Bits = "1d7b5889",
                Height = 283723,
            };
            
        }

        [Fact]
        public void TestBlockTemplate()
        {
            const string json = "{\"result\":{\"version\":2,\"previousblockhash\":\"628f9771c0f6f0c4f7de9067ea6000fe1445ac4dae3ea0b2cb7a291f4ba8cdde\",\"transactions\":[{\"data\":\"010000000ba688978919ba8bc07fe4a534bb91e73f10df759893a853d84362bcec733c343b000000006a47304402207cd91c6b5a06be1213d4a554d590b8ca5e89bbbdbdc609484201353cc5c56a9d022011c4c4f9f10c31397332a5cb768ddd0d571f8f30312ffb8efa7396320e46e0bf01210233638afa905e549afb53febc4aeb23dee9e906a38de894f0352c1e322a94c460ffffffffc0a870acbf4c8ae1b3c06e415b830dc174b7b83dfd65e193d16b772be20dc159000000006a473044022046128d16b96a917f632f0dcc51bc3cc5ac651f1dd80a65a23653c75a8732ccdc022052383cbc35cf21b89e1d8d963773ce052bab9e711e28d6c8955b075722ae610001210233638afa905e549afb53febc4aeb23dee9e906a38de894f0352c1e322a94c460ffffffffa8a1049d86076a990cdd55402c25d2ce0439db3650b11d9d808dfe6e5380cec1000000006a4730440220333ff2b803ab7c01100d65d57d38455d4f7d7f0c2bf83d638242b5b64ab14b0102202db5805b8e005aeca7f3de4540d0b00c513d8a475f22787b3317442ae720500901210233638afa905e549afb53febc4aeb23dee9e906a38de894f0352c1e322a94c460ffffffff5d8c52cb42cbb51e43b6c8a40444a12e38d84739b66033d6db5bd97d12190c0c000000006c49304602210084a4678cfa9e7b338150932537e83efd5a461876b02788ba1652a08793ae53b4022100f10792aa0b235e51f7c23dca49205ace5c5a912a3f7685ce932d081e6f42a19901210233638afa905e549afb53febc4aeb23dee9e906a38de894f0352c1e322a94c460ffffffff7aa2b162938bd59068061d8b6bc65c30da37d3edf66517bdcb8736e457aaf6bf000000006b483045022100ae57759407f72ef6fc771a064b27ddfe09b3df076be1ccb67db324ad8d623c5d02200cb4cdb8486746ffb8c8d4f8c426f936919564200fe2488db164061bc601402501210233638afa905e549afb53febc4aeb23dee9e906a38de894f0352c1e322a94c460fffffffff6dee530fc0390bfa94c28c477506f1b844650a63db4a7cff84f37c1385cc519000000006b483045022100ace7b74472bbaf00c1389508f537a999b868f19e619bddef53cef27ab510866e02205dfce1090c9c02f34a5527e073f957cc3522cf4b0a1cb821573c6153acd3b49a01210233638afa905e549afb53febc4aeb23dee9e906a38de894f0352c1e322a94c460ffffffffeafbfe205e452a8ca544c5ca63863b55b8acd8ae33a168cb00dd08a953676706000000006b483045022100ee78095f2a28ef90a0fb648623172252a263c22d523703bc0028dbb9181b078702207a295130989598d2573b896e3e8b922410d286f6d86bf82bcc8cc636fda256ce01210233638afa905e549afb53febc4aeb23dee9e906a38de894f0352c1e322a94c460ffffffff9c992ae24d265aa58c8ea89c33235247b9db317b3602d743e57dc3b4aa041282000000006b483045022100c4904faadf3cb3894831f0376c4962135c3573462ecbaa1641ba04c4097087a402201a34693d19f741d7463400386783afd1b9691b146bb59f81fb0f19ef82ba7c9701210233638afa905e549afb53febc4aeb23dee9e906a38de894f0352c1e322a94c460ffffffffd347a768891f6b83ba58c4a32dcc881ebfafd0f16023e1381eba6552cf11ed99000000006b4830450221008ba065070659f3bc8992aef5604434aaba1bfc16b1224b39a4be14bc1deb33d4022048d6f97b7fdeda1c3c626b05df1ea70300c25537c36ec4b200825b0333cda6ad01210233638afa905e549afb53febc4aeb23dee9e906a38de894f0352c1e322a94c460ffffffff89551fd7c7dd51b9a6e4aa9830caa769862c24c11ef453e873e5295f73cd5b59000000006b483045022056c4125114487c393c035be859dcf79fe840d091b1f095964acc9e791490cd1f022100af8effc6008d79428c3cafeeb3d43ab9fff5815f6584583e428c15ffc2d3bd3f01210233638afa905e549afb53febc4aeb23dee9e906a38de894f0352c1e322a94c460ffffffffeed66d4bfd40b0a423babde33b0550c39825bf5a7d6377caf1eddef1c14c9c02000000006b483045022100ead4bec71e9ee792ebb92f378d4592e1b65672b806ae9ed382401882902fd7520220497df514d5d08510fb1666f2a83a468999b204afcc4bd938a18eca1f1747e7f701210233638afa905e549afb53febc4aeb23dee9e906a38de894f0352c1e322a94c460ffffffff016072afad0c0000001976a91447d35ca845a6a11484e367e6be43f0d23636e04288ac00000000\",\"hash\":\"0d507fde9e1b16386f0798128067adbd37cdee98acd27903f59b0877f7f31737\",\"depends\":[],\"fee\":0,\"sigops\":1},{\"data\":\"0100000001a666e23a6e64ddfe0eb28cf39ff4568c05b2f0617528910075946a4a4b6c0688000000006a473044022000f5e4d7490e379127d377786fa6003228d9b259f808eb932c2046248253f02602207b417e8042eaf9cbb9c759124072d4ddb2c478999ef8cefdfdb0f830bfb151210121023bb744351b1f6675ef023125090d5adcf1830a31da89a57a000c08367dadce70ffffffff02805b6d29010000001976a9148a7fd9881a93c13a63310462473a827d022b699a88ac80969800000000001976a914fc9963f591c418921a401373069f4158e295886388ac00000000\",\"hash\":\"623de7ab570540ef659076e6be4f4b8227a47249ea727a8a5afba06a5251e332\",\"depends\":[],\"fee\":0,\"sigops\":2}],\"coinbaseaux\":{\"flags\":\"062f503253482f\"},\"coinbasevalue\":5000000000,\"target\":\"0000007b58890000000000000000000000000000000000000000000000000000\",\"mintime\":1401274780,\"mutable\":[\"time\",\"transactions\",\"prevblock\"],\"noncerange\":\"00000000ffffffff\",\"sigoplimit\":20000,\"sizelimit\":1000000,\"curtime\":1401276010,\"bits\":\"1d7b5889\",\"height\":283723},\"error\":null,\"id\":1}";           
            
            // now init blocktemplate from our json.
            var @object = JsonConvert.DeserializeObject<DaemonResponse<BlockTemplate>>(json);
            var blockTemplate = @object.Result;

            // test the values.
            blockTemplate.Version.Should().Equal(_expected.Version);
            blockTemplate.PreviousBlockHash.Should().Equal(_expected.PreviousBlockHash);

            for(int i=0;i<blockTemplate.Transactions.Length;i++)
            {
                blockTemplate.Transactions[i].Data.Should().Equal(_expected.Transactions[i].Data);
                blockTemplate.Transactions[i].Hash.Should().Equal(_expected.Transactions[i].Hash);
                blockTemplate.Transactions[i].Depends.Should().Equal(_expected.Transactions[i].Depends);
                blockTemplate.Transactions[i].Fee.Should().Equal(_expected.Transactions[i].Fee);
                blockTemplate.Transactions[i].Sigops.Should().Equal(_expected.Transactions[i].Sigops);
            }

            blockTemplate.CoinBaseAux.Flags.Should().Equal(_expected.CoinBaseAux.Flags);
            blockTemplate.Coinbasevalue.Should().Equal(_expected.Coinbasevalue);
            blockTemplate.Target.Should().Equal(_expected.Target);
            blockTemplate.MinTime.Should().Equal(_expected.MinTime);
            blockTemplate.Mutable.Should().Equal(_expected.Mutable);
            blockTemplate.NonceRange.Should().Equal(_expected.NonceRange);
            blockTemplate.SigOpLimit.Should().Equal(_expected.SigOpLimit);
            blockTemplate.SizeLimit.Should().Equal(_expected.SizeLimit);
            blockTemplate.CurTime.Should().Equal(_expected.CurTime);
            blockTemplate.Bits.Should().Equal(_expected.Bits);
            blockTemplate.Height.Should().Equal(_expected.Height);
        }
    }
}
