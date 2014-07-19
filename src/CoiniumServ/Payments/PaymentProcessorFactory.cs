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

using CoiniumServ.Coin.Config;
using CoiniumServ.Daemon;
using CoiniumServ.Mining.Pools.Config;
using CoiniumServ.Persistance;
using CoiniumServ.Repository.Context;
using Nancy.TinyIoc;

namespace CoiniumServ.Payments
{
    public class PaymentProcessorFactory : IPaymentProcessorFactory
    {
                /// <summary>
        /// The _kernel
        /// </summary>
        private readonly IApplicationContext _applicationContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentProcessorFactory" /> class.
        /// </summary>
        /// <param name="applicationContext">The application context.</param>
        public PaymentProcessorFactory(IApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public IPaymentProcessor Get(IDaemonClient daemonClient, IStorage storage, IWalletConfig walletConfig, ICoinConfig coinConfig)
        {
            var @params = new NamedParameterOverloads
            {
                {"daemonClient", daemonClient},
                {"storage", storage},
                {"walletConfig", walletConfig},
                {"coinConfig", coinConfig}
            };

            return _applicationContext.Container.Resolve<IPaymentProcessor>(@params);
        }
    }
}
