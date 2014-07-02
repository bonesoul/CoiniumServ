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
using System;
using AustinHarris.JsonRpc;
using Coinium.Daemon;
using Coinium.Mining.Jobs.Tracker;
using Coinium.Persistance;
using Coinium.Server.Stratum;
using Coinium.Server.Stratum.Errors;
using Coinium.Server.Vanilla;
using Coinium.Utils.Extensions;
using Serilog;

namespace Coinium.Mining.Shares
{
    public class ShareManager : IShareManager
    {
        public event EventHandler BlockFound;

        private readonly IJobTracker _jobTracker;

        private readonly IDaemonClient _daemonClient;

        private readonly IStorage _storage;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShareManager" /> class.
        /// </summary>
        /// <param name="daemonClient"></param>
        /// <param name="jobTracker"></param>
        /// <param name="storage"></param>
        public ShareManager(IDaemonClient daemonClient, IJobTracker jobTracker, IStorage storage)
        {
            _daemonClient = daemonClient;
            _jobTracker = jobTracker;
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
            var job = _jobTracker.Get(id);

            // create the share
            var share = new Share(miner, id, job, extraNonce2, nTimeString, nonceString);

            if (share.IsValid)
            {               
                _storage.CommitShare(share);

                if (share.IsBlockCandidate)
                {
                    Log.ForContext<ShareManager>().Information("Share with block candidate [{0}] accepted at {1:0.00}/{2} by miner {3}.", share.Height, share.Difficulty, miner.Difficulty, miner.Username);

                    var success = SubmitBlock(share); // submit block to daemon.
                    _storage.CommitBlock(share); // commit the block.

                    // TODO: notify back job manager using an event so he can create a new job.
                }
                else
                    Log.ForContext<ShareManager>().Information("Share accepted at {0:0.00}/{1} by miner {2}.", share.Difficulty, miner.Difficulty, miner.Username);
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

                Log.ForContext<ShareManager>().Information("Share rejected at {0:0.00}/{1} by miner {2}.", share.Difficulty, miner.Difficulty, miner.Username);
            }


            return share;
        }

        public IShare ProcessShare(VanillaMiner miner, string data)
        {
            throw new NotImplementedException();
        }

        private bool SubmitBlock(Share share)
        {
            try
            {
                _daemonClient.SubmitBlock(share.BlockHex.ToHexString());
                var isAccepted = CheckIfBlockAccepted(share);

                Log.ForContext<ShareManager>()
                    .Information(
                        isAccepted
                            ? "Found block [{0}] with hash: {1}."
                            : "Submitted block [{0}] but got denied: {1}.", 
                            share.Height, share.BlockHash.ToHexString());

                if(isAccepted)
                    OnBlockFound(EventArgs.Empty); // notify the listeners about the new block.

                return isAccepted;
            }
            catch (Exception e)
            {
                Log.ForContext<ShareManager>().Error(e, "Submit block failed - height: {0}, hash: {1}", share.Height, share.BlockHash);
                return false;
            }
        }

        private bool CheckIfBlockAccepted(Share share)
        {
            try
            {
                var block = _daemonClient.GetBlock(share.BlockHash.ToHexString()); // query the block.
                share.SetFoundBlock(block); // assign the block to share.
                return true;
            }
            catch (Exception e)
            {
                Log.ForContext<ShareManager>().Error(e, "Get block failed - height: {0}, hash: {1}", share.Height, share.BlockHash);
                return false;
            }
        }

        protected virtual void OnBlockFound(EventArgs e)
        {
            var handler = BlockFound;

            if (handler != null)
                handler(this, e);
        }
    }
}
