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

using CoiniumServ.Daemon;
using CoiniumServ.Daemon.Responses;
using CoiniumServ.Jobs;
using CoiniumServ.Transactions.Script;
using CoiniumServ.Utils.Extensions;
using Newtonsoft.Json;
using Should.Fluent;
using Xunit;

/*  sample data
    -- create-generation start --
    rpcData: {"version":2,"previousblockhash":"f5f50aa8da33bde3805fe2a56b5f5ab82a2c0ce8597ef97a0abd8348d33ef1b6","transactions":[],"coinbaseaux":{"flags":"062f503253482f"},"coinbasevalue":5000000000,"target":"00000fffff000000000000000000000000000000000000000000000000000000","mintime":1402264399,"mutable":["time","transactions","prevblock"],"noncerange":"00000000ffffffff","sigoplimit":20000,"sizelimit":1000000,"curtime":1402265776,"bits":"1e0fffff","height":294740}
    -- scriptSigPart data --
    -> height: 294740 serialized: 03547f04
    -> coinbase: 062f503253482f hex: 062f503253482f
    -> date: 1402265775319 final:1402265775 serialized: 04afe09453
    scriptSigPart1: 03547f04062f503253482f04afe0945308
    scriptSigPart2: /nodeStratum/ serialized: 0d2f6e6f64655374726174756d2f 
 */

namespace CoiniumServ.Tests.Transactions.Script
{
    public class SignatureScriptTests
    {
        // object mocks.
        private readonly IBlockTemplate _blockTemplate;
        private readonly IExtraNonce _extraNonce;

        public SignatureScriptTests()
        {
            // block template
            const string json = "{\"result\":{\"version\":2,\"previousblockhash\":\"f5f50aa8da33bde3805fe2a56b5f5ab82a2c0ce8597ef97a0abd8348d33ef1b6\",\"transactions\":[],\"coinbaseaux\":{\"flags\":\"062f503253482f\"},\"coinbasevalue\":5000000000,\"target\":\"00000fffff000000000000000000000000000000000000000000000000000000\",\"mintime\":1402264399,\"mutable\":[\"time\",\"transactions\",\"prevblock\"],\"noncerange\":\"00000000ffffffff\",\"sigoplimit\":20000,\"sizelimit\":1000000,\"curtime\":1402265776,\"bits\":\"1e0fffff\",\"height\":294740},\"error\":null,\"id\":1}";
            var @object = JsonConvert.DeserializeObject<DaemonResponse<BlockTemplate>>(json);
            _blockTemplate = @object.Result;

            // extra nonce
            _extraNonce = new ExtraNonce(0);
        }

        [Fact]
        public void SignatureScriptTest()
        {            
            var signatureScript = new SignatureScript(
                _blockTemplate.Height,
                _blockTemplate.CoinBaseAux.Flags,
                1402265775319,
                (byte)_extraNonce.ExtraNoncePlaceholder.Length,
                "/nodeStratum/");

            // test the objects.
            signatureScript.Initial.ToHexString().Should().Equal("03547f04062f503253482f04afe0945308");
            signatureScript.Final.ToHexString().Should().Equal("0d2f6e6f64655374726174756d2f");
        }
    }
}
