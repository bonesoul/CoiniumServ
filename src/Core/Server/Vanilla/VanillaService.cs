/*
 *   Coinium - Crypto Currency Pool Software - https://github.com/CoiniumServ/CoiniumServ
 *   Copyright (C) 2013 - 2014, Coinium Project - http://www.coinium.org
 *
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using AustinHarris.JsonRpc;
using Coinium.Core.Mining.Wallet;
using Coinium.Core.Mining.Wallet.Responses;
using Serilog;

namespace Coinium.Core.Server.Vanilla
{
    /// <summary>
    /// Stratum protocol implementation.
    /// </summary>
    public class VanillaService : JsonRpcService
    {
        public VanillaService()
        { }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <remarks>Documentation:
        /// https://en.bitcoin.it/wiki/Getwork
        /// https://github.com/sinisterchipmunk/bitpool/wiki/Bitcoin-Mining-Pool-Developer's-Reference
        /// https://bitcointalk.org/index.php?topic=51281.0
        /// </remarks>
        [JsonRpcMethod("getwork")]
        public Getwork Getwork(string data = null)
        {
            // var context = (HttpRpcContext)JsonRpcContext.Current().Value;
            // var request = context.Request;
            // var miner = (GetworkMiner)(context.Miner);

            if (data == null)
                return WalletManager.Instance.Client.Getwork();
            else
            {                
                var result = WalletManager.Instance.Client.Getwork(data);
                if(result)
                    Log.Verbose("Found block!: {0}", data);

                return null;
            }                
        }     
    }
}
