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
using CoiniumServ.Accounts;
using CoiniumServ.Persistance.Blocks;
using CoiniumServ.Persistance.Layers;

namespace CoiniumServ.Payments
{
    public class PaymentRound : IPaymentRound
    {
        public IPersistedBlock Block { get; private set; }
        public IList<IPayment> Payments { get; private set; }

        private readonly IDictionary<string, double> _shares;

        private readonly IStorageLayer _storageLayer;

        private readonly IAccountManager _accountManager;

        public PaymentRound(IPersistedBlock block, IStorageLayer storageLayer, IAccountManager accountManager)
        {
            Block = block;
            _storageLayer = storageLayer;
            _accountManager = accountManager;

            Payments = new List<IPayment>();
            _shares = _storageLayer.GetShares(Block); // load the shares for the round.
            CalculatePayments(); // calculate the per-user payments.
        }

        private void CalculatePayments()
        {
            // find total shares within the round.
            var totalShares = _shares.Sum(pair => pair.Value);

            // loop through user shares and calculate the payouts.
            foreach (var pair in _shares)
            {
                var percent = pair.Value / totalShares;
                var amount = (decimal)percent * Block.Reward;

                // get the user id for the payment.
                var user = _accountManager.GetAccountByUsername(pair.Key);

                // if we can't find a user for the given username, just skip.
                if (user == null)
                    continue;

                Payments.Add(new Payment(Block, user.Id, amount));
            }

            // mark the block as accounted
            Block.Accounted = true;
        }
    }
}
