/*
 *   CoiniumServ - crypto currency pool software - https://github.com/CoiniumServ/CoiniumServ
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

/* This file is based on https://github.com/BitKoot/BitcoinRpcSharp */

using Newtonsoft.Json;

namespace Coinium.Coin.Daemon
{
    /// <summary>
    /// Class containing the information contained in a JSON RPC response received from
    /// the Bitcoin wallet.
    /// </summary>
    /// <typeparam name="T">
    /// Type of the result object. This can simply be a string like a transaction id, 
    /// or a more complex object like TransactionDetails.
    /// </typeparam>
    public class DaemonResponse<T>
    {
        /// <summary>
        /// The result object.
        /// </summary>
        [JsonProperty(PropertyName = "result", Order = 0)]
        public T Result { get; set; }

        /// <summary>
        /// The id of the corresponding request.
        /// </summary>
        [JsonProperty(PropertyName = "id", Order = 1)]
        public int Id { get; set; }

        /// <summary>
        /// The error returned by the wallet, if any.
        /// </summary>
        [JsonProperty(PropertyName = "error", Order = 2)]
        public string Error { get; set; }

        /// <summary>
        /// Create a new JSON RPC response with the given id, error and result object.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="error">The error.</param>
        /// <param name="result">The result object.</param>
        public DaemonResponse(int id, string error, T result)
        {
            Id = id;
            Error = error;
            Result = result;
        }
    }
}
