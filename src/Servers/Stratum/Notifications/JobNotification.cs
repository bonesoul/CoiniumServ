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

using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Coinium.Servers.Stratum.Notifications
{
    [JsonArray]
    public class JobNotification : IEnumerable<object>
    {
        /// <summary>
        /// ID of the job. Use this ID while submitting share generated from this job.
        /// </summary>
        [JsonIgnore]   
        public string Id;

        /// <summary>
        /// Hash of previous block.
        /// </summary>
        [JsonIgnore]   
        public string PreviousBlockHash;

        /// <summary>
        /// Initial part of coinbase transaction.
        /// <remarks>The miner inserts ExtraNonce1 and ExtraNonce2 after this section of the coinbase. (https://www.btcguild.com/new_protocol.php)</remarks>
        /// </summary>
        [JsonIgnore]
        public string CoinbaseInitial;

        /// <summary>
        /// Final part of coinbase transaction.
        /// <remarks>The miner appends this after the first part of the coinbase and the two ExtraNonce values. (https://www.btcguild.com/new_protocol.php)</remarks>
        /// </summary>
        [JsonIgnore]
        public string CoinbaseFinal;

        /// <summary>
        /// List of hashes, will be used for calculation of merkle root. 
        /// <remarks>This is not a list of all transactions, it only contains prepared hashes of steps of merkle tree algorithm. Please read some materials (http://en.wikipedia.org/wiki/Hash_tree) for understanding how merkle trees calculation works. (http://mining.bitcoin.cz/stratum-mining)</remarks>
        /// <remarks>The coinbase transaction is hashed against the merkle branches to build the final merkle root.</remarks>
        /// </summary>
        [JsonIgnore]
        public List<object> MerkleBranches;

        /// <summary>
        /// Coin's block version.
        /// </summary>
        [JsonIgnore]
        public string BlockVersion;

        /// <summary>
        /// Encoded current network difficulty.
        /// </summary>
        [JsonIgnore]
        public string NetworkDifficulty;

        /// <summary>
        /// The current time. nTime rolling should be supported, but should not increase faster than actual time.
        /// </summary>
        [JsonIgnore]
        public string nTime;

        /// <summary>
        /// When true, server indicates that submitting shares from previous jobs don't have a sense and such shares will be rejected. When this flag is set, miner should also drop all previous jobs, so job_ids can be eventually rotated. (http://mining.bitcoin.cz/stratum-mining)
        /// <remarks>f true, miners should abort their current work and immediately use the new job. If false, they can still use the current job, but should move to the new one after exhausting the current nonce range. (https://www.btcguild.com/new_protocol.php)</remarks>
        /// </summary>
        [JsonIgnore]
        public bool CleanJobs;

        public IEnumerator<object> GetEnumerator()
        {
            var data = new List<object>
            {
                this.Id,
                this.PreviousBlockHash,
                this.CoinbaseInitial,
                this.CoinbaseFinal,
                this.MerkleBranches,
                this.BlockVersion,
                this.NetworkDifficulty,
                this.nTime,
                this.CleanJobs
            };

            return data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
