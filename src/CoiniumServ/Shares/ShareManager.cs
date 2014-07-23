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
using CoiniumServ.Daemon;
using CoiniumServ.Jobs.Tracker;
using CoiniumServ.Persistance;
using CoiniumServ.Server.Mining.Stratum;
using CoiniumServ.Server.Mining.Stratum.Errors;
using CoiniumServ.Server.Mining.Vanilla;
using CoiniumServ.Utils.Extensions;
using Serilog;

namespace CoiniumServ.Shares
{
    public class ShareManager : IShareManager
    {
        public event EventHandler BlockFound;

        public event EventHandler ShareSubmitted;

        private readonly IJobTracker _jobTracker;

        private readonly IDaemonClient _daemonClient;

        private readonly IStorage _storage;

        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShareManager" /> class.
        /// </summary>
        /// <param name="pool"></param>
        /// <param name="daemonClient"></param>
        /// <param name="jobTracker"></param>
        /// <param name="storage"></param>
        public ShareManager(string pool, IDaemonClient daemonClient, IJobTracker jobTracker, IStorage storage)
        {
            _daemonClient = daemonClient;
            _jobTracker = jobTracker;
            _storage = storage;
            _logger = Log.ForContext<ShareManager>().ForContext("Component", pool);
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
        public IShare ProcessShare(IStratumMiner miner, string jobId, string extraNonce2, string nTimeString, string nonceString)
        {
            // check if the job exists
            var id = Convert.ToUInt64(jobId, 16);
            var job = _jobTracker.Get(id);

            // create the share
            var share = new Share(miner, id, job, extraNonce2, nTimeString, nonceString);

            if (share.IsValid)
                HandleValidShare(share);
            else
                HandleInvalidShare(share);

            OnShareSubmitted(new ShareEventArgs(miner));  // notify the listeners about the share.

            return share;
        }

        public IShare ProcessShare(IVanillaMiner miner, string data)
        {
            throw new NotImplementedException();
        }

        private void HandleValidShare(IShare share)
        {
            var miner = (IStratumMiner) share.Miner;
            miner.ValidShares++;

            _storage.AddShare(share); // commit the share.

            if (!share.IsBlockCandidate)
            {
                _logger.Debug("Share accepted at {0:0.00}/{1} by miner {2:l}", share.Difficulty, miner.Difficulty, miner.Username);
                return;
            }

            // if share contains a block candicate
            _logger.Debug("Share with block candidate [{0}] accepted at {1:0.00}/{2} by miner {3:l}", share.Height, share.Difficulty, miner.Difficulty, miner.Username);

            var accepted = SubmitBlock(share); // submit block to daemon.

            if (!accepted) 
                return;

            OnBlockFound(EventArgs.Empty); // notify the listeners about the new block.

            _storage.AddBlock(share); // commit the block.
        }

        private void HandleInvalidShare(IShare share)
        {
            var miner = (IStratumMiner)share.Miner;
            miner.InvalidShares++;

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
                    JsonRpcContext.SetException(new JobNotFoundError(share.JobId));
                    break;
                case ShareError.LowDifficultyShare:
                    JsonRpcContext.SetException(new LowDifficultyShare(share.Difficulty));
                    break;
                case ShareError.NTimeOutOfRange:
                    JsonRpcContext.SetException(new OtherError("nTime out of range"));
                    break;
            }

            _logger.Debug("Share rejected at {0:0.00}/{1} by miner {2:l}", share.Difficulty, miner.Difficulty, miner.Username);            
        }

        private bool SubmitBlock(IShare share)
        {
            try
            {
                _daemonClient.SubmitBlock(share.BlockHex.ToHexString());
                var isAccepted = CheckIfBlockAccepted(share);

                _logger.Information(
                    isAccepted
                        ? "Found block [{0}] with hash: {1:l}"
                        : "Submitted block [{0}] but got denied: {1:l}",
                    share.Height, share.BlockHash.ToHexString());

                return isAccepted;
            }
            catch (Exception e)
            {
                _logger.Error(e, "Submit block failed - height: {0}, hash: {1:l}", share.Height, share.BlockHash);
                return false;
            }
        }

        private bool CheckIfBlockAccepted(IShare share)
        {
            try
            {
                var block = _daemonClient.GetBlock(share.BlockHash.ToHexString()); // query the block.
                share.SetFoundBlock(block); // assign the block to share.
                return true;
            }
            catch (Exception e)
            {
                _logger.Error(e, "Get block failed - height: {0}, hash: {1:l}", share.Height, share.BlockHash);
                return false;
            }
        }

        private void OnBlockFound(EventArgs e)
        {
            var handler = BlockFound;

            if (handler != null)
                handler(this, e);
        }

        private void OnShareSubmitted(EventArgs e)
        {
            var handler = ShareSubmitted;

            if (handler != null)
                handler(this, e);
        }
    }
}
