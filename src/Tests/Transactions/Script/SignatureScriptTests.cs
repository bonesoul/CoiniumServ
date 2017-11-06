#region License
// 
//     MIT License
//
//     CoiniumServ - Crypto Currency Mining Pool Server Software
//     Copyright (C) 2013 - 2017, CoiniumServ Project
//     Hüseyin Uslu, shalafiraistlin at gmail dot com
//     https://github.com/bonesoul/CoiniumServ
// 
//     Permission is hereby granted, free of charge, to any person obtaining a copy
//     of this software and associated documentation files (the "Software"), to deal
//     in the Software without restriction, including without limitation the rights
//     to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//     copies of the Software, and to permit persons to whom the Software is
//     furnished to do so, subject to the following conditions:
//     
//     The above copyright notice and this permission notice shall be included in all
//     copies or substantial portions of the Software.
//     
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//     FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//     LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//     OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//     SOFTWARE.
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
