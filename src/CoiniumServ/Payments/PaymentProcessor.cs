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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using CoiniumServ.Daemon;
using CoiniumServ.Daemon.Exceptions;
using CoiniumServ.Persistance;
using CoiniumServ.Persistance.Blocks;
using Serilog;

namespace CoiniumServ.Payments
{
    // TODO: needs a cleanup.

    public class PaymentProcessor : IPaymentProcessor
    {
        public bool IsEnabled { get; private set; }

        private readonly IDaemonClient _daemonClient;
        private readonly IStorage _storage;
        private IPaymentConfig _config;
        private Timer _timer;

        private Int32 _precision; // coin's precision.
        private UInt32 _magnitude; // coin's magnitude
        private decimal _paymentThresholdInSatoshis; // minimum amount of satoshis to issue a payment.

        private readonly object _paymentsLock = new object();
        private readonly Stopwatch _stopWatch = new Stopwatch();

        private readonly IWalletConfig _walletConfig;
        private string _poolAccount = string.Empty;

        private readonly ILogger _logger;

        public PaymentProcessor(string pool, IDaemonClient daemonClient, IStorage storage , IWalletConfig walletConfig)
        {
            _daemonClient = daemonClient;
            _storage = storage;
            _walletConfig = walletConfig;
            _logger = Log.ForContext<PaymentProcessor>().ForContext("Component", pool);
        }

        public void Initialize(IPaymentConfig config)
        {
            _config = config;

            IsEnabled = _config.Enabled;

            if (!IsEnabled) 
                return;

            // validate the pool wallet.
            if (!ValidatePoolAddress())
                return;

            // get the pool's account name if any.
            GetPoolAccount();

            // determine the satoshis in the coin.
            if (!DeterminePrecision())
                return;

            // calculate the minimum amount of payments in satoshis.
            _paymentThresholdInSatoshis = (decimal) (_magnitude*config.Minimum);

            // if we reached here, then we can just setup the timer to run payments.  
            _timer = new Timer(RunPayments, null, _config.Interval * 1000, Timeout.Infinite);
        }

        private void RunPayments(object state)
        {
            if (!IsEnabled)
                return;

            lock (_paymentsLock)
            {
                _stopWatch.Start();


                var pendingBlocks = _storage.GetPendingBlocks(); // get all the pending blocks.
                var finalizedBlocks = GetFinalizedBlocks(pendingBlocks);

                var rounds = GetPaymentRounds(finalizedBlocks); // get the list of rounds that should be paid.
                AssignSharesToRounds(rounds); // process the rounds, calculate shares and payouts per rounds.
                var previousBalances = GetPreviousBalances(); // get previous balances of workers.
                var workerBalances = CalculateRewards(rounds, previousBalances); // calculate the payments.               
                ExecutePayments(workerBalances); // execute the payments.
                ProcessRemainingBalances(workerBalances); // process the remaining balances.
                ProcessRounds(rounds); // process the rounds.

                _logger.Information("Payments processed - took {0:0.000} seconds.", (float)_stopWatch.ElapsedMilliseconds / 1000);
                
                _stopWatch.Reset();

                _timer.Change(_config.Interval*1000, Timeout.Infinite); // reset the payments timer.
            }
        }

        private Dictionary<string, double> GetPreviousBalances()
        {
            var previousBalances = _storage.GetPreviousBalances();
            return previousBalances;
        }

        private IList<IWorkerBalance> CalculateRewards(IEnumerable<IPaymentRound> rounds, IDictionary<string, double> previousBalances)
        {
            var workerBalances = new Dictionary<string, IWorkerBalance>();

            // set previous balances
            foreach(var pair in previousBalances)
            {
                if (!workerBalances.ContainsKey(pair.Key))
                    workerBalances.Add(pair.Key, new WorkerBalance(pair.Key, _magnitude));

                workerBalances[pair.Key].SetPreviousBalance(pair.Value);
            }

            // add rewards by rounds
            foreach (var round in rounds) 
            {
                if (round.Block.Status != BlockStatus.Confirmed) // only pay to confirmed rounds.
                    continue;

                foreach (var pair in round.Payouts) // loop through all payouts for the rounds
                {
                    if (!workerBalances.ContainsKey(pair.Key)) // make sure a payout for worker already exists
                        workerBalances.Add(pair.Key, new WorkerBalance(pair.Key, _magnitude));

                    workerBalances[pair.Key].AddReward(pair.Value);
                }
            }

            return workerBalances.Values.ToList();
        }

