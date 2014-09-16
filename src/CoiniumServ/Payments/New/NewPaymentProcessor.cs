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
using CoiniumServ.Persistance.Layers;
using CoiniumServ.Pools;
using Serilog;

namespace CoiniumServ.Payments.New
{
    public class NewPaymentProcessor:INewPaymentProcessor
    {
        private Timer _timer;

        private readonly Stopwatch _stopWatch = new Stopwatch();

        private readonly IPoolConfig _poolConfig;

        private readonly IStorageLayer _storageLayer;

        private readonly IDaemonClient _daemonClient;

        private string _poolAccount = string.Empty;

        private readonly ILogger _logger;

        public NewPaymentProcessor(IPoolConfig poolConfig, IStorageLayer storageLayer, IDaemonClient daemonClient)
        {
            _poolConfig = poolConfig;
            _storageLayer = storageLayer;
            _daemonClient = daemonClient;
            _logger = Log.ForContext<BlockAccounter>().ForContext("Component", poolConfig.Coin.Name);

            if (!_poolConfig.Payments.Enabled) // make sure payments are enabled.
                return;

            if (!ValidatePoolAddress()) // try to validate the pool wallet.
                return; // if we can't, stop the payment processor.

            if (!GetPoolAccount()) // get the pool's account name if any.
                return; // if we can't, stop the payment processor.

            // setup the timer to run calculations.  
            //_timer = new Timer(Run, null, _poolConfig.Payments.Interval * 1000, Timeout.Infinite);
            Run(null);
        }

        private void Run(object state)
        {
            var paymentsToExecute = GetPaymentsToExecute(); // get the pending payments available for execution.            
            var success = ExecutePayments(paymentsToExecute); // try to execute the payments.

            if(success) // if we were able to execute the payments
                CommitTransactions(paymentsToExecute); // commit them to storage layer.
        }

        private IList<IPaymentTransaction> GetPaymentsToExecute()
        {
            var pendingPayments = _storageLayer.GetPendingPayouts(); // get all pending payments.
            var paymentsToExecute = new List<IPaymentTransaction>();  // list of payments to be executed.
             
            foreach (var payment in pendingPayments)
            {
                // query the user for the payment.
                var user = _storageLayer.GetUserById(payment.UserId);

                if (user == null)
                    continue;

                // see if user payout address is directly payable from the pool's main daemon connection
                // which happens when a user connects an XYZ pool and want his payments in XYZ coin.

                var result = _daemonClient.ValidateAddress(user.Address); // does the user have a directly payable address set?

                if (!result.IsValid) // if not skip the payment and let it handled by auto-exchange module.
                    continue;

                paymentsToExecute.Add(new PaymentTransaction(user, payment, _poolConfig.Coin.Symbol));
            }

            return paymentsToExecute;
        }

        private bool ExecutePayments(IList<IPaymentTransaction> paymentsToExecute)
        {
            try
            {
                if (paymentsToExecute.Count == 0) // make sure we have payments to execute.
                    return true;

                var payments = paymentsToExecute.ToDictionary(x => x.User.Address, x => x.Payment.Amount); // get dictionary of address<->amount pairs.
                var result = _daemonClient.SendMany(_poolAccount, payments);
                return true;
            }
            catch (RpcException e)
            {
                _logger.Error("An error occured while trying to execute payment; {0}", e.Message);
                return false;
            }
        }

        private void CommitTransactions(IList<IPaymentTransaction> executedPayments)
        {
            if (executedPayments.Count == 0) // make sure we have payments to execute.
                return;

            foreach (var entry in executedPayments)
            {
                entry.Payment.Completed = true; // mark the payout as completed.

            }
        }

        private bool ValidatePoolAddress()
        {
            try
            {
                var result = _daemonClient.ValidateAddress(_poolConfig.Wallet.Adress);

                // make sure the pool central wallet address is valid and belongs to the daemon we are connected to.
                if (result.IsValid && result.IsMine)
                    return true;

                _logger.Error("Halted as daemon we are connected to does not own the pool address: {0:l}.", _poolConfig.Wallet.Adress);
                return false;
            }
            catch (RpcException e)
            {
                _logger.Error("Halted as we can not connect to configured coin daemon: {0:l}", e.Message);
                return false;
            }
        }

        private bool GetPoolAccount()
        {
            try
            {
                _poolAccount = _daemonClient.GetAccount(_poolConfig.Wallet.Adress);
                return true;
            }
            catch (RpcException e)
            {
                _logger.Error("Cannot determine pool's central wallet account: {0:l}", e.Message);
                return false;
            }
        }
    }
}
