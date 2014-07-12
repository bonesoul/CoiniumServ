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
using System.Linq;
using System.Threading;
using Coinium.Daemon;
using Coinium.Daemon.Exceptions;
using Coinium.Persistance;
using Coinium.Persistance.Blocks;

namespace Coinium.Mining.Pools.Statistics
{
    public class BlockStatistics : IBlockStatistics
    {
        public IEnumerable<IPersistedBlock> Latest { get; private set; }

        public int Pending { get; private set; }
        public int Confirmed { get; private set; }
        public int Orphaned { get; private set; }

        public int Total
        {
            get { return Pending + Confirmed + Orphaned; }
        }

        private readonly IStorage _storage;
        private readonly IDaemonClient _daemonClient;

        private readonly Timer _timer;
        private const int TimerExpiration = 10;

        private const string PoolAddress = "n3Mvrshbf4fMoHzWZkDVbhhx4BLZCcU9oY";

        public BlockStatistics(IDaemonClient daemonClient, IStorage storage)
        {
            _daemonClient = daemonClient;
            _storage = storage;

            _timer = new Timer(Recache, null, Timeout.Infinite, Timeout.Infinite); // create the timer as disabled.
            Recache(null); // recache data initially.
        }

        private void Recache(object state)
        {
            // get block statistics.
            var blockCounts = _storage.GetBlockCounts();

            // read block stats.
            Pending = blockCounts.ContainsKey("pending") ? blockCounts["pending"] : 0;
            Confirmed = blockCounts.ContainsKey("confirmed") ? blockCounts["confirmed"] : 0;
            Orphaned = blockCounts.ContainsKey("orphaned") ? blockCounts["orphaned"] : 0;

            // read blocks            
            Latest = _storage.GetAllBlocks().OrderByDescending(x => x.Key).Take(20).Select(item=> item.Value).ToList();
            CheckBlocks(Latest);

            // reset the recache timer.
            _timer.Change(TimerExpiration * 1000, Timeout.Infinite);
        }

        // TODO: duplicate code from payment processor.
        private void CheckBlocks(IEnumerable <IPersistedBlock> blocks)
        {
            //foreach (var block in blocks)
            //{
            //    foreach (var hashes in block)
            //    {
            //        CheckBlockHashes(hashes);
            //    }
            //}
        }

        // TODO: duplicate code from payment processor.
        private void CheckBlockHashes(IHashCandidate candidate)
        {
            try
            {
                // get the transaction.
                var transaction = _daemonClient.GetTransaction(candidate.TransactionHash);

                // total amount of coins contained in the block.
                candidate.Amount = transaction.Details.Sum(output => (decimal)output.Amount);

                // get the output transaction that targets pools central wallet.
                var poolOutput = transaction.Details.FirstOrDefault(output => output.Address == PoolAddress);

                // make sure output for the pool central wallet exists
                if (poolOutput == null)
                {
                    candidate.Status = BlockStatus.Kicked; // kick the hash.
                    candidate.Reward = 0;
                }
                else
                {
                    switch (poolOutput.Category)
                    {
                        case "immature":
                            candidate.Status = BlockStatus.Pending;
                            break;
                        case "orphan":
                            candidate.Status = BlockStatus.Orphaned;
                            break;
                        case "generate":
                            candidate.Status = BlockStatus.Confirmed;
                            break;
                        default:
                            candidate.Status = BlockStatus.Pending;
                            break;
                    }

                    candidate.Reward = (decimal)poolOutput.Amount;
                }
            }
            catch (RpcException)
            {
                candidate.Status = BlockStatus.Kicked; // kick the candidate.
                candidate.Reward = 0;
            }
        }
    }
}
