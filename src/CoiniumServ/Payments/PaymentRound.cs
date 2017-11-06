#region License
// 
//     MIT License
//
//     CoiniumServ - Crypto Currency Mining Pool Server Software
//     Copyright (C) 2013 - 2017, CoiniumServ Project
//     Hüseyin Uslu, shalafiraistlin at gmail dot com
//     https://github.com/bonesoul/CoiniumServ
// 
//     Permission is hereby granted, free of charge, to any person obtaining a copy
//     of this software and associated documentation files (the "Software"), to deal
//     in the Software without restriction, including without limitation the rights
//     to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//     copies of the Software, and to permit persons to whom the Software is
//     furnished to do so, subject to the following conditions:
//     
//     The above copyright notice and this permission notice shall be included in all
//     copies or substantial portions of the Software.
//     
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//     FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//     LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//     OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//     SOFTWARE.
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
