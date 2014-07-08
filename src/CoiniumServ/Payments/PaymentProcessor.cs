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
using Coinium.Daemon;
using Coinium.Daemon.Exceptions;
using Coinium.Persistance;
using Serilog;

namespace Coinium.Payments
{
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

        private const string PoolAddress = "n3Mvrshbf4fMoHzWZkDVbhhx4BLZCcU9oY";
        private string _poolAccount = string.Empty;

        public PaymentProcessor(IDaemonClient daemonClient, IStorage storage)
        {
            _daemonClient = daemonClient;
            _storage = storage;
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
            _paymentThresholdInSatoshis = _magnitude*config.Minimum;

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

                var blocks = _storage.GetPendingBlocks(); // get all the pending blocks.
                CheckBlocks(blocks);
                var rounds = GetPaymentRounds(blocks); // get the list of rounds that should be paid.
                AssignSharesToRounds(rounds); // process the rounds, calculate shares and payouts per rounds.
                var previousBalances = GetPreviousBalances(); // get previous balances of workers.
                var workerBalances = CalculateRewards(rounds, previousBalances); // calculate the payments.               
                ExecutePayments(workerBalances); // execute the payments.

                ProcessRemainingBalances(workerBalances); // process the remaining balances.

                ProcessRounds(rounds); // process the rounds.

                Log.ForContext<PaymentProcessor>().Information("Payments processed - took {0:0.000} seconds.", (float)_stopWatch.ElapsedMilliseconds/1000);
                
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
                if (round.Block.Status != PersistedBlockStatus.Confirmed) // only pay to confirmed rounds.
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
                    Log.ForContext<PaymentProcessor>().Information("No pending payments found.");
                    return;
                }

                var result = _daemonClient.SendMany(_poolAccount, payments); // send the payments

                // mark the paid ones.
                foreach (var balance in workerBalances.Where(balance => balance.BalanceInSatoshis >= _paymentThresholdInSatoshis))
                {
                    balance.Paid = true;
                }

                Log.ForContext<PaymentProcessor>().Information("Paid a total of {0} coins to {1} workers.", totalAmountToPay, workerBalances.Count);
            }
            catch (RpcException e)
            {
                Log.ForContext<PaymentProcessor>().Error("Payment failed: {0} [{1}] - payouts: {2}.", e.Message, e.Code, payments);
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
                if (round.Block.OutstandingHashes.Status == PersistedBlockStatus.Pending) // if the block is still pending,
                    continue; // just skip it.

                switch (round.Block.OutstandingHashes.Status)
                {
                    case PersistedBlockStatus.Confirmed:
                        _storage.DeleteShares(round); // delete the associated shares.
                        _storage.MoveBlock(round); // move pending block to appropriate place.  
                        break;
                    case PersistedBlockStatus.Kicked:
                    case PersistedBlockStatus.Orphan:
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

        private void CheckBlockHashes(IPersistedBlockHashes hashes)
        {
            try
            {
                // get the transaction.
                var transaction = _daemonClient.GetTransaction(hashes.TransactionHash);

                // get the output transaction that targets pools central wallet.
                var poolOutput = transaction.Details.FirstOrDefault(output => output.Address == PoolAddress);

                // make sure output for the pool central wallet exists
                if (poolOutput == null)
                {
                    hashes.Status = PersistedBlockStatus.Kicked; // kick the hash.
                    hashes.Reward = 0;
                }
                else
                {
                    switch (poolOutput.Category)
                    {
                        case "immature":
                            hashes.Status = PersistedBlockStatus.Pending;
                            break;
                        case "orphan":
                            hashes.Status = PersistedBlockStatus.Orphan;
                            break;
                        case "generate":
                            hashes.Status = PersistedBlockStatus.Confirmed;
                            break;
                        default:
                            hashes.Status = PersistedBlockStatus.Pending;
                            break;
                    }

                    hashes.Reward = (decimal) poolOutput.Amount;
                }
            }
            catch (RpcException)
            {
                hashes.Status = PersistedBlockStatus.Kicked; // kick the hash.
                hashes.Reward = 0;
            }            
        }

        private void CheckBlocks(IEnumerable<IPersistedBlock> blocks)
        {
            foreach (var block in blocks)
            {
                foreach (var hashes in block.Hashes)
                {
                    CheckBlockHashes(hashes);
                }
            }
        }

        private IList<IPaymentRound> GetPaymentRounds(IEnumerable<IPersistedBlock> blocks)
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
            var result = _daemonClient.GetAccount(PoolAddress);
            _poolAccount = result;
        }

        private bool ValidatePoolAddress()
        {
            var result = _daemonClient.ValidateAddress(PoolAddress);

            // make sure the pool central wallet address is valid and belongs to the daemon we are connected to.
            if (result.IsValid && result.IsMine)
                return true;

            Log.ForContext<PaymentProcessor>().Error("Halted as daemon we are connected to does not own the pool address: {0:l}.", PoolAddress);
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
                Log.ForContext<PaymentProcessor>().Error("Halted as getbalance call failed: {0}.", e.Message);
                return false;
            }
            catch (Exception e)
            {
                Log.ForContext<PaymentProcessor>().Error("Halted as we can not determine satoshis in a coin - failed parsing: {0}", json);
                return false;
            }
        }
    }
}
