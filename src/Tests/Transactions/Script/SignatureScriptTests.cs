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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coinium.Coin.Daemon;
using Coinium.Coin.Daemon.Responses;
using Coinium.Mining.Jobs;
using Coinium.Transactions.Script;
using Newtonsoft.Json;
using Should.Fluent;
using Xunit;

namespace Tests.Transactions.Script
{
    public class SignatureScriptTests
    {
        [Fact]
        public void SignatureScriptTest()
        {
            /*  sample data
            -- create-generation start --
            rpcData: {"version":2,"previousblockhash":"f5f50aa8da33bde3805fe2a56b5f5ab82a2c0ce8597ef97a0abd8348d33ef1b6","transactions":[],"coinbaseaux":{"flags":"062f503253482f"},"coinbasevalue":5000000000,"target":"00000fffff000000000000000000000000000000000000000000000000000000","mintime":1402264399,"mutable":["time","transactions","prevblock"],"noncerange":"00000000ffffffff","sigoplimit":20000,"sizelimit":1000000,"curtime":1402265776,"bits":"1e0fffff","height":294740}
            -- scriptSigPart data --
            -> height: 294740 serialized: 03547f04
            -> coinbase: 062f503253482f hex: 062f503253482f
            -> date: 1402265775319 final:1402265775 serialized: 04afe09453
            scriptSigPart1: 03547f04062f503253482f04afe0945308
            scriptSigPart2: /nodeStratum/ serialized: 0d2f6e6f64655374726174756d2f */

            // block template json
            const string json = "{\"result\":{\"version\":2,\"previousblockhash\":\"f5f50aa8da33bde3805fe2a56b5f5ab82a2c0ce8597ef97a0abd8348d33ef1b6\",\"transactions\":[],\"coinbaseaux\":{\"flags\":\"062f503253482f\"},\"coinbasevalue\":5000000000,\"target\":\"00000fffff000000000000000000000000000000000000000000000000000000\",\"mintime\":1402264399,\"mutable\":[\"time\",\"transactions\",\"prevblock\"],\"noncerange\":\"00000000ffffffff\",\"sigoplimit\":20000,\"sizelimit\":1000000,\"curtime\":1402265776,\"bits\":\"1e0fffff\",\"height\":294740},\"error\":null,\"id\":1}";

            // now init blocktemplate from our json.
            var @object = JsonConvert.DeserializeObject<DaemonResponse<BlockTemplate>>(json);
            var blockTemplate = @object.Result;

            // init required objects.
            var extraNonce = new ExtraNonce(0);   

            var signatureScript = new SignatureScript(
                blockTemplate.Height,
                blockTemplate.CoinBaseAux.Flags,
                1402265775319,
                (byte) extraNonce.ExtraNoncePlaceholder.Length,
                "/nodeStratum/");

            // test the objects.
            signatureScript.Initial.Should().Equal(new byte[] { 0x03, 0x54, 0x7f, 0x04, 0x06, 0x2f, 0x50, 0x32, 0x53, 0x48, 0x2f, 0x04, 0xaf, 0xe0, 0x94, 0x53, 0x08 });
            signatureScript.Final.Should().Equal(new byte[] { 0x0d, 0x2f, 0x6e, 0x6f, 0x64, 0x65, 0x53, 0x74, 0x72, 0x61, 0x74, 0x75, 0x6d, 0x2f });
        }
    }
}
