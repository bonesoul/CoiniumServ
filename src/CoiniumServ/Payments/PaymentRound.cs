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
using CoiniumServ.Persistance.Blocks;

namespace CoiniumServ.Payments
{
    public class PaymentRound:IPaymentRound
    {
        public IFinalizedBlock Block { get; private set; }
        public Dictionary<string, double> Shares { get; private set; }
        public Dictionary<string, decimal> Payouts { get; private set; }


        public PaymentRound(IFinalizedBlock block)
        {
            Block = block;
            Payouts = new Dictionary<string, decimal>();
            Shares = new Dictionary<string, double>();
        }

        public void AddShares(IDictionary<string, double> shares)
        {
            foreach (var pair in shares)
            {
                Shares.Add(pair.Key, pair.Value);
            }

            if(Block.Status == BlockStatus.Confirmed)
                CalculatePayouts();
        }

        private void CalculatePayouts()
        {
            var totalShares = Shares.Sum(pair => pair.Value); // total shares.
            
            // calculate per worker amounts.
            foreach (var pair in Shares)
            {
                var percent = pair.Value/totalShares;
                var workerRewardInSatoshis = (decimal)percent * Block.Reward;

                Payouts.Add(pair.Key, workerRewardInSatoshis);
            }
        }

        public override string ToString()
        {
            return string.Format("Amount: {0}, Block: {1}", Block.Reward, Block);
        }
    }
}
