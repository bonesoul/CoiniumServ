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

        /// <summary>
        /// Subscribes a Miner to allow it to recieve work to begin hashing and submitting shares.
        /// </summary>
        /// <param name="signature">Miner Connection</param>
        [JsonRpcMethod("mining.subscribe")]
        public SubscribeResponse SubscribeMiner(string signature)
        {
            var miner = (Miner)(JsonRpcContext.Current().Value);

            this._counter++;

            var response = new SubscribeResponse
            {
                ExtraNonce1 = this._counter.ToString("x8"), // Hex-encoded, per-connection unique string which will be used for coinbase serialization later. (http://mining.bitcoin.cz/stratum-mining)
                ExtraNonce2Size = 0x4
            };

            miner.Subscribe();

            return response;
        }

        /// <summary>
        /// Authorise a miner based on their username and password
        /// </summary>
        /// <param name="user">Worker Username (e.g. "coinium.1").</param>
        /// <param name="password">Worker Password (e.g. "x").</param>
        [JsonRpcMethod("mining.authorize")]
        public bool AuthorizeMiner(string user, string password)
        {
            var miner = (Miner)(JsonRpcContext.Current().Value);
            return miner.Authenticate(user, password);
        }

        /// <summary>
        /// Allows a miner to submit the work they have done 
        /// </summary>
        /// <param name="user">Worker Username.</param>
        /// <param name="jobId">Job ID(Should be unique per Job to ensure that share diff is recorded properly) </param>
        /// <param name="extronance2">Hex-encoded big-endian extranonce2, length depends on extranonce2_size from mining.notify</param>
        /// <param name="ntime"> UNIX timestamp (32bit integer, big-endian, hex-encoded), must be >= ntime provided by mining,notify and <= current time'</param>
        /// <param name="nonce"> 32bit integer hex-encoded, big-endian </param>
        [JsonRpcMethod("mining.submit")]
        public bool SubmitMiner(string user, string jobId, string extronance2, string ntime, string nonce)
        {
            return true;
        }
    }
}
