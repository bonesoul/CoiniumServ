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
using CoiniumServ.Payments;
using CoiniumServ.Persistance.Query;

namespace CoiniumServ.Persistance.Layers.Mpos
{
    public partial class MposStorage
    {
        public void AddPayment(IPayment payment)
        {
            // this function is not supported as this functionality is only required by payment processors which mpos itself is already one so and handles itself.
            throw new NotSupportedException();
        }

        public void UpdatePayment(IPayment payment)
        {
            // this function is not supported as this functionality is only required by payment processors which mpos itself is already one so and handles itself.
            throw new NotSupportedException();
        }

        public IList<IPayment> GetPendingPayments()
        {
            // TODO: implement me!
            throw new NotImplementedException();
        }

        public IList<IPaymentDetails> GetPaymentsForBlock(uint height)
        {
            throw new NotImplementedException();
        }

        public IList<IPaymentDetails> GetPaymentsForAccount(int id, IPaginationQuery paginationQuery)
        {
            throw new NotImplementedException();
        }

        public IPaymentDetails GetPaymentDetailsByTransactionId(uint id)
        {
            throw new NotImplementedException();
        }

        public IPaymentDetails GetPaymentDetailsByPaymentId(uint id)
        {
            throw new NotImplementedException();
        }

        public void AddTransaction(ITransaction transaction)
        {
            // this function is not supported as this functionality is only required by payment processors which mpos itself is already one so and handles itself.
            throw new NotSupportedException();
        }
    }
}
