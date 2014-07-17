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

namespace CoiniumServ.Payments
{
    public class WorkerBalance:IWorkerBalance
    {
        public string Worker { get; private set; }
        public decimal Balance { get; private set; }
        public decimal BalanceInSatoshis { get; private set; }
        public decimal PreviousBalance { get; private set; }
        public decimal Reward { get; private set; }
        public bool Paid { get; set; }

        private readonly UInt32 _satoshiMagnitude;

        public WorkerBalance(string worker, UInt32 satoshiMagnitude)
        {
            Worker = worker;
            _satoshiMagnitude = satoshiMagnitude;
            Balance = 0;
            BalanceInSatoshis = 0;
            PreviousBalance = 0;
            Reward = 0;
            Paid = false;
        }

        public void AddReward(decimal amount)
        {
            Reward += amount;
            CalculateBalance();
        }

        public void SetPreviousBalance(double amount)
        {
            PreviousBalance = (decimal)amount;
            CalculateBalance();
        }

        private void CalculateBalance()
        {
            Balance = PreviousBalance + Reward;
            BalanceInSatoshis = Balance * _satoshiMagnitude;
        }

        public override string ToString()
        {
            return string.Format("Worker: {0}, Balance: {1} Previous: {2} Reward: {3}", Worker, Balance, PreviousBalance, Reward);
        }
    }
}
