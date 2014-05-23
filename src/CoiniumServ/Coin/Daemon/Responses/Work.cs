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

namespace Coinium.Coin.Daemon.Responses
{
    // Documentation: 
    // https://en.bitcoin.it/wiki/Getwork
    // https://github.com/sinisterchipmunk/bitpool/wiki/Bitcoin-Mining-Pool-Developer's-Reference
    // https://bitcointalk.org/index.php?topic=51281.0

    public class Work
    {
        /// <summary>
        /// This should be advertised iff the miner supports generating its own midstates. In this case, the pool may decide to omit the now-deprecated "midstate" and "hash1" fields in the work response.
        /// </summary>
        public string MidState { get; set; }

        /// <summary>
        /// Pre-processed SHA-2 input chunks, in little-endian order, as a hexadecimal-encoded string
        /// </summary>
        public string Data { get; set; }

        public string Hash1 { get; set; }

        /// <summary>
        /// Proof-of-work hash target as a hexadecimal-encoded string
        /// </summary>
        public string Target { get; set; }

        /// <summary>
        /// Brief specification of proof-of-work algorithm. Not provided by bitcoind or most poolservers.
        /// </summary>
        public string Algorithm { get; set; }
    }
}
