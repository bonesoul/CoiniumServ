/*
 *   Coinium project - crypto currency pool software - https://github.com/raistlinthewiz/coinium
 *   Copyright (C) 2013 Hüseyin Uslu, Int6 Studios - http://www.coinium.org
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
using AustinHarris.JsonRpc;
using coinium.Net.RPC.Server.Responses;

namespace coinium.Core.Mining.Service
{
    public class MiningService : JsonRpcService
    {
        // nonce counter 
        private const ulong InstanceId = 31;
        private ulong _counter;

        public MiningService()
        {                              
            //Last 5 most-significant bits represents instanceId, the rest is just an iterator of jobs.     
            this._counter = InstanceId << 27;  // basically allows to run two pool-nodes within the same database. (https://github.com/moopless/stratum-mining-litecoin/issues/23#issuecomment-22728564)
        }

        [JsonRpcMethod("mining.subscribe")]
        public SubscribeResponse SubscribeMiner(string miner)
        {
            this._counter++;

            var response = new SubscribeResponse
            {
                ExtraNonce1 = this._counter.ToString("x8"), // Hex-encoded, per-connection unique string which will be used for coinbase serialization later. (http://mining.bitcoin.cz/stratum-mining)
                ExtraNonce2Size = 0x4
            };

            return response;
        }

        [JsonRpcMethod("mining.authorize")]
        public bool AuthorizeMiner(string user, string password)
        {
            return true;
        }
    }
}
