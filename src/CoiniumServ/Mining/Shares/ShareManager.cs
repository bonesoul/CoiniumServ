#region License
// 
//     CoiniumServ - Crypto Currency Mining Pool Server Software
//     Copyright (C) 2013 - 2014, CoiniumServ Project - http://www.coinium.org
//     https://github.com/CoiniumServ/CoiniumServ
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
using System;
using AustinHarris.JsonRpc;
using Coinium.Daemon;
using Coinium.Mining.Jobs;
using Coinium.Persistance;
using Coinium.Server.Stratum;
using Coinium.Server.Stratum.Errors;
using Coinium.Utils.Extensions;
using Serilog;

namespace Coinium.Mining.Shares
{
    public class ShareManager : IShareManager
    {
        private readonly IJobManager _jobManager;

        private readonly IDaemonClient _daemonClient;

        private readonly IStorage _storage;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShareManager" /> class.
        /// </summary>
        /// <param name="jobManager">The job manager.</param>
        /// <param name="daemonClient"></param>
        public ShareManager(IJobManager jobManager, IDaemonClient daemonClient, IStorage storage)
        {
            _jobManager = jobManager;
            _daemonClient = daemonClient;
            _storage = storage;
        }

        /// <summary>
        /// Processes the share.
        /// </summary>
        /// <param name="miner">The miner.</param>
        /// <param name="jobId">The job identifier.</param>
        /// <param name="extraNonce2">The extra nonce2.</param>
        /// <param name="nTimeString">The n time string.</param>
        /// <param name="nonceString">The nonce string.</param>
        /// <returns></returns>
        public IShare ProcessShare(StratumMiner miner, string jobId, string extraNonce2, string nTimeString, string nonceString)
        {
            // check if the job exists
            var id = Convert.ToUInt64(jobId, 16);
            var job = _jobManager.GetJob(id);

            // create the share
            var share = new Share(miner, id, job, _jobManager.ExtraNonce.Current, extraNonce2, nTimeString, nonceString);


            if (share.IsValid)
            {               
                _storage.CommitShare(share);

                if (share.IsCandidate)
                {
                    Log.ForContext<ShareManager>().Information("Share with block candidate [{0}] accepted at {1}/{2} by miner {3}.", share.Height, share.Job.Difficulty, share.Difficulty, miner.Username);

                    var success = SubmitBlock(share); // submit block to daemon

                    if (success)
                        _storage.CommitBlock(share);
                }
                else
                    Log.ForContext<ShareManager>().Information("Share accepted at {0}/{1} by miner {2}.", share.Job.Difficulty, share.Difficulty, miner.Username);
            }
            else
            {
                switch (share.Error)
                {
                    case ShareError.DuplicateShare:
                        JsonRpcContext.SetException(new DuplicateShareError(share.Nonce));
                        break;
                    case ShareError.IncorrectExtraNonce2Size:
                        JsonRpcContext.SetException(new OtherError("Incorrect extranonce2 size"));
                        break;
                    case ShareError.IncorrectNTimeSize:
                        JsonRpcContext.SetException(new OtherError("Incorrect nTime size"));
                        break;
                    case ShareError.IncorrectNonceSize:
                        JsonRpcContext.SetException(new OtherError("Incorrect nonce size"));
                        break;
                    case ShareError.JobNotFound:
                        JsonRpcContext.SetException(new JobNotFoundError(share.Job.Id));
                        break;
                    case ShareError.LowDifficultyShare:
                        JsonRpcContext.SetException(new LowDifficultyShare(share.Difficulty));
                        break;
                    case ShareError.NTimeOutOfRange:
                        JsonRpcContext.SetException(new OtherError("nTime out of range"));
                        break;
                }

                Log.ForContext<ShareManager>().Information("Share rejected at {0}/{1} by miner {2}.", share.Job.Difficulty, share.Difficulty, miner.Username);
            }


            return share;
        }

        private bool SubmitBlock(Share share)
        {
            try
            {
                _daemonClient.SubmitBlock(share.BlockHex.ToHexString());
                Log.ForContext<ShareManager>().Information("Submitted block [{0}] using submitblock: {1}.", share.Height, share.BlockHash.ToHexString());
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