        private void ExecutePayments(IList<IWorkerBalance> workerBalances)
        {
            var payments = new Dictionary<string, decimal>();

            try
            {
                decimal totalAmountToPay = 0;

                foreach (var balance in workerBalances)
                {
                    if (balance.BalanceInSatoshis >= _paymentThresholdInSatoshis) // if worker's balance exceed's threshold, add him to payment list.
                    {
                        totalAmountToPay += balance.Balance;
                        payments.Add(balance.Worker, balance.Balance);
                    }
                }

                if (totalAmountToPay <= 0)
                {
                    _logger.Information("No pending payments found.");
                    return;
                }

                var result = _daemonClient.SendMany(_poolAccount, payments); // send the payments

                // mark the paid ones.
                foreach (var balance in workerBalances.Where(balance => balance.BalanceInSatoshis >= _paymentThresholdInSatoshis))
                {
                    balance.Paid = true;
                }

                _logger.Information("Paid a total of {0} coins to {1} workers.", totalAmountToPay, workerBalances.Count);
            }
            catch (RpcException e)
            {
                _logger.Error("Payment failed: {0} [{1}] - payouts: {2}.", e.Message, e.Code, payments);
            }
        }

        private void ProcessRemainingBalances(IList<IWorkerBalance> workerBalances)
        {
            _storage.SetRemainingBalances(workerBalances); // commit remaining balances to storage.
        }

        private void ProcessRounds(IEnumerable<IPaymentRound> rounds)
        {
            foreach (var round in rounds)
            {
                if (round.Block.Status == BlockStatus.Pending) // if the block is still pending,
                    continue; // just skip it.

                switch (round.Block.Status)
                {
                    case BlockStatus.Confirmed:
                        _storage.DeleteShares(round); // delete the associated shares.
                        _storage.MoveBlock(round); // move pending block to appropriate place.  
                        break;
                    case BlockStatus.Kicked:
                    case BlockStatus.Orphaned:
                        _storage.MoveSharesToCurrentRound(round); // move shares to current round so the work of miners aren't gone to void.
                        _storage.MoveBlock(round); // move pending block to appropriate place.                      
                        break;
                }
            }
        }   

        private void AssignSharesToRounds(IList<IPaymentRound> rounds)
        {
            // get shares for the rounds.
            var shares = _storage.GetSharesForRounds(rounds);

            // assign shares to the rounds.
            foreach (var round in rounds)
            {
                if (!shares.ContainsKey(round.Block.Height)) 
                    continue;

                round.AddShares(shares[round.Block.Height]);
            }
        }

        private IList<IFinalizedBlock> GetFinalizedBlocks(IEnumerable<IPendingBlock> blocks)
        {
            var finalizedBlocks = new List<IFinalizedBlock>();

            foreach (var block in blocks)
            {
                foreach (var candidate in block.Candidates)
                {
                    CheckCandidates(candidate);
                }

                block.Check();

                if(block.IsFinalized)
                    finalizedBlocks.Add(block.Finalized);
            }

            return finalizedBlocks;
        }

        private IEnumerable<IConfirmedBlock> GetConfirmedBlocks(IEnumerable<IFinalizedBlock> blocks)
        {
            return blocks.OfType<IConfirmedBlock>().ToList();
        }

        private void CheckCandidates(IHashCandidate candidate)
        {
            try
            {
                // get the transaction.
                var transaction = _daemonClient.GetTransaction(candidate.TransactionHash);

                // total amount of coins contained in the block.
                candidate.Amount = transaction.Details.Sum(output => (decimal)output.Amount);

                // get the output transaction that targets pools central wallet.
                var poolOutput = transaction.Details.FirstOrDefault(output => output.Address == _walletConfig.Adress);

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

                    candidate.Reward = (decimal) poolOutput.Amount;
                }
            }
            catch (RpcException)
            {
                candidate.Status = BlockStatus.Kicked; // kick the hash.
                candidate.Reward = 0;
            }            
        }

        private IList<IPaymentRound> GetPaymentRounds(IEnumerable<IFinalizedBlock> blocks)
        {
            var rounds = new List<IPaymentRound>();

            foreach (var block in blocks)
            {
                var round = new PaymentRound(block);
                rounds.Add(round);
            }

            return rounds;            
        }

        private void GetPoolAccount()
        {
            var result = _daemonClient.GetAccount(_walletConfig.Adress);
            _poolAccount = result;
        }

        private bool ValidatePoolAddress()
        {
            var result = _daemonClient.ValidateAddress(_walletConfig.Adress);

            // make sure the pool central wallet address is valid and belongs to the daemon we are connected to.
            if (result.IsValid && result.IsMine)
                return true;

            _logger.Error("Halted as daemon we are connected to does not own the pool address: {0:l}.", _walletConfig.Adress);
            return false;
        }

        private bool DeterminePrecision()
        {
            var json = string.Empty;

            try
            {
                json = _daemonClient.MakeRawRequest("getbalance", DaemonClient.EmptyString);

                // we want a raw request
                var satoshis = json.Split(new[] { "result\":" }, StringSplitOptions.None)[1].Split(',')[0].Split('.')[1];

                _precision = satoshis.Length;
                _magnitude = 1;

                for (int i = 1; i <= _precision; i++)
                {
                    _magnitude *= 10;
                }

                return true;
            }
            catch (RpcException e)
            {
                _logger.Error("Halted as getbalance call failed: {0}.", e.Message);
                return false;
            }
            catch (Exception)
            {
                _logger.Error("Halted as we can not determine satoshis in a coin - failed parsing: {0}", json);
                return false;
            }
        }
    }
}
